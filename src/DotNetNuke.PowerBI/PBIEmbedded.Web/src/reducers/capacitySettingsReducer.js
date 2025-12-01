import { capacitySettings as ActionTypes } from "../constants/actionTypes"; 

export default function capacitySettingsReducer(state = {
    capacities: [],
    loading: false,
    error: null
}, action) {
    switch (action.type) {
        case ActionTypes.GET_CAPACITIES_STARTED:
            return { ...state, loading: true, error: null };
        case ActionTypes.GET_CAPACITIES_SUCCESS:
            return { ...state, capacities: action.data, loading: false };
        case ActionTypes.GET_CAPACITIES_FAILED:
            return { ...state, error: action.data, loading: false };
        
        case ActionTypes.CREATE_CAPACITY_STARTED:
        case ActionTypes.UPDATE_CAPACITY_STARTED:
        case ActionTypes.DELETE_CAPACITY_STARTED:
            return { ...state, loading: true, error: null };
        
        case ActionTypes.CREATE_CAPACITY_SUCCESS:
        case ActionTypes.UPDATE_CAPACITY_SUCCESS:
        case ActionTypes.DELETE_CAPACITY_SUCCESS:
            return { ...state, loading: false };
        
        case ActionTypes.CREATE_CAPACITY_FAILED:
        case ActionTypes.UPDATE_CAPACITY_FAILED:
        case ActionTypes.DELETE_CAPACITY_FAILED:
            return { ...state, error: action.data, loading: false };
        
        default:
            return state;
    }
}
