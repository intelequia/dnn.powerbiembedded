jQuery(function ($) {
    var app = app || {};
    var context = JSON.parse(dnn.getVar("PowerBISettings_Context"));

    function SettingsModel(settings) {
        var that = this;

        this.SettingsId = ko.observable(settings.SettingsId);
        this.SettingsGroupId = ko.observable(settings.SettingsGroupId);
        this.SettingsGroupName = ko.observable(settings.SettingsGroupName);
    }

    app.Settings = function () {
        var that = this;
        var service = {
            path: "PowerBI/UI",
            framework: $.ServicesFramework(context.ModuleId),
            controller: "GroupSettings"
        };
        service.baseUrl = service.framework.getServiceRoot(service.path);

        this.AvailableSettingsGroups = ko.observableArray();

        this.SelectedSettingsGroup = ko.observable();

        this.GetSettingsGroups = function () {
            service.controller = "GroupSettings";
            Common.Call("GET",
                "GetSettingsGroups",
                service,
                null,
                function (response) {
                    if (response.Success) {
                        console.log("success");
                        that.AvailableSettingsGroups.removeAll()
                        ko.utils.arrayForEach(response.Data,
                            function (item) {
                                that.AvailableSettingsGroups.push(new SettingsModel(item));
                            });
                        that.AvailableSettingsGroups.valueHasMutated();
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

        this.GetModuleSettings = function () {
            service.controller = "GroupSettings";
            var params = {
                tabModuleId: context.TabModuleId
            };

            Common.Call("GET",
                "GetModuleSettings",
                service,
                params,
                function (response) {
                    if (response.Success) {
                        that.SelectedSettingsGroup(response.ModuleSettings);
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

        this.SaveSettingByModule = function () {
            service.controller = "GroupSettings";

            var params = {
                settingsId: that.SelectedSettingsGroup(),
                tabModuleId: context.TabModuleId
            };

            Common.Call("GET",
                "SaveModuleSettings",
                service,
                params,
                function (response) {
                    if (response.Success) {
                        console.log("success");
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
    };
    var viewmodel = new app.Settings();

    viewmodel.GetSettingsGroups();
    viewmodel.GetModuleSettings();

    ko.applyBindings(viewmodel, $("#ListViewGroupSettings")[0]);
});   