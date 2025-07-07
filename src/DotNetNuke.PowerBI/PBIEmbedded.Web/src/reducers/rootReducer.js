import { combineReducers } from "redux";
import settings from "./settingsReducer";
import capacityManagement from "./capacityManagementReducer";

const rootReducer = combineReducers({
    settings,
    capacityManagement
});

export default rootReducer;
