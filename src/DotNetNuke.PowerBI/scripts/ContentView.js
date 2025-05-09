﻿jQuery(function ($) {
    var app = app || {};
    var contexts = [];
    if (!dnn.vars)
        dnn.getVars();
    for (var i in dnn.vars) {
        if (i.startsWith("ViewContext_")) {
            contexts.push(JSON.parse(dnn.getVar(i)));
        }
    }

    function SubscriptionModel(p, id, portalId, reportId, groupId, name, startDate, endDate, repeatPeriod, repeatTime, timeZone, emailSubject, message, reportPages, enabled, users, roles, moduleId) {
        var that = this;
        var parent = p;
        this.id = ko.observable(id);
        this.portalId = ko.observable(portalId);
        this.reportId = ko.observable(reportId);
        this.groupId = ko.observable(groupId);
        this.moduleId = ko.observable(moduleId);
        this.name = ko.observable(name).extend({ required: true });
        this.startDate = ko.observable(startDate).extend({ required: true });
        this.endDate = ko.observable(endDate).extend({ required: true });
        this.repeatPeriod = ko.observable(repeatPeriod).extend({ required: true });
        this.repeatTime = ko.observable(repeatTime).extend({ required: true });
        this.timeZone = ko.observable(timeZone).extend({ required: true });
        this.emailSubject = ko.observable(emailSubject).extend({ required: true });
        this.message = ko.observable(message).extend({ required: true });
        this.enabled = ko.observable(enabled)
        this.users = ko.observableArray(users);
        this.roles = ko.observableArray(roles);
        this.reportPages = ko.observableArray(reportPages);
        this.pagesArray = ko.observableArray(parent.pagesArray().slice());
        this.editUserSearchQuery = ko.observable('');
        this.editRoleSearchQuery = ko.observable('');
        this.addedUsers = ko.observableArray(users.slice());
        this.addedRoles = ko.observableArray(roles.slice());
        this.addedPages = ko.observableArray(reportPages.slice());
        this.repeatTimeFormatted = ko.computed(function () {
            var time = that.repeatTime();
            var hours = parseInt(time.substring(0, 2), 10);
            var minutes = time.substring(3, 5);
            var ampm = hours >= 12 ? 'PM' : 'AM';

            // Convert hours from 24-hour to 12-hour format 
            if (hours > 12) {
                hours -= 12;
            }

            // Add leading zero for single-digit hours
            if (hours < 10) {
                hours = '0' + hours;
            }

            return hours + ':' + minutes + ' ' + ampm;
        });

        // Error group for editing a subscription.
        this.editSubscriptionErrors = ko.validation.group({
            name: this.name,
            startDate: this.startDate,
            endDate: this.endDate,
            repeatPeriod: this.repeatPeriod,
            repeatTime: this.repeatTime,
            timeZone: this.timeZone,
            emailSubject: this.emailSubject,
            message: this.message,
        });

        this.searchTimeout = null;
        this.availableUsers = ko.observableArray([]);
        this.editUserSearchQuery.subscribe(function (newValue) {
            var users = [];
            query = newValue.toLowerCase();
            if (query == null || query == "") {
                that.availableUsers(users)
                return;
            }
            let params = {
                searchName: query,
                workspaceId: that.groupId(),
                reportId: that.reportId(),
            }
            if (that.searchTimeout)
                clearTimeout(that.searchTimeout);
            that.searchTimeout = setTimeout(function () {
                that.searchTimeout = null;
                Common.Call("GET", "SearchUsers", parent.subscriptionsService, params,
                    function (data) {
                        if (data.Success) {
                            users = data.Data.filter(user => {
                                return !that.addedUsers().some(selectedUser => selectedUser.UserID === user.UserID);
                            });
                            that.availableUsers(users)
                        }
                    },
                    function (error) {
                        console.log(error);
                    },
                    function () {
                    });
            }, 500);
        });

        this.availableRoles = ko.observableArray([]);
        this.editRoleSearchQuery.subscribe(function (newValue) {
            var roles = [];
            query = newValue.toLowerCase();
            if (query == null || query == "") {
                that.availableRoles(roles)
                return;
            }
            let params = {
                searchName: query,
                workspaceId: that.groupId(),
                reportId: that.reportId(),
            }
            if (that.searchTimeout)
                clearTimeout(that.searchTimeout);
            that.searchTimeout = setTimeout(function () {
                Common.Call("GET", "SearchRoles", parent.subscriptionsService, params,
                    function (data) {
                        if (data.Success) {
                            roles = data.Data.filter(role => {
                                return !that.addedRoles().some(selectedRole => selectedRole.RoleID === role.RoleID);
                            });
                            that.availableRoles(roles)
                        }
                    },
                    function (error) {
                        console.log(error);
                    },
                    function () {
                    });
            }, 500);
        });
        this.availablePages = ko.computed(function () {
            return that.pagesArray().filter(function (page) {
                return !that.addedPages().some(selectedPage => selectedPage.name === page.name)
            });
        });

        this.editing = ko.observable(false);
        this.startEditing = function () {
            that.editing(true);
            parent.editing(true);            setTimeout(function () {
                $('input[type=checkbox]').dnnCheckbox(); //workaround to display the checkboxes
            }, 0);
        };

        this.addUserToSubscription = function (user) {
            that.addedUsers.push(user);
            that.availableUsers.remove(user);
        };
        this.addRoleToSubscription = function (role) {
            that.addedRoles.push(role);
            that.availableRoles.remove(role);
        };
        this.addPageToSubscription = function (page) {
            that.addedPages.push(page);
        }
        this.removeUserFromSubscription = function (user) {
            that.addedUsers.remove(user);
            that.availableUsers.push(user);
        };
        this.removeRoleFromSubscription = function (role) {
            that.addedRoles.remove(role);
            that.availableRoles.push(role);
        };
        this.removePageFromSubscription = function (page) {
            that.addedPages.remove(page);
        }
        // Method to cancel editing
        this.cancelEditing = function () {

            that.name(name);
            that.startDate(startDate);
            that.endDate(endDate);
            that.repeatPeriod(repeatPeriod);
            that.repeatTime(repeatTime);
            that.timeZone(timeZone);
            that.emailSubject(emailSubject);
            that.message(message);
            that.enabled(enabled);
            that.addedUsers(users.slice());
            that.addedRoles(roles.slice());
            that.addedPages(reportPages.slice());
            
            // Set editing state back to false
            that.editing(false);
            parent.editing(false);
        };

        this.editSubscription = function () {
            that.startEditing();
            parent.selectedSubscription(that);
        };

        this.deleteSubscription = function () {
            let params = {
                Id: that.id(),
                ReportId: that.reportId(),
                groupId: that.groupId(),
            };
            var confirmed = confirm("Are you sure you want to delete " + that.name() + " subscription?");
            if (confirmed) {
                Common.Call("POST", "DeleteSubscription", parent.subscriptionsService, params,
                    function (data) {
                        if (data.Success) {
                            parent.createSubscriptionList();
                        }
                        else {
                            alert(data.ErrorMessage);
                        }
                    },
                    function (error) {
                        console.log(error);
                    },
                    function () {
                    });
            }
        };
    }
    function BookmarkModel(p, id, name, displayName, state, reportId) {
        var that = this;
        var parent = p;
        this.id = ko.observable(id);
        this.name = ko.observable(name);
        this.displayName = ko.observable(displayName);
        this.state = ko.observable(state);
        this.reportId = ko.observable(reportId);

        this.bookmarkClass = ko.computed(function () {
            return "" + (parent.selectedBookmark() !== null && parent.selectedBookmark().id() === that.id() ? " selected" : "");
        });
    }

    function ExportItemModel(id, createdDateTime, lastActionDateTime, reportId, reportName, status, percentComplete, resourceLocation, resourceFileExtension, expirationTime) {
        var that = this;
        this.id = ko.observable(id);
        this.createdDateTime = ko.observable(createdDateTime);
        this.lastActionDateTime = ko.observable(lastActionDateTime);
        this.reportId = ko.observable(reportId);
        this.reportName = ko.observable(reportName);
        this.status = ko.observable(status);
        this.percentComplete = ko.observable(percentComplete);
        this.resourceLocation = ko.observable(resourceLocation);
        this.resourceFileExtension = ko.observable(resourceFileExtension);
        this.expirationTime = ko.observable(expirationTime);
        this.interval = null;
        this.exportStatusClass = ko.computed(function () {
            switch (that.status()) {
                case "Succeeded": return "succeeded";
                case "Failed": return "failed";
                default: return "exporting";
            }
        });
    }

    app.View = function (c) {
        var that = this;
        var context = c;

        //Bookmarks
        this.bookmarksArray = ko.observableArray([]);
        this.selectedBookmark = ko.observable(null);
        this.newBookmarkName = ko.observable('').extend({ required: true });
        this.bookmarksService = {
            path: "PowerBI/Services",
            controller: "Bookmarks",
            framework: $.ServicesFramework(context.ModuleId)
        }
        this.bookmarksService.baseUrl = that.bookmarksService.framework.getServiceRoot(that.bookmarksService.path);

        // Subscriptions
        this.subscriptionsArray = ko.observableArray([]);
        this.pagesArray = ko.observableArray(context.ReportPages.value.slice());
        this.selectedSubscription = ko.observable();


        this.editing = ko.observable(false);

        this.subscriptionsService = {
            path: "PowerBI/Services",
            controller: "Subscription",
            framework: $.ServicesFramework(context.ModuleId)
        }
        this.subscriptionsService.baseUrl = that.subscriptionsService.framework.getServiceRoot(that.subscriptionsService.path);

        this.exportsService = {
            path: "PowerBI/Services",
            controller: "Exports",
            framework: $.ServicesFramework(context.ModuleId)
        }
        this.exportsService.baseUrl = that.exportsService.framework.getServiceRoot(that.exportsService.path);


        this.searchTimeout = null;

        this.createSubscriptionList = function () {
            let params = {
                workspaceId: that.report.config.groupId,
                reportId: context.Id
            }
            Common.Call("GET", "GetSubscriptions", that.subscriptionsService, params,
                function (data) {
                    if (data.Success) {
                        var subscriptions = [];

                        data.Data.forEach(subscription => {
                            var reportPageNames = subscription.ReportPages.split(',');
                            var reportPages = reportPageNames.map(function (pageName) {
                                var correspondingPage = that.pagesArray().find(function (page) {
                                    return page.name === pageName;
                                });

                                return correspondingPage ? correspondingPage : null;
                            })
                                .filter(function (page) {
                                    return page !== null; // Remove null values (non-existing pages)
                                });

                            var b = new SubscriptionModel(
                                that,
                                subscription.Id,
                                subscription.PortalId,
                                subscription.ReportId,
                                subscription.GroupId,
                                subscription.Name,
                                new Date(subscription.StartDate.split('T')[0]).toISOString().split('T')[0],
                                new Date(subscription.EndDate.split('T')[0]).toISOString().split('T')[0],
                                subscription.RepeatPeriod,
                                subscription.RepeatTime,
                                subscription.TimeZone,
                                subscription.EmailSubject,
                                subscription.Message,
                                reportPages,
                                subscription.Enabled,
                                JSON.parse(subscription.Users),
                                JSON.parse(subscription.Roles),
                                context.PortalId,
                                context.ModuleId,
                            );
                            subscriptions.push(b);
                        });
                        that.subscriptionsArray(subscriptions);
                    }
                    else {
                        that.subscriptionsArray([]);
                    }
                },
                function (error) {
                    console.log(error);
                },
                function () {
                });
        }

        this.createBookmarksList = function () {
            let params = {
                reportId: context.Id
            }

            Common.Call("GET", "GetBookmarks", that.bookmarksService, params,
                function (data) {
                    if (data.Success) {
                        var bookmarks = [];
                        data.Data.forEach(bookmark => {
                            var b = new BookmarkModel(that, bookmark.Id, "bookmark_" + bookmark.Id, bookmark.DisplayName, bookmark.State, context.Id);
                            bookmarks.push(b);
                        });
                        that.bookmarksArray(bookmarks);
                        // Set first bookmark active
                        if (bookmarks.length > 0) {
                            // Apply first bookmark state
                            that.onBookmarkClicked(bookmarks[0]);
                        }
                    }
                },
                function (error) {
                    console.log("error");
                },
                function () {
                });
        }
        // set validation group
        this.errors = ko.validation.group(that);

        // Get models. models contains enums that can be used.
        this.models = window['powerbi-client'].models;
        this.isMobile = window.innerWidth < 425 || window.innerHeight < 425;
        this.isLandscape = window.innerWidth > window.innerHeight;
        this.defaultHeight = context.Height.toString();
        this.pageName = context.PageName;
        this.overrideVisualHeaderVisibility = context.OverrideVisualHeaderVisibility;
        this.overrideFilterPaneVisibility = context.OverrideFilterPaneVisibility;
        this.applicationInsightsEnabled = context.ApplicationInsightsEnabled;
        this.config = {
            type: context.ContentType,
            tokenType: that.models.TokenType.Embed,
            accessToken: context.Token,
            embedUrl: context.EmbedUrl,
            id: context.Id,
            permissions: context.CanEdit ? that.models.Permissions.All : that.models.Permissions.Read,
            viewMode: that.models.ViewMode.View,
            settings: {
                navContentPaneEnabled: context.NavPaneVisible,
                localeSettings: {
                    language: context.Locale,
                    formatLocale: context.Locale
                },
            },
            pageName: context.PageName
        };

        if (this.overrideVisualHeaderVisibility) {
            that.config.settings.visualSettings = {
                visualHeaders: [
                    {
                        settings: {
                            visible: context.VisualHeaderVisible
                        }
                    }
                ]
            }
        }

        //override filter pane
        if (context.overrideFilterPaneVisibility) {
            that.config.settings.filterPaneEnabled = context.FilterPaneVisible && !that.isMobile;
        }

        // Get a reference to the embedded report HTML element
        this.reportContainer = $('#embedContainer_' + context.ModuleId)[0];

        if (!that.isMobile && that.isLandscape) {
        var h = that.defaultHeight !== '' ? that.defaultHeight : window.innerHeight - 50 - 20 - 39 - 20 - 10;

            $('#embedContainer_' + context.ModuleId).css("height", h + "px");
        }
    
        if (this.overrideFilterPaneVisibility) {
            that.config.settings.panes = {
                filters: {
                    visible: context.FilterPaneVisible && !that.isMobile, // hide filter pane
                }
            }
        }

        // Embed the report and display it within the div container.
        this.report = powerbi.load(that.reportContainer, that.config);

        //Getreport bookmarks
        this.report.bookmarksManager.getBookmarks()
            .then(function (bookmarks) {
                // Create bookmarks list from the existing report bookmarks 
                that.updateBookmarksList(bookmarks);
            });
        this.trackEvent = function (eventName, data) {
            if (that.applicationInsightsEnabled && typeof appInsights !== "undefined") {
                let userId = "-1";
                if (dnn.getVar("dnn_current_userid") !== undefined) {
                    userId = dnn.getVar("dnn_current_userid");
                    appInsights.setAuthenticatedUserContext(userId);
                }
                appInsights.trackEvent({
                    name: "PowerBI_" + eventName,
                    properties: {
                        reportId: that.report.config.id,
                        groupId: that.report.config.groupId,
                        userId: userId,
                        url: document.URL,
                        data: eventName === "pageChanged" && data.newPage ?
                            {
                                report: {
                                    id: that.report.config.id,
                                    displayName: ""
                                },
                                page: {
                                    name: data.newPage.name,
                                    displayName: data.newPage.displayName
                                }
                            }
                            : data
                    }
                });
            }
        }

        this.report.allowedEvents.forEach(e => {
            that.report.off(e);
            that.report.on(e, function (event) {
                that.trackEvent(e, event.detail);
            });
        });

        this.setCustomLayout = function () {
            that.config.settings.layoutType = that.models.LayoutType.Custom;
            that.config.settings.customLayout = {
                displayOption: that.models.DisplayOption.FitToWidth,
                width: "100%",
                height: "100%"
            }
            that.config.settings.panes = {
                pageNavigation: {
                    visible: false
                },
            }
        }

        this.report.on("loaded", async function () {
            if (that.isMobile && that.config.settings.layoutType != that.models.LayoutType.MobilePortrait) {
                var page = await that.report.getActivePage()
                const hasLayout = await page.hasLayout(that.models.LayoutType.MobilePortrait);
                if (hasLayout) {
                    that.config.settings.layoutType = that.models.LayoutType.MobilePortrait;
                    await powerbi.reset(that.reportContainer);
                    that.report = powerbi.embed(that.reportContainer, that.config);
                } else {
                    if (that.config.settings.layoutType != that.models.LayoutType.Custom) {
                        that.setCustomLayout();
                        await powerbi.reset(that.reportContainer);
                        that.report = powerbi.embed(that.reportContainer, that.config);
                    }
                }
            }
            else {
                await that.report.render();
            }
        })

        this.createBookmarksList = function () {
            let params = {
                reportId: context.Id
            }

            Common.Call("GET", "GetBookmarks", that.bookmarksService, params,
                function (data) {
                    if (data.Success) {
                        var bookmarks = [];
                        data.Data.forEach(bookmark => {
                            var b = new BookmarkModel(that, bookmark.Id, "bookmark_" + bookmark.Id, bookmark.DisplayName, bookmark.State, context.Id);
                            bookmarks.push(b);
                        });
                        that.bookmarksArray(bookmarks);
                        // Set first bookmark active
                        if (bookmarks.length > 0) {
                            // Apply first bookmark state
                            that.onBookmarkClicked(bookmarks[0]);
                        }
                    }
                },
                function (error) {
                    console.log("error");
                },
                function () {
                });
        }

        this.updateBookmarksList = function (bookmarks) {
            // Set bookmarks array to the report's fetched bookmarks
            that.bookmarksArray(bookmarks);
            // Set first bookmark active
            if (bookmarks.length > 0) {
                that.selectedBookmark(bookmarks[0].name);

                // Apply first bookmark state
                that.onBookmarkClicked(bookmarks[0]);
            }
        }

        this.onBookmarkClicked = function (element) {
            // Set the clicked bookmark as active
            that.selectedBookmark(element);

            // Apply the bookmark state
            that.report.bookmarksManager.applyState(element.state());
        }

        // Capture new bookmark of the current state and update the bookmarks list
        this.onBookmarkCaptureClicked = function () {
            // check if the new bookmark name is valid
            if (that.errors().length > 0) {
                that.errors.showAllMessages(true);
            } else {
                // Capture the report's current state
                that.report.bookmarksManager.capture()
                    .then(function (capturedBookmark) {
                        // Build bookmark element
                        let bookmark = {
                            name: "bookmark_" + that.newBookmarkName(),
                            displayName: that.newBookmarkName(),
                            state: capturedBookmark.state,
                            reportId: context.Id
                        }
                        Common.Call("POST", "SaveBookmark", that.bookmarksService, bookmark,
                            function (data) {
                                if (data.Success) {
                                    that.newBookmarkName('');
                                    that.errors.showAllMessages(false);
                                    var b = new BookmarkModel(that, data.Data, "bookmark_" + data.Data, bookmark.displayName, bookmark.state, bookmark.reportId);
                                    that.bookmarksArray.push(b);
                                    that.selectedBookmark(b);
                                }
                            },
                            function (error) {
                                console.log("error");
                            },
                            function () {
                            });
                    });
            }
        }

        this.deleteBookmark = function (element) {
            let bookmark = {
                id: element.id()
            };

            Common.Call("POST", "DeleteBookmark", that.bookmarksService, bookmark,
                function (data) {
                    if (data.Success) {
                        that.bookmarksArray.remove(element);
                    }
                },
                function (error) {
                    console.log("error");
                },
                function () {
                }
            );
        }

        // Subscriptions
        this.openLateralTab = function () {
            $(".pbi-overlay").css("display", "block");
            $(".lateral-tab").css("width", "450px");
            that.createSubscriptionList();
        }

        this.closeLateralTab = function () {
            $(".lateral-tab").css("width", "0");
            $(".pbi-overlay").css("display", "none");
        }


        this.cancelEditing = function (subscription) {
            if (subscription.id() == -1) {
                that.subscriptionsArray.splice(that.subscriptionsArray.indexOf(subscription), 1);
            }
            else {
                subscription.cancelEditing();
            }
            that.editing(false);
            that.selectedSubscription(null);
        };
        this.addNewSubscription = function () {
            var b = new SubscriptionModel(
                that,
                -1,
                context.PortalId,
                that.report.config.id,
                that.report.config.groupId,
                "",
                "",
                "",
                "",
                "",
                context.PreferredTimeZone.Id,
                "",
                "",
                [],
                false,
                [],
                [],
                context.ModuleId,
            );
            that.subscriptionsArray.push(b);
            b.editSubscription();
        }

        this.saveEditedSubscription = function (subscription) {

            if (subscription.editSubscriptionErrors().length > 0) {
                subscription.editSubscriptionErrors.showAllMessages(true);
            }
            else {
                let serializedUsers = subscription.addedUsers().map(user => user.UserID).join(",");
                let serializedRoles = subscription.addedRoles().map(role => role.RoleID).join(",");
                let serializedReportPages = subscription.addedPages().map(page => page.name).join(",");
                let params = {
                    Id: subscription.id(),
                    ReportId: subscription.reportId(),
                    GroupId: subscription.groupId(),
                    ModuleId: subscription.moduleId(),
                    PortalId: subscription.portalId(),
                    Name: subscription.name(),
                    StartDate: subscription.startDate(),
                    EndDate: subscription.endDate(),
                    RepeatPeriod: subscription.repeatPeriod(),
                    RepeatTime: subscription.repeatTime(),
                    TimeZone: subscription.timeZone(),
                    EmailSubject: subscription.emailSubject(),
                    Message: subscription.message(),
                    ReportPages: serializedReportPages,
                    Enabled: subscription.enabled(),
                    Users: serializedUsers,
                    Roles: serializedRoles,
                };
                Common.Call("POST", "EditSubscription", that.subscriptionsService, params,
                    function (data) {
                        if (data.Success) {
                            that.createSubscriptionList();
                            that.cancelEditing(subscription);
                            subscription.editing(false);
                            subscription.editSubscriptionErrors.showAllMessages(false);
                        }
                    },
                    function (error) {
                        console.log(error);
                    },
                    function () {
                    });
            }
        };
        this.getParameterByName = function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }

        this.pbirefresh = function () {
            that.report.refresh();
        }
        this.pbifullscreen = function () {
            that.report.fullscreen();
        }

        this.pbipages = async function () {
            const pages = await that.report.getPages();
            return pages;
        }

        this.pbiedit =  async function () {
            await that.report.switchMode("edit");
            const newSettings = {
                panes: {
                    filters: {
                        visible: true
                    },
                    pageNavigation: {
                        visible: true
                    },
                    visualizations: {
                        visible: !context.HideVisualizationData
                    },
                    fields: {
                        visible: !context.HideVisualizationData
                    },
                }
            };
            await that.report.updateSettings(newSettings);
        };

        this.pbidownload = async function () {
            var options = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/octet-stream',
                    'TabId': that.exportsService.framework.getTabId(),
                    'ModuleId': that.exportsService.framework.getModuleId()
                }
            }

            var afValue = that.exportsService.framework.getAntiForgeryValue();
            if (afValue) {
                const additionalHeaders = {
                    'RequestVerificationToken': afValue,
                };
                options.headers = {
                    ...options.headers,
                    ...additionalHeaders
                }
            }

            fetch(that.exportsService.baseUrl + '/' + that.exportsService.controller + '/Download?sid=' + context.WorkspaceId + '&rid=' + context.Id, options)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Error downloading the file');
                    }
                    // Obtener el nombre del fichero desde el encabezado Content-Disposition
                    const disposition = response.headers.get('Content-Disposition');
                    let filename = 'downloaded_file';
                    if (disposition && disposition.indexOf('filename=') !== -1) {
                        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        const matches = filenameRegex.exec(disposition);
                        if (matches != null && matches[1]) {
                            filename = matches[1].replace(/['"]/g, '');
                        }
                    }
                    return response.blob().then(blob => ({ blob, filename }));
                })
                .then(({ blob, filename }) => {
                    // Crear una URL temporal para el blob
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.style.display = 'none';
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                    // Limpiar
                    window.URL.revokeObjectURL(url);
                    a.remove();
                })
                .catch(error => {
                    console.error('Error downloading the file:', error);
                    alert('There was an error downloading the file.');
                });
        };
        this.showQueue = async function () {
            $(".powerbiContentView .mnuQueue").css("left", $(".powerbiContentView .exportQueue").position().left);
            $(".powerbiContentView .mnuQueue").css("top", $(".powerbiContentView .exportQueue").height() + 1);
            $(".powerbiContentView .mnuQueue").toggle();
        };
        this.cancelAllExports = async function () {
            // Cancel all exports
            for (var i = 0; i < that.exportsQueue().length; i++) {
                var item = that.exportsQueue()[i];
                if (item.interval) {
                    clearInterval(item.interval);
                    item.interval = null;

                }
            }                        
            that.exportsQueue.removeAll();
            that.exportsQueue.valueHasMutated();
            $(".powerbiContentView .mnuQueue").toggle();
        };
        this.pbiexport = async function () {
            $(".powerbiContentView .mnuexport").css("left", $(".powerbiContentView .export").position().left);
            $(".powerbiContentView .mnuexport").css("top", $(".powerbiContentView .export").height() + 1);
            $(".powerbiContentView .mnuexport").toggle();
        };
        this.pbiexportExcel = async function () {
            $(".powerbiContentView .mnuexport").hide();            
            that.exportToFile('xlsx');
        };
        this.pbiexportPowerpoint = async function () {            
            $(".powerbiContentView .mnuexport").hide();
            that.exportToFile('pptx');
        };
        this.pbiexportPDF = async function () {            
            $(".powerbiContentView .mnuexport").hide();
            that.exportToFile('pdf');
        };
        this.exportsQueue = ko.observableArray([]);
        this.exportsQueueStatus = ko.computed(function () {
            var status = "succeeded";
            that.exportsQueue().forEach(item => {
                if (item.status() == "Failed") {
                    status = "failed";
                }
                else if (item.status() == "NotStarted" || item.status() == "Undefined" || item.status() == "Running" ) {
                    status = "exporting";
                }
            });
            return status;
        });
        this.exportsQueue.subscribe(function () {
            that.exportsQueue().forEach(item => {
                if (!item.interval && item.percentComplete() == 0) {
                    item.interval = setInterval(() => {
                        var options = {
                            method: 'GET',
                            headers: {
                                'Content-Type': 'application/octet-stream',
                                'TabId': that.exportsService.framework.getTabId(),
                                'ModuleId': that.exportsService.framework.getModuleId()
                            }
                        }

                        var afValue = that.exportsService.framework.getAntiForgeryValue();
                        if (afValue) {
                            const additionalHeaders = {
                                'RequestVerificationToken': afValue,
                            };
                            options.headers = {
                                ...options.headers,
                                ...additionalHeaders
                            }
                        }
                        let params = {
                            sid: context.WorkspaceId,
                            rid: context.Id,
                            exportId: item.id()
                        }
                        Common.CallWithOptions("GET", "ExportStatus", that.exportsService, params, options,
                            function (data) {
                                if (data) {

                                    item.percentComplete(data.percentComplete);
                                    item.status(data.status);
                                    item.lastActionDateTime(data.lastActionDateTime);
                                    item.resourceLocation(data.resourceLocation);
                                    item.resourceFileExtension(data.ResourceFileExtension);
                                    item.expirationTime(data.expirationTime);
                                    if (item.percentComplete() >= 100) {
                                        that.downloadExportedFile(item);
                                        clearInterval(item.interval);
                                        item.interval = null;
                                    }
                                    that.exportsQueue.valueHasMutated();
                                }
                                else {
                                    alert("There was an error exporting the file.");
                                }
                            },
                            function (error) {
                                console.log(error);
                            },
                            function () {
                            }
                        );
                    }, 5000);
                }
            });
        });

        this.downloadExportedFile = function (item) {
            var options = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/octet-stream',
                    'TabId': that.exportsService.framework.getTabId(),
                    'ModuleId': that.exportsService.framework.getModuleId()
                }
            }

            var afValue = that.exportsService.framework.getAntiForgeryValue();
            if (afValue) {
                const additionalHeaders = {
                    'RequestVerificationToken': afValue,
                };
                options.headers = {
                    ...options.headers,
                    ...additionalHeaders
                }
            }

            fetch(that.exportsService.baseUrl + '/' + that.exportsService.controller + '/GetExportedFile?sid=' + context.WorkspaceId + '&rid=' + context.Id + '&exportid=' + item.id() + '&resourceFileExtension=' + item.resourceFileExtension(), options)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Error downloading the file');
                    }
                    // Obtener el nombre del fichero desde el encabezado Content-Disposition
                    const disposition = response.headers.get('Content-Disposition');
                    let filename = 'downloaded_file';
                    if (disposition && disposition.indexOf('filename=') !== -1) {
                        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        const matches = filenameRegex.exec(disposition);
                        if (matches != null && matches[1]) {
                            filename = matches[1].replace(/['"]/g, '');
                        }
                    }
                    return response.blob().then(blob => ({ blob, filename }));
                })
                .then(({ blob, filename }) => {
                    // Crear una URL temporal para el blob
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.style.display = 'none';
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                    // Limpiar
                    window.URL.revokeObjectURL(url);
                    a.remove();
                })
                .catch(error => {
                    console.error('Error downloading the file:', error);
                    alert('There was an error downloading the file.');
                });

        }

        this.exportToFile = function (format) {
            var options = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/octet-stream',
                    'TabId': that.exportsService.framework.getTabId(),
                    'ModuleId': that.exportsService.framework.getModuleId()
                }
            }

            var afValue = that.exportsService.framework.getAntiForgeryValue();
            if (afValue) {
                const additionalHeaders = {
                    'RequestVerificationToken': afValue,
                };
                options.headers = {
                    ...options.headers,
                    ...additionalHeaders
                }
            }
            let params = {
                sid: context.WorkspaceId,
                rid: context.Id,
                format: format
            }

            Common.CallWithOptions("GET", "Export", that.exportsService, params, options,
                function (data) {
                    if (data) {
                        that.exportsQueue().push(new ExportItemModel(
                            data.id,
                            data.createdDateTime,
                            data.lastActionDateTime,
                            data.reportId,
                            data.reportName,
                            data.status,
                            data.percentComplete,
                            data.resourceLocation,
                            data.ResourceFileExtension,
                            data.expirationTime
                        ));
                        that.exportsQueue.valueHasMutated();
                    }
                    else {
                        alert("There was an error exporting the file.");
                    }
                },
                function (error) {
                    console.log(error);
                },
                function () {
                }
            );
        }


        this.pbireload = function () {
            that.report.reload();
        }

        this.pbiprint = function () {
            that.report.print();
        }

        this.openNav = function () {
            $("#mySidenav_" + context.ModuleId).css("width", "250px");
        }

        this.closeNav = function () {
            $("#mySidenav_" + context.ModuleId).css("width", "0");
            that.selectedBookmark(null);
        }

        window.onresize = function () {
            that.isMobile = window.innerWidth < 425 || window.innerHeight < 425;

            isLandscape = window.innerWidth > window.innerHeight;
            if (!that.isMobile && isLandscape) {
                var h = that.defaultHeight !== '' ? that.defaultHeight : window.innerHeight - 50 - 20 - 39 - 20 - 10;

                $('#embedContainer_' + context.ModuleId).css("height", h + "px");
            }
        }

        window.onmessage = function (m) {
            if (m && m.isTrusted
                && m.origin === "https://app.powerbi.com"
                && m.type === "message"
                && m.data.body) {
                switch (m.data.body.event) {
                    case "tileClicked":
                        var reportId = that.getParameterByName("reportId", m.data.body.reportEmbedUrl);
                        if (reportId && reportId !== "")
                            window.location = context.ReportsPage + '?reportId=' + reportId;
                        break;
                    default: break;
                }
            }
        }

        this.timesZones = ko.observableArray([]);

        context.TimeZones.forEach(function (timeZone) {
            that.timesZones.push({
                Id: timeZone.Id,
                DisplayName: timeZone.DisplayName
            });
        });

        this.Init = function () {
            that.createBookmarksList();
        };
    };

    ko.validation.init({ insertMessages: false, decorateElement: false, registerExtenders: true });

    // Initialize
    for (var i = 0; i < contexts.length; i++) {
        if (contexts[i].Token) {
            var viewmodel = new app.View(contexts[i]);
            viewmodel.Init();
            ko.applyBindings(viewmodel, $(`#powerbiContentView_` + contexts[i].ModuleId)[0]);
        }
    }
});