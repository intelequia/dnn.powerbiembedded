var app = app || {};

app.settings = function (context) {
    var that = this;
    this.service = {
        path: 'PowerBI/Services',
        controller: 'ModuleSettings',
        framework: $.ServicesFramework(context.ModuleId)
    };
    this.service.baseUrl = that.service.framework.getServiceRoot(that.service.path);
    this.$workspaces;

    // These settings only apply to reports (content item values prefixed with 'R_').
    // They are also shown when no workspace or no content item is selected, since that
    // is the configuration used when no specific content item is targeted.
    this.toggleReportSectionName = function () {
        var $contentItems = $('#ContentItemId');
        var $reportOnlyItems = $('.pbi-report-only');
        if ($contentItems.length === 0 || $reportOnlyItems.length === 0) {
            return;
        }

        var contentItemValue = $contentItems.val() || '';
        var workspaceValue = ($('#SettingsGroupId').val() || '');
        var isReport = contentItemValue.indexOf('R_') === 0;
        var noSelection = workspaceValue === '' || contentItemValue === '';
        $reportOnlyItems.toggleClass('pbi-hidden', !(isReport || noSelection));
    };

    // Shows the matching RLS textbox depending on the selected User Property Method.
    this.toggleRlsUserProperty = function () {
        var $userProperty = $('#UserProperty');
        var $customUserPropertyItem = $('#CustomUserPropertyItem');
        var $customExtensionLibraryItem = $('#CustomExtensionLibraryItem');
        if ($userProperty.length === 0) {
            return;
        }

        var value = $userProperty.val() || '';
        $customUserPropertyItem.toggleClass('pbi-hidden', value !== 'Custom User Profile Property');
        $customExtensionLibraryItem.toggleClass('pbi-hidden', value !== 'Custom Extension Library');
    };

    this.refreshContentItems = function (groupId) {
        let params = {
            groupId: groupId,
        };

        var $contentItems = $('#ContentItemId');
        if ($contentItems.length === 0) { 
            // The Content Item dropdown is only rendered for ContentView, nothing to refresh.
            return;
        }

        var previousValue = $contentItems.val();
        $contentItems.prop('disabled', true);

        $.ajax({
            url: that.service.baseUrl + that.service.controller + '/GetContentItemsByGroup',
            type: 'GET',
            async: true,
            data: params,
            dataType: 'json',
            headers: {
                'PortalId': context.PortalId,
                'ModuleId': context.ModuleId,
                'TabId': context.TabId,
                'RequestVerificationToken': $.ServicesFramework().getAntiForgeryValue(),
            }
        }).done(function (data) {
            // Web API may serialize using either PascalCase or camelCase depending on the
            // configured JsonFormatter. Be tolerant to both.
            var contentItems = data && (data.contentItems || data.ContentItems);
            if (contentItems == null) {
                return;
            }

            var dashboards = contentItems.Dashboards || contentItems.dashboards || [];
            var reports = contentItems.Reports || contentItems.reports || [];

            $contentItems.empty();
            $contentItems.append($('<option>', { value: '', text: 'Choose one...' }));

            $.each(dashboards, function (i, item) {
                var id = item.Id || item.id;
                var displayName = item.DisplayName || item.displayName;
                $contentItems.append($('<option>', {
                    text: 'Dashboard - ' + displayName,
                    value: 'D_' + id,
                }));
            });

            $.each(reports, function (i, item) {
                var id = item.Id || item.id;
                var name = item.Name || item.name;
                $contentItems.append($('<option>', {
                    text: 'Reports - ' + name,
                    value: 'R_' + id,
                }));
            });

            // Preserve the previously selected item if it still exists in the new list.
            if (previousValue && $contentItems.find('option[value="' + previousValue + '"]').length > 0) {
                $contentItems.val(previousValue);
            }

            that.toggleReportSectionName();
            that.refreshReportPages();
        }).fail(function (error, exception) {
            console.error(error);
        }).always(function () {
            $contentItems.prop('disabled', false);
        });
    };

    // Populates the Report Section Name dropdown (#PageName) with the pages of the
    // selected report, fetched from the Power BI Get Pages REST API.
    this.refreshReportPages = function () {
        var $contentItems = $('#ContentItemId');
        var $pageName = $('#PageName');
        if ($contentItems.length === 0 || $pageName.length === 0) {
            return;
        }

        var contentItemValue = $contentItems.val() || '';
        if (contentItemValue.indexOf('R_') !== 0) {
            // Only reports have pages; leave the (hidden) dropdown untouched.
            return;
        }

        var params = {
            groupId: $('#SettingsGroupId').val() || '',
            reportId: contentItemValue
        };

        var previousValue = $pageName.val();
        $pageName.prop('disabled', true);

        $.ajax({
            url: that.service.baseUrl + that.service.controller + '/GetReportPages',
            type: 'GET',
            async: true,
            data: params,
            dataType: 'json',
            headers: {
                'PortalId': context.PortalId,
                'ModuleId': context.ModuleId,
                'TabId': context.TabId,
                'RequestVerificationToken': $.ServicesFramework().getAntiForgeryValue(),
            }
        }).done(function (data) {
            var pages = data && (data.pages || data.Pages);
            if (pages == null) {
                return;
            }

            $pageName.empty();
            $pageName.append($('<option>', { value: '', text: '(Default page)' }));

            $.each(pages, function (i, page) {
                var name = page.name || page.Name;
                var displayName = page.displayName || page.DisplayName || name;
                $pageName.append($('<option>', {
                    text: displayName,
                    value: name,
                }));
            });

            // Preserve the previously selected page if it still exists in the new list.
            if (previousValue && $pageName.find('option[value="' + previousValue + '"]').length > 0) {
                $pageName.val(previousValue);
            }
        }).fail(function (error) {
            console.error(error);
        }).always(function () {
            $pageName.prop('disabled', false);
        });
    };

    this.init = function () {
        that.workspaces = $('#SettingsGroupId');

        that.workspaces.on('change', function () {
            that.refreshContentItems(that.workspaces.val());
            that.toggleReportSectionName();
        });

        // Use delegated handlers bound to the document so they survive DNN partial
        // postbacks that re-render the settings form (and therefore the dropdown). 
        $(document).off('change.pbiReportSection', '#ContentItemId')
            .on('change.pbiReportSection', '#ContentItemId', function () {
                that.toggleReportSectionName();
                that.refreshReportPages();
            });

        $(document).off('change.pbiRlsUserProperty', '#UserProperty')
            .on('change.pbiRlsUserProperty', '#UserProperty', function () {
                that.toggleRlsUserProperty();
            });

        // Re-apply the initial visibility after each partial postback, since the
        // re-rendered dropdown defaults to "visible" until we evaluate it again.
        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                that.toggleReportSectionName();
                that.toggleRlsUserProperty();
            });
        }

        that.toggleReportSectionName();
        that.toggleRlsUserProperty();
        that.refreshReportPages();
    };
};
