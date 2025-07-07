import ApplicationService from "../services/applicationService";
import { capacityManagement as ActionTypes } from "../constants/actionTypes";

const CapacityManagementActions = {
    getCapacityStatus(settingsId, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.GET_CAPACITY_STATUS_STARTED
            });

            ApplicationService.getCapacityStatus(settingsId, data => {
                dispatch({
                    type: ActionTypes.GET_CAPACITY_STATUS_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.GET_CAPACITY_STATUS_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },

    startCapacity(settingsId, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.START_CAPACITY_STARTED
            });

            ApplicationService.startCapacity(settingsId, data => {
                dispatch({
                    type: ActionTypes.START_CAPACITY_SUCCESS,
                    data: data
                });
                // Refresh capacity status after successful start
                dispatch(CapacityManagementActions.getCapacityStatus(settingsId));
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.START_CAPACITY_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },

    pauseCapacity(settingsId, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.PAUSE_CAPACITY_STARTED
            });

            ApplicationService.pauseCapacity(settingsId, data => {
                dispatch({
                    type: ActionTypes.PAUSE_CAPACITY_SUCCESS,
                    data: data
                });
                // Refresh capacity status after successful pause
                dispatch(CapacityManagementActions.getCapacityStatus(settingsId));
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.PAUSE_CAPACITY_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },

    getCapacityRules(settingsId, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.GET_CAPACITY_RULES_STARTED
            });

            ApplicationService.getCapacityRules(settingsId, data => {
                dispatch({
                    type: ActionTypes.GET_CAPACITY_RULES_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.GET_CAPACITY_RULES_FAILED,
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
                type: ActionTypes.CREATE_CAPACITY_RULE_STARTED
            });

            ApplicationService.createCapacityRule(rule, data => {
                dispatch({
                    type: ActionTypes.CREATE_CAPACITY_RULE_SUCCESS,
                    data: data
                });
                // Refresh rules list
                dispatch(CapacityManagementActions.getCapacityRules(rule.settingsId));
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.CREATE_CAPACITY_RULE_FAILED,
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
                type: ActionTypes.UPDATE_CAPACITY_RULE_STARTED
            });

            ApplicationService.updateCapacityRule(rule, data => {
                dispatch({
                    type: ActionTypes.UPDATE_CAPACITY_RULE_SUCCESS,
                    data: data
                });
                // Refresh rules list
                dispatch(CapacityManagementActions.getCapacityRules(rule.settingsId));
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.UPDATE_CAPACITY_RULE_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    },

    deleteCapacityRule(ruleId, callback, failureCallback) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.DELETE_CAPACITY_RULE_STARTED
            });

            ApplicationService.deleteCapacityRule(ruleId, data => {
                dispatch({
                    type: ActionTypes.DELETE_CAPACITY_RULE_SUCCESS,
                    data: data
                });
                if (callback) {
                    callback(data);
                }
            }, data => {
                dispatch({
                    type: ActionTypes.DELETE_CAPACITY_RULE_FAILED,
                    data: data
                });
                if (failureCallback) {
                    failureCallback(data);
                }
            });
        };
    }
};

export default CapacityManagementActions;