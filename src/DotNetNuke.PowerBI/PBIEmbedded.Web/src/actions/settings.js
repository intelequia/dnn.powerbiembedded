import { settings as ActionTypes, capacityManagement as CapacityActionTypes, capacitySettings as CapacitySettingsActionTypes } from "../constants/actionTypes";
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
    clearCapacityData() {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.CLEAR_CAPACITY_DATA
            });
        };
    },
    clearOperationError() {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.CLEAR_OPERATION_ERROR
            });
        };
    },
    getCapacityStatus(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.GET_CAPACITY_STATUS_STARTED
            });
            
            ApplicationService.getCapacityStatus(payload, data => {
                dispatch({
                    type: CapacityActionTypes.GET_CAPACITY_STATUS_SUCCESS,
                    data: data,
                    payload: payload
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.GET_CAPACITY_STATUS_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    pollCapacityStatus(payload, callback) {
        return (dispatch) => {
            ApplicationService.getCapacityStatus(payload, data => {
                dispatch({
                    type: CapacityActionTypes.POLL_CAPACITY_STATUS_SUCCESS,
                    data: data,
                    payload: payload
                });
                if (callback) {
                    callback(data);
                }
            }, () => {
                // Silently fail during polling
            });
        };
    },
    startCapacity(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.START_CAPACITY_STARTED
            });
            
            ApplicationService.startCapacity(payload, data => {
                dispatch({
                    type: CapacityActionTypes.START_CAPACITY_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.START_CAPACITY_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    pauseCapacity(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.PAUSE_CAPACITY_STARTED
            });
            
            ApplicationService.pauseCapacity(payload, data => {
                dispatch({
                    type: CapacityActionTypes.PAUSE_CAPACITY_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.PAUSE_CAPACITY_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    getCapacityRules(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.GET_CAPACITY_RULES_STARTED
            });
            
            ApplicationService.getCapacityRules(payload, data => {
                dispatch({
                    type: CapacityActionTypes.GET_CAPACITY_RULES_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.GET_CAPACITY_RULES_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    createCapacityRule(rule, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.CREATE_CAPACITY_RULE_STARTED
            });
            
            const ruleCopy = { ...rule };
            
            ApplicationService.createCapacityRule(ruleCopy, data => {
                dispatch({
                    type: CapacityActionTypes.CREATE_CAPACITY_RULE_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.CREATE_CAPACITY_RULE_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    updateCapacityRule(rule, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.UPDATE_CAPACITY_RULE_STARTED
            });
            
            const ruleCopy = { ...rule };
            
            ApplicationService.updateCapacityRule(ruleCopy, data => {
                dispatch({
                    type: CapacityActionTypes.UPDATE_CAPACITY_RULE_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.UPDATE_CAPACITY_RULE_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    deleteCapacityRule(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: CapacityActionTypes.DELETE_CAPACITY_RULE_STARTED
            });
            
            const payloadCopy = { ...payload };
            
            ApplicationService.deleteCapacityRule(payloadCopy, data => {
                const successData = { ruleId: payloadCopy.RuleId, ...data };
                dispatch({
                    type: CapacityActionTypes.DELETE_CAPACITY_RULE_SUCCESS,
                    data: successData
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: CapacityActionTypes.DELETE_CAPACITY_RULE_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },
    // Capacity Settings Actions
    getCapacities(callback) {
        return (dispatch) => {
            dispatch({ type: CapacitySettingsActionTypes.GET_CAPACITIES_STARTED });
            ApplicationService.getCapacities(data => {
                dispatch({
                    type: CapacitySettingsActionTypes.GET_CAPACITIES_SUCCESS,
                    data: Array.isArray(data) ? data : (data.capacities || data.Capacities || [])
                });
                if (callback) callback(data);
            }, error => {
                dispatch({
                    type: CapacitySettingsActionTypes.GET_CAPACITIES_FAILED,
                    data: error
                });
            });
        };
    },

    createCapacity(capacity, callback, failureCallback) {
        return (dispatch) => {
            dispatch({ type: CapacitySettingsActionTypes.CREATE_CAPACITY_STARTED });
            ApplicationService.createCapacity(capacity, data => {
                dispatch({ type: CapacitySettingsActionTypes.CREATE_CAPACITY_SUCCESS });
                if (callback) callback(data);
            }, error => {
                dispatch({ type: CapacitySettingsActionTypes.CREATE_CAPACITY_FAILED, data: error });
                if (failureCallback) failureCallback(error);
            });
        };
    },

    updateCapacity(capacity, callback, failureCallback) {
        return (dispatch) => {
            dispatch({ type: CapacitySettingsActionTypes.UPDATE_CAPACITY_STARTED });
            ApplicationService.updateCapacity(capacity, data => {
                dispatch({ type: CapacitySettingsActionTypes.UPDATE_CAPACITY_SUCCESS });
                if (callback) callback(data);
            }, error => {
                dispatch({ type: CapacitySettingsActionTypes.UPDATE_CAPACITY_FAILED, data: error });
                if (failureCallback) failureCallback(error);
            });
        };
    },

    deleteCapacity(payload, callback, failureCallback) {
        return (dispatch) => {
            dispatch({ type: CapacitySettingsActionTypes.DELETE_CAPACITY_STARTED });
            ApplicationService.deleteCapacity(payload, data => {
                dispatch({ type: CapacitySettingsActionTypes.DELETE_CAPACITY_SUCCESS });
                if (callback) callback(data);
            }, error => {
                dispatch({ type: CapacitySettingsActionTypes.DELETE_CAPACITY_FAILED, data: error });
                if (failureCallback) failureCallback(error);
            });
        };
    }

};

export default settingsActions;