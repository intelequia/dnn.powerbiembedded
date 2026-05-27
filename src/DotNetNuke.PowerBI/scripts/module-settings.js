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
        }).fail(function (error, exception) {
            console.error(error);
        }).always(function () {
            $contentItems.prop('disabled', false);
        });
    };

    this.init = function () {
        that.workspaces = $('#SettingsGroupId');

        that.workspaces.on('change', function () {
            that.refreshContentItems(that.workspaces.val());
        });
    };
};
