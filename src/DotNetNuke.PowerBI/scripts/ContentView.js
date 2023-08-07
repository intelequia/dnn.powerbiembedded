jQuery(function ($) {
    var app = app || {};
    var contexts = [];
    if (!dnn.vars)
        dnn.getVars();
    for (var i in dnn.vars) {
        if (i.startsWith("ViewContext_")) {
            contexts.push(JSON.parse(dnn.getVar(i)));
        }
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

    app.View = function (c) {
        var that = this;
        var context = c;

        //Bookmarks
        this.bookmarksArray = ko.observableArray([]);
        this.selectedBookmark = ko.observable(null);
        this.newBookmarkName = ko.observable('').extend({ required: true });
        this.service = {
            path: "Bookmarks",
            controller: "Bookmarks",
            framework: $.ServicesFramework(context.ModuleId)
        }
        this.service.baseUrl = that.service.framework.getServiceRoot(that.service.path);

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
            permissions: that.models.Permissions.All,
            viewMode: that.models.ViewMode.View,
            settings: {
                //panes: {
                //    bookmarks: {
                //        visible: true
                //},
                //panes: {
                //    filters: {
                //        visible: false, // Hide the filter pane
                //    },
                //    pageNavigation: {
                //        visible: false, // Hide the page navigation pane
                //    },
                //    slicer: {
                //        visible: false, // Hide the slicer pane
                //    },
                //},
                navContentPaneEnabled: context.NavPaneVisible,
                localeSettings: {
                    language: context.Locale,
                    formatLocale: context.Locale
                },
                //background: models.BackgroundType.Transparent,
                layoutType: that.isMobile ? (that.isLandscape ? that.models.LayoutType.MobileLandscape : that.models.LayoutType.MobilePortrait) : null
            },
            pageName: context.PageName
        };

        if (this.overrideVisualHeaderVisibility) {
            this.config.settings.visualSettings = {
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
            this.config.settings.filterPaneEnabled = context.FilterPaneVisible && !that.isMobile;
        }

        // Get a reference to the embedded report HTML element
        this.reportContainer = $('#embedContainer_' + context.ModuleId)[0];
        if (!that.isMobile && that.isLandscape) {
            var h = that.defaultHeight !== '' ? that.defaultHeight : window.innerHeight - 50 - 20 - 39 - 20 - 10;
            $('#embedContainer_' + context.ModuleId).css("height", h + "px");
        }

        // Embed the report and display it within the div container.
        this.report = powerbi.embed(that.reportContainer, that.config);


        if (this.overrideFilterPaneVisibility) {
            const newSettings = {
                panes: {
                    filters: {
                        visible: context.FilterPaneVisible && !that.isMobile, // hide filter pane
                    }
                }
            }
            that.report.updateSettings(newSettings)
                .catch(error => { console.log(error) });
        }

        //Getreport bookmarks
        this.report.bookmarksManager.getBookmarks()
            .then(function (bookmarks) {
                // Create bookmarks list from the existing report bookmarks 
                that.updateBookmarksList(bookmarks);
            });
        this.trackEvent = function(eventName, data) {
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
        //embedContainer_389



        this.createBookmarksList = function() {
            let params = {
                reportId: context.Id
            }

            Common.Call("GET", "GetBookmarks", that.service, params,
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

        this.updateBookmarksList = function(bookmarks) {
            // Set bookmarks array to the report's fetched bookmarks
            that.bookmarksArray(bookmarks);
            // Set first bookmark active
            if (bookmarks.length > 0) {
                that.selectedBookmark(bookmarks[0].name);

                // Apply first bookmark state
                that.onBookmarkClicked(bookmarks[0]);
            }
        }

        this.onBookmarkClicked = function(element) {
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
                        Common.Call("POST", "SaveBookmark", that.service, bookmark,
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

            Common.Call("POST", "DeleteBookmark", that.service, bookmark,
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

        this.pbiedit = async function (hidePanes) {
            await that.report.switchMode("edit");
            if (hidePanes) {
                const newSettings = {
                    panes: {
                        filters: {
                            visible: true
                        },
                        visualizations: {
                            visible: false
                        },
                        fields: {
                            visible: false
                        },
                    }
                };
                await that.report.updateSettings(newSettings);
            }
        };
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