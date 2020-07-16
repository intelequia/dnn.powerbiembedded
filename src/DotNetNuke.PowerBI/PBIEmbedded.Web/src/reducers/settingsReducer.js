import { settings as ActionTypes } from "../constants/actionTypes";

export default function settings(state = {
    selectedTab: 0,
    workspaces: null,
    powerBiObjects: null,
    selectedWorkspace: ""
}, action) {
    switch (action.type) {
            
        case ActionTypes.RETRIEVED_WORKSPACES:
            return { ...state,
                workspaces: action.data.workspaces,
                clientModified: action.data.clientModified
            };
        case ActionTypes.UPDATED_WORKSPACE:
            return { ...state,
                clientModified: action.data.clientModified
            };            
        case ActionTypes.SWITCH_TAB:
            return {
                ...state,
                selectedTab: action.payload
            };
        case ActionTypes.CANCELLED_WORKSPACE_CLIENT_MODIFIED:
            return { ...state,
                workspaceClientModified: action.data.workspaceClientModified
            };
        case ActionTypes.WORKSPACE_CLIENT_MODIFIED:
            return { ...state,
                workspaceDetail: action.data.workspaceDetail,
                workspaceClientModified: action.data.workspaceClientModified
            };
        case ActionTypes.RETRIEVED_POWERBI_OBJECT_LIST:
            return { ...state,
                inheritPermissions: action.data.inheritPermissions,
                powerBiObjects: action.data.powerBiObjects,
                permissionsClientModified: action.data.permissionsClientModified
            };       
        case ActionTypes.SELECTED_POWERBI_OBJECT:
            return {...state,
                selectedObjectId: action.data.selectedObjectId,
                permissionsClientModified: action.data.permissionsClientModified
            };
        case ActionTypes.INHERIT_PERMISSIONS_CHANGED:
            return {...state,
                inheritPermissions: action.data.inheritPermissions,
                permissionsClientModified: action.data.permissionsClientModified
            };
        case ActionTypes.PERMISSIONS_CHANGED:
            return {...state,
                powerBiObjects: action.data.powerBiObjects,
                permissionsClientModified: action.data.permissionsClientModified                
            };
        default:
            return { ...state
            };
    }
}
