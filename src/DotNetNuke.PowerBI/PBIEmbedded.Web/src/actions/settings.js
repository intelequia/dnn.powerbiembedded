import { settings as ActionTypes } from "../constants/actionTypes";
import ApplicationService from "../services/applicationService";

const settingsActions = {
    switchTab(index, callback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.SWITCH_TAB,
                payload: index
            });
            if (callback) {
                callback();
            }
        };
    },
    getWorkspaces(callback) {
        return (dispatch) => {
            ApplicationService.getWorkspaces(data => {
                dispatch({
                    type: ActionTypes.RETRIEVED_WORKSPACES,
                    data: {
                        workspaces: data.workspaces,
                        clientModified: false
                    }
                });
                if (callback) {
                    callback(data);
                }
            });
        };
    },
    cancelWorkspaceClientModified() {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.CANCELLED_WORKSPACE_CLIENT_MODIFIED,
                data: {
                    workspaceClientModified: false
                }
            });
        };
    },
    workspaceClientModified(parameter) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.WORKSPACE_CLIENT_MODIFIED,
                data: {
                    workspaceDetail: parameter,
                    workspaceClientModified: true
                }
            });
        };
    },
    updateWorkspace(payload, callback, failureCallback) {
        return () => {
            ApplicationService.updateWorkspace(payload, data => {
                if (callback) {
                    callback(data);
                }
            }, data => {
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    deleteWorkspace(payload, callback, failureCallback) {
        return () => {
            ApplicationService.deleteWorkspace(payload, data => {
                if (callback) {
                    callback(data);
                }
            }, data => {
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    permissionsClientModified(parameter) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.PERMISSIONS_CLIENT_MODIFIED,
                data: {
                    selectedWorkspace: parameter.selectedWorkspace,
                    permissions: parameter.permissions,
                    workspaceClientModified: true
                }
            });
        };
    },   
    getPowerBiObjectList(settingsId, callback) {
        return (dispatch) => {
            ApplicationService.getPowerBiObjectList(settingsId, data => {
                dispatch({
                    type: ActionTypes.RETRIEVED_POWERBI_OBJECT_LIST,
                    data: {
                        inheritPermissions: data.InheritPermissions,
                        powerBiObjects: data.PowerBiObjects,
                        permissionsClientModified: false
                    }
                });
                if (callback) {
                    callback(data);
                }
            });
        };
    },   
    selectObject(objectId)   {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.SELECTED_POWERBI_OBJECT,
                data: {
                    selectedObjectId: objectId,
                    permissionsClientModified: false
                }
            });
        };        
    },
    settingsClientModified(settings) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.INHERIT_PERMISSIONS_CHANGED,
                data: {
                    inheritPermissions: settings.inheritPermissions,
                    permissionsClientModified: true
                }
            });
        };
    },
    permissionsChanged(powerBiObjects) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.PERMISSIONS_CHANGED,
                data: {
                    powerBiObjects: powerBiObjects,
                    permissionsClientModified: true
                }
            });
        };          
    },
    updatePermissions(payload, callback, failureCallback) {
        return () => {
            ApplicationService.updatePermissions(payload, data => {
                if (callback) {
                    callback(data);
                }
            }, data => {
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },    
};

export default settingsActions;