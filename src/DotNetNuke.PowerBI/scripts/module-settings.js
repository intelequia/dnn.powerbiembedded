var app = app || {};

app.settings = function (context) {
    var that = this;
    this.service = {
        path: 'Bookmarks',
        controller: 'ModuleSettings',
        framework: $.ServicesFramework(context.ModuleId)
    };
    this.service.baseUrl = that.service.framework.getServiceRoot(that.service.path);
    this.$workspaces;

    this.refreshContentItems = function (groupId) {
        let params = {
            groupId: groupId,
        };

        $.ajax({
            url: that.service.baseUrl + that.service.controller + '/GetContentItemsByGroup',
            type: 'GET',
            async: true,
            data: params,
            dataType: '',
            contentType: '',
            headers: {
                'PortalId': context.PortalId,
                'ModuleId': context.ModuleId,
                'TabId': context.TabId,
                'RequestVerificationToken': $.ServicesFramework().getAntiForgeryValue(),
            }
        }).done(function (data) {
            if(data.contentItems != null) {
                var $contentItems = $('#ContentItemId');
                $contentItems.empty();
                $contentItems.append($('<option>', { value: '', text: 'Choose one...' }));

                $.each(data.contentItems.Dashboards, function (i, item) {
                    $contentItems.append($('<option>', { 
                        text: 'Dashboard - ' + item.displayName,
                        value: 'D_' + item.id,
                    }));
                });
                
                $.each(data.contentItems.Reports, function (i, item) {
                    $contentItems.append($('<option>', { 
                        text: 'Reports - ' + item.name,
                        value: 'R_' + item.id,
                    }));
                });
            }
        }).fail(function (error, exception) {
            console.error(error);
        });
    };

    this.init = function () {
        that.workspaces = $('#SettingsGroupId');

        that.workspaces.on('change', function () {
            that.refreshContentItems(that.workspaces.val());
        });
    };
};
