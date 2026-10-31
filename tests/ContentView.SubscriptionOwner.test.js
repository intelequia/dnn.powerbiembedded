const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

const root = path.join(__dirname, '..', 'src', 'DotNetNuke.PowerBI');
const source = fs.readFileSync(path.join(root, 'scripts', 'ContentView.js'), 'utf8');
const ko = require(path.join(root, 'scripts', 'vendors', 'knockout.min.js'));
ko.validation = { group: () => () => [] };
const sandbox = { ko };
vm.createContext(sandbox);
vm.runInContext(source.slice(source.indexOf('    function parseRepeatTime('), source.indexOf('    function BookmarkModel(')), sandbox);

function subscription(owner, id = 1) {
    const parent = { pagesArray: ko.observableArray([]), editing: ko.observable(false) };
    return new sandbox.SubscriptionModel(parent, id, 1, 'report', 'workspace', 'Name',
        '2026-01-01', '2026-12-31', 'Daily', '09:00:00', 'UTC', 'Subject', 'Message',
        [], true, [], [], 42, false, '', null, '', '', false, owner);
}

test('the owner is read-only display data and survives cancelling edits', () => {
    const model = subscription('Original creator');
    assert.equal(model.ownerDisplayName(), 'Original creator');
    model.name('Changed name');
    model.cancelEditing();
    assert.equal(model.ownerDisplayName(), 'Original creator');
    assert.equal(model.name(), 'Name');
});

test('missing owners and unsaved subscriptions do not acquire a guessed owner', () => {
    assert.equal(subscription(null).ownerDisplayName(), '');
    assert.equal(subscription(undefined, -1).ownerDisplayName(), '');
});

test('API owner data reaches the model and both views use escaped text bindings', () => {
    assert.match(source, /subscription\.LastDayOfMonth,\s*subscription\.OwnerDisplayName,/);
    const view = fs.readFileSync(path.join(root, 'Views', 'ContentView', 'Index.cshtml'), 'utf8');
    assert.equal((view.match(/text: ownerDisplayName, visible: ownerDisplayName/g) || []).length, 2);
    assert.equal((view.match(/LocalizeString\("SubscriptionOwnerUnavailable"\)/g) || []).length, 2);
    assert.match(view, /class="dnnFormItem" data-bind="visible: id\(\) > 0"/);
});

test('owner labels and unavailable-user text exist in every supported locale', () => {
    for (const locale of ['', '.es-es', '.de-de']) {
        const resource = fs.readFileSync(path.join(root, 'App_LocalResources', `ContentView${locale}.resx`), 'utf8');
        for (const key of ['SubscriptionOwner', 'SubscriptionOwnerUnavailable']) {
            assert.match(resource, new RegExp(`<data name="${key}\\.Text"[^>]*>\\s*<value>[^<]+</value>`));
        }
    }
});
