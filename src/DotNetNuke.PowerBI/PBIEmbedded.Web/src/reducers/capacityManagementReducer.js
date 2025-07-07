import { capacityManagement as ActionTypes } from "../constants/actionTypes";

export default function capacityManagementReducer(state = {
    capacityStatus: null,
    capacityRules: [],
    loading: false,
    error: null
}, action) {
    switch (action.type) {
        case ActionTypes.GET_CAPACITY_STATUS_STARTED:
        case ActionTypes.START_CAPACITY_STARTED:
        case ActionTypes.PAUSE_CAPACITY_STARTED:
        case ActionTypes.GET_CAPACITY_RULES_STARTED:
        case ActionTypes.CREATE_CAPACITY_RULE_STARTED:
        case ActionTypes.UPDATE_CAPACITY_RULE_STARTED:
        case ActionTypes.DELETE_CAPACITY_RULE_STARTED:
            return {
                ...state,
                loading: true,
                error: null
            };

        case ActionTypes.GET_CAPACITY_STATUS_SUCCESS:
            return {
                ...state,
                capacityStatus: action.data,
                loading: false,
                error: null
            };

        case ActionTypes.START_CAPACITY_SUCCESS:
        case ActionTypes.PAUSE_CAPACITY_SUCCESS:
            return {
                ...state,
                loading: false,
                error: null
            };

        case ActionTypes.GET_CAPACITY_RULES_SUCCESS:
            return {
                ...state,
                capacityRules: action.data,
                loading: false,
                error: null
            };

        case ActionTypes.CREATE_CAPACITY_RULE_SUCCESS:
        case ActionTypes.UPDATE_CAPACITY_RULE_SUCCESS:
            return {
                ...state,
                loading: false,
                error: null
            };

        case ActionTypes.DELETE_CAPACITY_RULE_SUCCESS:
            return {
                ...state,
                capacityRules: state.capacityRules.filter(rule => rule.ruleId !== action.data.ruleId),
                loading: false,
                error: null
            };

        case ActionTypes.GET_CAPACITY_STATUS_FAILED:
        case ActionTypes.START_CAPACITY_FAILED:
        case ActionTypes.PAUSE_CAPACITY_FAILED:
        case ActionTypes.GET_CAPACITY_RULES_FAILED:
        case ActionTypes.CREATE_CAPACITY_RULE_FAILED:
        case ActionTypes.UPDATE_CAPACITY_RULE_FAILED:
        case ActionTypes.DELETE_CAPACITY_RULE_FAILED:
            return {
                ...state,
                loading: false,
                error: action.data
            };

        default:
            return state;
    }
}