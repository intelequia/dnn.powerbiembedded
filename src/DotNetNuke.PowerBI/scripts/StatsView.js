(function (window, $) {
    "use strict";

    window.app = window.app || {};

    function toLocalDisplay(iso) {
        if (!iso) { return "-"; }
        var d = new Date(iso);
        if (isNaN(d.getTime())) { return "-"; }
        return d.toLocaleString();
    }

    function toElapsedDisplay(iso) {
        if (!iso) { return "-"; }
        var d = new Date(iso);
        if (isNaN(d.getTime())) { return "-"; }

        var diffMs = Date.now() - d.getTime();
        if (diffMs < 0) { diffMs = 0; }

        var totalMinutes = Math.floor(diffMs / 60000);
        if (totalMinutes < 60) {
            return totalMinutes + 'm';
        }

        var totalHours = Math.floor(totalMinutes / 60);
        var minutes = totalMinutes % 60;
        if (totalHours < 24) {
            return totalHours + 'h' + (minutes > 0 ? (' ' + minutes + 'm') : '');
        }

        var days = Math.floor(totalHours / 24);
        var hours = totalHours % 24;
        return days + 'd' + (hours > 0 ? (' ' + hours + 'h') : '');
    }

    function safeInt(value) {
        var n = parseInt(value, 10);
        return isNaN(n) ? 0 : n;
    }

    // ApexCharts integration
    var _apexInstance = null;

    function loadApexCharts(callback) {
        if (typeof ApexCharts !== 'undefined') {
            callback();
            return;
        }
        var s = document.createElement('script');
        s.src = 'https://cdn.jsdelivr.net/npm/apexcharts/dist/apexcharts.min.js';
        s.onload = callback;
        s.onerror = function () { callback(true); };
        document.head.appendChild(s);
    }

    function renderLineChart($target, points, noDataText) {
        if (!points || points.length === 0) {
            if (_apexInstance) { _apexInstance.destroy(); _apexInstance = null; }
            $target.empty();
            $target.append('<div class="pbi-stats-chart-empty">' + (noDataText || 'No data') + '</div>');
            return;
        }

        loadApexCharts(function (err) {
            if (err || typeof ApexCharts === 'undefined') {
                $target.empty();
                $target.append('<div class="pbi-stats-chart-empty">Chart library unavailable</div>');
                return;
            }

            var data = [];
            var categories = [];
            for (var i = 0; i < points.length; i++) {
                data.push(safeInt(points[i].Views));
                categories.push(points[i].Label || '');
            }

            var options = {
                chart: {
                    type: 'area',
                    height: 240,
                    fontFamily: 'inherit',
                    toolbar: { show: false },
                    animations: { enabled: true, easing: 'easeinout', speed: 600 },
                    selection: { enabled: false }
                },
                series: [{ name: 'Views', data: data }],
                xaxis: {
                    categories: categories,
                    tooltip: { enabled: false },
                    axisBorder: { show: false }
                },
                yaxis: {
                    labels: { formatter: function (v) { return Math.round(v); } }
                },
                colors: ['#4263eb'],
                fill: {
                    type: 'gradient',
                    gradient: { shadeIntensity: 1, opacityFrom: 0.45, opacityTo: 0.05, stops: [0, 90, 100] }
                },
                stroke: { curve: 'smooth', width: 3 },
                dataLabels: { enabled: false },
                grid: { strokeDashArray: 4, borderColor: 'rgba(98,105,118,.16)' },
                tooltip: { theme: 'light' }
            };

            if (_apexInstance) {
                _apexInstance.updateOptions(options, true, true);
            } else {
                $target.empty();
                _apexInstance = new ApexCharts($target[0], options);
                _apexInstance.render();
            }
        });
    }

    function renderMostViewed($target, items, viewsText, noDataText) {
        $target.empty();
        if (!items || items.length === 0) {
            $target.append('<div class="pbi-stats-empty">' + noDataText + '</div>');
            return;
        }

        var maxViews = 0;
        for (var m = 0; m < items.length; m++) {
            var v = safeInt(items[m].Views);
            if (v > maxViews) { maxViews = v; }
        }
        if (maxViews <= 0) { maxViews = 1; }

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var color = item.Color || '#4263eb';
            var views = safeInt(item.Views);
            var pct = Math.round((views / maxViews) * 100);
            if (pct < 2) { pct = 2; }

            var tagsHtml = '';
            if (item.Tags && item.Tags.length) {
                tagsHtml += '<span class="pbi-stats-list-tags">';
                for (var t = 0; t < item.Tags.length; t++) {
                    var tag = (item.Tags[t] || '').toString();
                    if (!tag) { continue; }
                    tagsHtml += '<span class="pbi-stats-list-tag">' + tag + '</span>';
                }
                tagsHtml += '</span>';
            }

            var html = '' +
                '<a class="pbi-stats-list-item" href="' + (item.Url || '#') + '">' +
                '<div class="pbi-stats-list-row">' +
                '<span class="pbi-stats-list-index" style="background:' + color + '">' + (i + 1) + '</span>' +
                '<span class="pbi-stats-list-title">' + (item.Title || '') + '</span>' +
                tagsHtml +
                '<span class="pbi-stats-list-views">' + views.toLocaleString() + '</span>' +
                '</div>' +
                '<div class="pbi-stats-list-progress">' +
                '<div class="pbi-stats-list-bar" style="width:' + pct + '%;background:' + color + '"></div>' +
                '</div>' +
                '</a>';
            $target.append(html);
        }
    }

    window.app.initStatsView = function (context, strings, defaultRange) {
        var moduleId = context.ModuleId;
        var $root = $('#pbiStats_' + moduleId);
        if ($root.length === 0) { return; }

        var sf = $.ServicesFramework(moduleId);
        var apiRoot = sf.getServiceRoot('PowerBI/Services') + 'Stats/';

        var $error = $root.find('[data-role="error"]');
        var $content = $root.find('[data-role="content"]');
        var $chart = $root.find('[data-role="chart"]');
        var $mostViewed = $root.find('[data-role="mostViewed"]');
        var $ranges = $root.find('[data-role="ranges"]');
        var $lastRefreshTile = $root.find('[data-role="lastRefreshTile"]');
        var lastRefreshIso = null;
        var previousKpis = {
            publishedReports: null,
            activeUsers24h: null
        };

        function setLoading(mode, isLoading) {
            if (mode === 'chart') {
                $root.toggleClass('is-loading-chart', !!isLoading);
            } else {
                $root.toggleClass('is-loading', !!isLoading);
            }
            $root.attr('aria-busy', !!isLoading);
        }

        function setError(message) {
            if (message) {
                $error.text(message).show();
            } else {
                $error.hide().text('');
            }
        }

        function applyHealth(health, note) {
            var $dot = $root.find('[data-field="lastRefreshDot"]');
            var $note = $root.find('[data-field="lastRefreshNote"]');
            var cls = 'pbi-stats-dot--green';

            if (health === 'red') {
                cls = 'pbi-stats-dot--red';
            } else if (health === 'yellow') {
                cls = 'pbi-stats-dot--yellow';
            }

            $dot.removeClass('pbi-stats-dot--green pbi-stats-dot--yellow pbi-stats-dot--red').addClass(cls);
            $note.text(note || '');
        }

        function applyTrend(fieldName, nextValue, trendFieldSelector, options) {
            var previous = previousKpis[fieldName];
            var delta = previous == null ? 0 : (nextValue - previous);
            var isDown = delta < 0;
            var sign = delta > 0 ? '+' : '';
            var iconClass = isDown ? 'ti ti-trending-down' : 'ti ti-trending-up';
            var trendClass = isDown ? 'pbi-stats-kpi-sub--down' : 'pbi-stats-kpi-sub--up';
            var text = '';
            var $trend = $root.find(trendFieldSelector);

            options = options || {};

            if (options.mode === 'percent') {
                var base = typeof options.baseValue === 'number'
                    ? options.baseValue
                    : (previous == null ? nextValue : previous);
                delta = nextValue - base;
                isDown = delta < 0;
                iconClass = isDown ? 'ti ti-trending-down' : 'ti ti-trending-up';
                trendClass = isDown ? 'pbi-stats-kpi-sub--down' : 'pbi-stats-kpi-sub--up';
                var pct = base > 0 ? Math.round((delta / base) * 100) : 0;
                var pctSign = pct > 0 ? '+' : '';
                text = pctSign + pct.toLocaleString() + '% ' + (options.suffix || '');
            } else if (options.mode === 'countFixed') {
                var fixedCount = safeInt(options.countValue);
                iconClass = 'ti ti-trending-up';
                trendClass = 'pbi-stats-kpi-sub--up';
                text = '+' + fixedCount.toLocaleString() + (options.suffix ? (' ' + options.suffix) : '');
            } else {
                var shownDelta = previous == null ? Math.abs(delta) : delta;
                text = sign + shownDelta.toLocaleString() + (options.suffix ? (' ' + options.suffix) : '');
            }

            $trend.removeClass('pbi-stats-kpi-sub--up pbi-stats-kpi-sub--down').addClass(trendClass);
            $trend.find('i').attr('class', iconClass);
            $trend.find('span').text(text);

            previousKpis[fieldName] = nextValue;
        }

        function applyLastRefresh(iso) {
            var $lastRefresh = $root.find('[data-field="lastRefresh"]');
            var absoluteText = toLocalDisplay(iso);
            var elapsedText = toElapsedDisplay(iso);

            $lastRefresh.text(elapsedText);
            if (absoluteText !== '-') {
                $lastRefresh.attr('title', absoluteText);
            } else {
                $lastRefresh.removeAttr('title');
            }
        }

        function applyLastRefreshClickUrl(url) {
            if (!$lastRefreshTile.length) { return; }
            if (url) {
                $lastRefreshTile
                    .addClass('pbi-stats-kpi--clickable')
                    .css('cursor', 'pointer')
                    .off('click.lastRefresh')
                    .on('click.lastRefresh', function () {
                        window.location.href = url;
                    });
            } else {
                $lastRefreshTile
                    .removeClass('pbi-stats-kpi--clickable')
                    .css('cursor', '')
                    .off('click.lastRefresh');
            }
        }

        function updateRelativeLastRefresh() {
            if (lastRefreshIso) {
                applyLastRefresh(lastRefreshIso);
            }
        }

        function load(range, chartOnly) {
            if (!chartOnly) {
                setError('');
            }
            setLoading(chartOnly ? 'chart' : 'full', true);

            $.ajax({
                url: apiRoot + 'Get',
                type: 'GET',
                dataType: 'json',
                data: { range: range },
                headers: {
                    PortalId: context.PortalId,
                    ModuleId: context.ModuleId,
                    TabId: context.TabId,
                    RequestVerificationToken: $.ServicesFramework().getAntiForgeryValue()
                }
            }).done(function (data) {
                var model = data || {};
                if (model.ErrorMessage) {
                    setError(model.ErrorMessage);
                    return;
                }

                if (!chartOnly) {
                    var publishedReports = safeInt(model.PublishedReports);
                    var activeUsers24h = safeInt(model.ActiveUsers24h);

                    $root.find('[data-field="publishedReports"]').text(publishedReports.toLocaleString());
                    $root.find('[data-field="activeUsers24h"]').text(activeUsers24h.toLocaleString());
                    $root.find('[data-field="datasetsTotal"]').text(safeInt(model.DatasetsTotal).toLocaleString());
                    $root.find('[data-field="datasetsAutoRefreshed"]').text(
                        safeInt(model.DatasetsAutoRefreshed24h).toLocaleString() + ' ' + (strings.autoRefreshed || 'auto-refreshed')
                    );

                    lastRefreshIso = model.LastRefreshUtc || null;
                    applyLastRefresh(lastRefreshIso);
                    applyLastRefreshClickUrl(model.LastRefreshClickUrl || null);

                    applyTrend('publishedReports', publishedReports, '[data-field="publishedReportsTrend"]', {
                        mode: 'countFixed',
                        countValue: safeInt(model.PublishedReportsThisMonth),
                        suffix: 'this month'
                    });
                    applyTrend('activeUsers24h', activeUsers24h, '[data-field="activeUsers24hTrend"]', {
                        mode: 'percent',
                        baseValue: safeInt(model.ActiveUsersYesterday),
                        suffix: 'vs. yesterday'
                    });

                    var healthNote = model.LastRefreshNote || '';
                    if (!healthNote) {
                        healthNote = model.LastRefreshHealth === 'red'
                            ? (strings.lastRefreshError || '')
                            : (model.LastRefreshHealth === 'yellow' ? (strings.lastRefreshWarning || '') : (strings.lastRefreshOk || ''));
                    }
                    applyHealth(model.LastRefreshHealth, healthNote);

                    if (!model.HasApplicationInsightsConfig) {
                        setError(strings.notConfigured || 'Application Insights credentials are not configured.');
                    }
                }

                renderLineChart($chart, model.Trend || [], strings.noData || 'No data');

                if (!chartOnly) {
                    renderMostViewed($mostViewed, model.MostViewed || [], strings.views || 'views', strings.noData || 'No data');
                }
            }).fail(function () {
                if (chartOnly) {
                    renderLineChart($chart, [], strings.error || 'Error loading stats.');
                } else {
                    setError(strings.error || 'Error loading stats.');
                }
            }).always(function () {
                setLoading(chartOnly ? 'chart' : 'full', false);
            });
        }

        $ranges.on('click', 'button[data-range]', function () {
            var $btn = $(this);
            $ranges.find('button').removeClass('active');
            $btn.addClass('active');
            load($btn.attr('data-range') || '14d', true);
        });

        load((defaultRange || '14d'), false);
        setInterval(updateRelativeLastRefresh, 60000);
    };
})(window, jQuery);
