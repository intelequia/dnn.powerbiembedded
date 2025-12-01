import { combineReducers } from "redux";
import settings from "./settingsReducer";
import capacityManagement from "./capacityManagementReducer";
import capacitySettings from "./capacitySettingsReducer";

const rootReducer = combineReducers({
    settings,
    capacityManagement,
    capacitySettings
});

export default rootReducer;
