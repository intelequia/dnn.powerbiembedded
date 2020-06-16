jQuery(function ($) {
    var app = app || {};
    
    var context = JSON.parse(dnn.getVar("PowerBISettings_Context"));

    app.View = function (model) {
        var that = this;
         
        var service = {
            path: "PowerBI/UI",
            framework: $.ServicesFramework(context.ModuleId),
            controller: "Admin"
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);

        this.powerBiObjects = ko.observableArray();
        this.reports = ko.computed(function () {
            return ko.utils.arrayFilter(that.powerBiObjects(), function(obj) {
                return obj.Type() == constants.REPORT_TYPE;
            }); 
        });
        this.dashboards = ko.computed(function () {
            return ko.utils.arrayFilter(that.powerBiObjects(), function(obj) {
                return obj.Type() == constants.DASHBOARD_TYPE;
            });
        });

        this.selectedPowerBiObject = ko.observable();
        this.shouldShowGridPermissions = ko.computed(function () {
            return that.selectedPowerBiObject() != undefined && that.selectedPowerBiObject() != null;
        });

        this.usersAndRoles = ko.observableArray();
        this.users = ko.computed(function () {
            return ko.utils.arrayFilter(that.usersAndRoles(), function (ug) {
                return ug.Type() == "user";
            });
        });
        this.roles = ko.computed(function () {
            return ko.utils.arrayFilter(that.usersAndRoles(), function (ug) {
                return ug.Type() == "role";
            });
        });

        this.pbiUsersPermissions = ko.computed(function () {
            return that.selectedPowerBiObject() ? that.selectedPowerBiObject().UsersPermissions() : null;
        });
        this.pbiRolesPermissions = ko.computed(function () {
            return that.selectedPowerBiObject() ? that.selectedPowerBiObject().RolesPermissions() : null;
        });

        this.PowerBiObjectModel = function () {
            var self = this;
            this.Id = ko.observable();
            this.Name = ko.observable();
            this.Type = ko.observable();
            this.Permissions = ko.observableArray();
            this.UsersPermissions = ko.computed(function () {
                return ko.utils.arrayFilter(self.Permissions(), function (sec) {
                    return sec.UserId() != undefined && sec.UserId() != null;
                });
            });
            this.RolesPermissions = ko.computed(function () {
                return ko.utils.arrayFilter(self.Permissions(), function (sec) {
                    return sec.RoleId() != undefined && sec.RoleId() != null;
                });
            });

            this.load = function (data) {
                this.Id(data.Id);
                this.Name(data.Name);
                this.Type(data.PowerBiType);
                for (var i = 0; i < data.Permissions.length; i++) {
                    var permission = new that.PermissionModel();
                    permission.load(data.Permissions[i], data.Id);
                    this.Permissions.push(permission);
                }
            }

            this.selectPowerBiObject = function (report) {
                that.selectedPowerBiObject(report);

                return true;
            }

            this.getJSON = function () {
                var result =
                {
                    Id: this.Id(),
                    Name: this.Name(),
                    Type: this.Type(),
                    Permissions: []
                };
                this.Permissions().map((p) => { result.Permissions.push(p.getJSON()); });

                return result;
            }
        }
        this.UserAndRoleModel = function () {
            this.Id = ko.observable();
            this.DisplayName = ko.observable();
            this.Type = ko.observable();

            this.loadUser = function (data) {
                this.Id(data.UserId);
                this.DisplayName(data.DisplayName);
                this.Type("user");
            }

            this.loadRole = function (data) {
                this.Id(data.RoleId);
                this.DisplayName(data.RoleName);
                this.Type("role");
            }
            this.onClick = function (data) {
                console.log(data);
            }
        }
        this.GetAllUsers = function () {
            var params = {}

            Common.Call("GET", "GetPortalUsersAndGroups", service, params,
                function (response) {
                    if (response.Success) {
                        that.usersAndRoles.removeAll();

                        response.Data.users.forEach(function (entry) {
                            var item = new that.UserAndRoleModel(that);
                            item.loadUser(entry);
                            that.usersAndRoles.push(item);
                        });
                        response.Data.roles.forEach(function (entry) {
                            var item = new that.UserAndRoleModel(that);
                            item.loadRole(entry);
                            that.usersAndRoles.push(item);
                        });

                        $('#UserGroupSelector').select2();
                    } else {
                        toastr.error("Error");
                        //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                    }
                },
                function (error) {
                    toastr.error("Error");
                    //Notify.Error(JSON.parse(error.responseText).Message, error.status);
                },
                function () {
                    //that.isBusy(false);
                });
        }
        this.PermissionModel = function () {
            var self = this;
            this.Id = ko.observable();
            this.PbiObjectId = ko.observable();
            this.PermissionId = ko.observable(1); // 1 -> View; 2 -> Edit
            this.AllowAccess = ko.observable(true);
            this.UserId = ko.observable();
            this.UserDisplayName = ko.observable();
            this.RoleName = ko.observable();
            this.RoleId = ko.observable();

            this.load = function (data, powerBiObjectId) {
                this.Id(data.ID);
                this.PbiObjectId(powerBiObjectId);
                this.PermissionId(data.PermissionId);
                this.AllowAccess(data.AllowAccess);
                this.UserId(data.UserID);
                this.RoleId(data.RoleID);
                this.UserDisplayName(data.UserDisplayName);
                this.RoleName(data.RoleName);
            }

            this.onClickAllowDeny = function () {
                this.AllowAccess(!this.AllowAccess());
            }

            this.onClickDelete = function () {
                that.selectedPowerBiObject().Permissions.remove(function (permission) {
                    if (self.Id())
                        return permission.Id() == self.Id();
                    else
                        return permission.PbiObjectId() == self.PbiObjectId() && permission.UserId() == self.UserId() && permission.RoleId() == self.RoleId();
                });
            }

            this.getJSON = function () {
                var result =
                {
                    Id: this.Id(),
                    PbiObjectId: this.PbiObjectId(),
                    PermissionId: this.PermissionId(),
                    AllowAccess: this.AllowAccess(),
                    UserId: this.UserId(),
                    RoleId: this.RoleId()
                };
                return result;
            }
        }
        this.AddUser = function () {
            var selectedValue = $('#UserGroupSelector').find(':selected')[0];
            var isUser = selectedValue.classList.contains("option-user");
            var alreadyInTheList = ko.utils.arrayFirst(that.selectedPowerBiObject().Permissions(), function (userOrRole) {
                return (isUser && userOrRole.UserId() == selectedValue.value) || (!isUser && userOrRole.RoleId() == selectedValue.value);
            });
            if (selectedValue && !alreadyInTheList) {
                var data = {
                    PbiObjectId: that.selectedPowerBiObject().Id(),
                    PermissionId: 1, 
                    AllowAccess: true,
                };
                if (isUser) {
                    var match = ko.utils.arrayFirst(that.users(), function (user) {
                        return user.Id() == selectedValue.value;
                    });
                    // Add user to the Users table
                    data.UserID = match.Id();
                    data.UserDisplayName = match.DisplayName();
                }
                else {  // It's not a user; it's a role
                    var match = ko.utils.arrayFirst(that.roles(), function (role) {
                        return role.Id() == selectedValue.value;
                    });
                    // Add role to the Roles table
                    data.RoleID = match.Id();
                    data.RoleName = match.DisplayName();
                }
                var permission = new that.PermissionModel();
                permission.load(data, that.selectedPowerBiObject().Id());
                that.selectedPowerBiObject().Permissions.push(permission);
            }
        }

        this.SavePermissions = function () {
            var params = {
                powerBiObjects: that.powerBiObjects().map((obj) => obj.getJSON())
            };

            Common.Call("POST", "SavePowerBiObjectsPermissions", service, params,
                function (response) {
                    if (response.Success) {
                        console.log("Permissions saved successfully");
                    } else {
                        console.error("Error retrieving PowerBI objects");
                    }
                },
                function (error) {
                    console.error("Error");
                },
                function () {
                    //that.isBusy(false);
                });
        }
        this.GetAvailableReports = function (id) {
            var params = {
                moduleId: context.ModuleId, 
                tabModuleId: context.TabModuleId
            }; 
             
            Common.Call("GET", "GetPowerBiObjectList", service, params,
                function (response) {
                    if (response.Success) {
                        that.powerBiObjects.removeAll();
                        response.PowebBiObjects.forEach(function (entry) {
                            var item = new that.PowerBiObjectModel(that);
                            item.load(entry);
                            that.powerBiObjects.push(item);
                        });
                        if (that.powerBiObjects().length > 0) {
                            that.selectedPowerBiObject(that.powerBiObjects()[0]);
                            $(`#${that.powerBiObjects()[0].Id()}`).attr("checked", true)
                        }
                    } else {
                        console.error("Error retrieving PowerBI objects");
                    }
                },
                function (error) {
                    console.error("Error");
                },
                function () {
                    //that.isBusy(false);
                });
        }

        this.Init = function () {
            that.GetAvailableReports(that);
            that.GetAllUsers();
            //that.GetPowerBiPermissions();
        }
    }

    // Initialize
    var viewmodel = new app.View();
    viewmodel.Init();

    ko.applyBindings(viewmodel, $("#ListViewSettings")[0]);
});