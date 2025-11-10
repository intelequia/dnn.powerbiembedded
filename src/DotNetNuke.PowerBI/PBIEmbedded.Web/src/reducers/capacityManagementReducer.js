import { capacityManagement as ActionTypes } from "../constants/actionTypes";

export default function capacityManagementReducer(state = {
    capacityStatus: null,
    capacityRules: [],
    loading: false,
    error: null,
    statusError: null,
    operationError: null
}, action) {
    switch (action.type) {
        case ActionTypes.CLEAR_CAPACITY_DATA:
            return {
                ...state,
                capacityStatus: null,
                capacityRules: [],
                error: null,
                statusError: null,
                operationError: null
            };
            
        case ActionTypes.CLEAR_OPERATION_ERROR:
            return {
                ...state,
                operationError: null
            };
            
        case ActionTypes.GET_CAPACITY_STATUS_STARTED:
            return {
                ...state,
                loading: true,
                statusError: null  // Only clear status-related errors
            };
            
        case ActionTypes.GET_CAPACITY_RULES_STARTED:
            return {
                ...state,
                loading: true
            };

        case ActionTypes.START_CAPACITY_STARTED:
        case ActionTypes.PAUSE_CAPACITY_STARTED:
            return {
                ...state,
                loading: true,
                operationError: null
            };

        case ActionTypes.CREATE_CAPACITY_RULE_STARTED:
        case ActionTypes.UPDATE_CAPACITY_RULE_STARTED:
        case ActionTypes.DELETE_CAPACITY_RULE_STARTED:
            return {
                ...state,
                loading: true,
                operationError: null
            };

        case ActionTypes.GET_CAPACITY_STATUS_SUCCESS:
            return {
                ...state,
                capacityStatus: action.data,
                loading: false,
                error: null,
                statusError: null
            };

        case ActionTypes.POLL_CAPACITY_STATUS_SUCCESS:
            return {
                ...state,
                capacityStatus: action.data
            };

        case ActionTypes.START_CAPACITY_SUCCESS:
        case ActionTypes.PAUSE_CAPACITY_SUCCESS:
            return {
                ...state,
                loading: false,
                error: null,
                operationError: null
            };

        case ActionTypes.GET_CAPACITY_RULES_SUCCESS:
            return {
                ...state,
                capacityRules: action.data,
                loading: false,
                error: null
            };

        case ActionTypes.CREATE_CAPACITY_RULE_SUCCESS:
            return {
                ...state,
                capacityRules: [action.data, ...state.capacityRules],
                loading: false,
                error: null,
                operationError: null
            };
        
        case ActionTypes.UPDATE_CAPACITY_RULE_SUCCESS:
            return {
                ...state,
                capacityRules: state.capacityRules.map(rule => 
                    rule.RuleId === action.data.RuleId ? action.data : rule
                ),
                loading: false,
                error: null,
                operationError: null
            };

        case ActionTypes.DELETE_CAPACITY_RULE_SUCCESS:
            return {
                ...state,
                capacityRules: state.capacityRules.filter(rule => rule.RuleId !== action.data.RuleId),
                loading: false,
                error: null,
                operationError: null
            };

        case ActionTypes.GET_CAPACITY_STATUS_FAILED:
            return {
                ...state,
                loading: false,
                error: action.data,
                statusError: action.data
            };

        case ActionTypes.START_CAPACITY_FAILED:
        case ActionTypes.PAUSE_CAPACITY_FAILED:
        case ActionTypes.GET_CAPACITY_RULES_FAILED:
        case ActionTypes.CREATE_CAPACITY_RULE_FAILED:
        case ActionTypes.UPDATE_CAPACITY_RULE_FAILED:
        case ActionTypes.DELETE_CAPACITY_RULE_FAILED:
            return {
                ...state,
                loading: false,
                operationError: action.data
            };

        default:
            return state;
    }
}