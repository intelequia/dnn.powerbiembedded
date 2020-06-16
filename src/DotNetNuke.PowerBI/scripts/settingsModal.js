jQuery(function ($) {
    var app = app || {};
    var context = JSON.parse(dnn.getVar("PowerBISettings_Context"));


    function SettingsViewModel(settings) {
        var that = this;

        this.SettingsId = ko.observable(settings.SettingsId);
        this.SettingsGroupId = ko.observable(settings.SettingsGroupId);
        this.SettingsGroupName = ko.observable(settings.SettingsGroupName);
        this.AuthType = ko.observable(settings.AuthenticationType);
        this.ContentPageUrl = ko.observable(settings.ContentPageUrl);
    }

    app.Settings = function () {
        var that = this;
        var service = {
            path: "PowerBI/UI",
            framework: $.ServicesFramework(context.ModuleId),
            controller: "GroupSettings"
        };
        service.baseUrl = service.framework.getServiceRoot(service.path);

        this.SettingsId = ko.observable(-1);

        this.AuthTypes = ko.observableArray(['MasterUser', 'ServicePrincipal']);

        this.SelectedAuth = ko.observable();
        this.SettingsGroupName = ko.observable();

        this.Username = ko.observable();
        this.Password = ko.observable();

        this.ServicePrincipalApplicationId = ko.observable();
        this.ServicePrincipalApplicationSecret = ko.observable();
        this.ServicePrincipalApplicationTenant = ko.observable();

        this.ApplicationId = ko.observable();
        this.WorkspaceId = ko.observable();
        this.ContentPageUrl = ko.observable();

        this.SettingsArray = ko.observableArray();
          
        this.AddSettings = function () {
            service.controller = "GroupSettings";

            var params = {
                SettingsId: that.SettingsId(),
                AuthenticationType: that.SelectedAuth(),
                SettingsGroupName: that.SettingsGroupName(),
                Username: that.Username(),
                Password: that.Password(),
                ServicePrincipalApplicationId: that.ServicePrincipalApplicationId(),
                ServicePrincipalApplicationSecret: that.ServicePrincipalApplicationSecret(),
                ServicePrincipalApplicationTenant: that.ServicePrincipalApplicationTenant(),
                ApplicationId: that.ApplicationId(),
                WorkspaceId: that.WorkspaceId(),
                ContentPageUrl: that.ContentPageUrl(),
            };
             
            Common.Call("POST",
                "AddOrEditSettings",
                service,
                params,
                function (response) {
                    if (response.Success) {
                        console.log("success");
                        that.GetSettingsGroups();
                    } else {
                        console.log("error");
                        //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                    }
                },
                function (error) {
                    console.log("error");
                    //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                },
                function () {
                    //that.isBusy(false);
                });
        };

        this.GetSettingsGroups = function () {
            service.controller = "GroupSettings";
            Common.Call("GET",
                "GetSettingsGroups",
                service,
                null,
                function (response) {
                    if (response.Success) {
                        console.log("success");
                        that.SettingsArray.removeAll();
                        ko.utils.arrayForEach(response.Data,
                            function (item) {
                                that.SettingsArray.push(new SettingsViewModel(item));
                            });
                        that.SettingsArray.valueHasMutated();
                    } else {
                        console.log("error");
                        //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                    }
                },
                function (error) {
                    console.log("error");
                    //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                },
                function () {
                    //that.isBusy(false);
                });
        } 

        this.deleteSettings = function (s) {
            service.controller = "GroupSettings";
            var params = {
                settingsId: s.SettingsId()
            };
            Common.Call("POST",
                "DeleteSettings",
                service,
                params,
                function (response) {
                    if (response.Success) {
                        console.log("success");
                        that.GetSettingsGroups();
                    } else {
                        console.log("error");
                        //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                    }
                },
                function (error) {
                    console.log("error");
                    //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                },
                function () {
                    //that.isBusy(false);
                });
        };
    };
    var viewmodel = new app.Settings();
    viewmodel.GetSettingsGroups();
    ko.applyBindings(viewmodel, $("#SharedSettingsList")[0]);
});