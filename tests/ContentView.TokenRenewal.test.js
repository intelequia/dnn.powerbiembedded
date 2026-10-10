const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

const source = fs.readFileSync(path.join(__dirname, '../src/DotNetNuke.PowerBI/scripts/ContentView.js'), 'utf8');
const renewalSource = source.slice(source.indexOf('    function createTokenRenewal('), source.indexOf('    function parseRepeatTime('));

function harness(minutes = 60) {
    let now = Date.parse('2026-10-10T12:00:00Z');
    let nextTimer = 0;
    const timers = new Map();
    const documentEvents = new Map();
    const windowEvents = new Map();
    const requests = [];
    const tokens = [];
    const config = { accessToken: 'initial' };
    const context = {
        ModuleId: 42,
        SettingsGroupId: 'settings-group',
        Id: 'report-id',
        ContentType: 'report',
        Expiration: new Date(now + minutes * 60000).toISOString()
    };
    const report = {
        element: {},
        setAccessToken: token => { tokens.push(token); return Promise.resolve(); }
    };
    const document = {
        hidden: false,
        documentElement: { contains: () => true },
        addEventListener: (name, handler) => documentEvents.set(name, handler),
        removeEventListener: name => documentEvents.delete(name)
    };
    const sandbox = {
        Date: class extends Date { static now() { return now; } },
        console: { error() {} },
        Promise,
        document,
        window: {
            addEventListener: (name, handler) => windowEvents.set(name, handler),
            removeEventListener: name => windowEvents.delete(name)
        },
        $: {
            ServicesFramework: moduleId => ({
                getServiceRoot: () => '/API/PowerBI/Services/',
                setModuleHeaders() {},
                moduleId
            }),
            ajax: options => {
                const request = { options, params: JSON.parse(options.data) };
                requests.push(request);
                const response = {
                    done: callback => { request.success = callback; return response; },
                    fail: callback => { request.fail = callback; return response; }
                };
                return response;
            }
        },
        setTimeout: (handler, delay) => {
            const timerId = ++nextTimer;
            timers.set(timerId, { handler, at: now + delay });
            return timerId;
        },
        clearTimeout: timerId => timers.delete(timerId)
    };
    vm.createContext(sandbox);
    vm.runInContext(renewalSource, sandbox);
    sandbox.createTokenRenewal(context, () => report, config);

    function advance(milliseconds, runTimers = true) {
        now += milliseconds;
        if (runTimers) {
            const due = [...timers.entries()].filter(([, timer]) => timer.at <= now);
            due.forEach(([timerId, timer]) => {
                timers.delete(timerId);
                timer.handler();
            });
        }
    }

    return {
        context, config, report, requests, tokens, timers, document, documentEvents, windowEvents,
        advance,
        expiration: minutesFromNow => new Date(now + minutesFromNow * 60000).toISOString(),
        startAnother: (anotherContext, anotherReport, anotherConfig) => sandbox.createTokenRenewal(anotherContext, () => anotherReport, anotherConfig),
        flush: () => new Promise(resolve => setImmediate(resolve))
    };
}

test('renews five minutes before expiry and applies token without re-embedding', async () => {
    const state = harness();
    state.advance(54 * 60000);
    assert.equal(state.requests.length, 0);
    state.advance(60000);
    const request = state.requests[0];
    assert.equal(request.options.type, 'POST');
    assert.equal(request.options.url, '/API/PowerBI/Services/EmbedToken/Renew');
    assert.equal(request.options.timeout, 30000);
    assert.equal(typeof request.options.beforeSend, 'function');
    assert.equal(request.params.SettingsGroupId, state.context.SettingsGroupId);
    assert.equal(request.params.Id, state.context.Id);
    request.success({ token: 'renewed', expiration: state.expiration(60) });
    await state.flush();
    assert.deepEqual(state.tokens, ['renewed']);
    assert.equal(state.config.accessToken, 'renewed');
    state.advance(55 * 60000);
    assert.equal(state.requests.length, 2);
});

test('checks expiry on visibility recovery and prevents concurrent renewals', () => {
    const state = harness();
    state.advance(61 * 60000, false);
    state.documentEvents.get('visibilitychange')();
    state.documentEvents.get('visibilitychange')();
    state.advance(60000);
    assert.equal(state.requests.length, 1);
});

test('retries temporary failures and SDK rejection', async () => {
    const state = harness();
    state.advance(55 * 60000);
    state.requests[0].fail({ status: 503 });
    state.advance(9999);
    assert.equal(state.requests.length, 1);
    state.advance(1);
    assert.equal(state.requests.length, 2);
    state.report.setAccessToken = () => Promise.reject(new Error('iframe failure'));
    state.requests[1].success({ token: 'new', expiration: state.expiration(60) });
    await state.flush();
    assert.equal(state.config.accessToken, 'initial');
    state.advance(20000);
    assert.equal(state.requests.length, 3);
});

test('stops on authorization failures', () => {
    for (const status of [401, 403]) {
        const state = harness();
        state.advance(55 * 60000);
        state.requests[0].fail({ status });
        state.advance(60000);
        assert.equal(state.requests.length, 1);
        assert.equal(state.timers.size, 0);
        assert.equal(state.documentEvents.size, 0);
    }
});

test('rejects malformed or expired renewal responses', () => {
    for (const response of [{ token: 'new', expiration: 'invalid' }, { token: 'new', expiration: '2000-01-01T00:00:00Z' }, {}]) {
        const state = harness();
        state.advance(55 * 60000);
        state.requests[0].success(response);
        assert.deepEqual(state.tokens, []);
        state.advance(10000);
        assert.equal(state.requests.length, 2);
    }
});

test('uses half the remaining lifetime for short-lived tokens', () => {
    const state = harness(4);
    state.advance(2 * 60000);
    assert.equal(state.requests.length, 1);
});

test('keeps renewal state scoped to each module', async () => {
    const state = harness();
    const otherTokens = [];
    const otherContext = { ...state.context, ModuleId: 43, Id: 'second-report' };
    const otherConfig = { accessToken: 'other-initial' };
    state.startAnother(otherContext, { element: {}, setAccessToken: token => { otherTokens.push(token); return Promise.resolve(); } }, otherConfig);
    state.advance(55 * 60000);
    assert.equal(state.requests.length, 2);
    state.requests[0].success({ token: 'first-new', expiration: state.expiration(60) });
    await state.flush();
    assert.deepEqual(state.tokens, ['first-new']);
    assert.deepEqual(otherTokens, []);
    assert.equal(otherConfig.accessToken, 'other-initial');
});

test('resumes renewal after returning from browser back-forward cache', () => {
    const state = harness();
    state.windowEvents.get('pagehide')();
    state.advance(61 * 60000, false);
    state.windowEvents.get('pageshow')();
    assert.equal(state.requests.length, 1);
});