import util from "../utils";
class ApplicationService {
    getServiceFramework(controller) {
        let sf = util.utilities.sf;
        sf.controller = controller;
        return sf;
    }

    getWorkspaces(callback) {
        const sf = this.getServiceFramework("PBIEmbedded");        
        sf.get("GetWorkspaces", {}, callback);
    }

    updateWorkspace(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("PBIEmbedded");        
        sf.post("UpdateWorkspace", payload, callback, failureCallback);
    }

    deleteWorkspace(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("PBIEmbedded");        
        sf.post("DeleteWorkspace", payload, callback, failureCallback);
    }

    getPowerBiObjectList(settingsId, callback) {
        const sf = this.getServiceFramework("PBIEmbedded");        
        sf.get("GetPowerBiObjectList", {settingsId: settingsId}, callback);
    }

    updatePermissions(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("PBIEmbedded");        
        sf.post("SavePowerBiObjectsPermissions", payload, callback, failureCallback);
    }

    // Capacity Management API methods
    getCapacityStatus(settingsId, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.get("GetCapacityStatus", { settingsId: settingsId }, callback, failureCallback);
    }

    startCapacity(settingsId, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.post("StartCapacity", { settingsId: settingsId }, callback, failureCallback);
    }

    pauseCapacity(settingsId, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.post("PauseCapacity", { settingsId: settingsId }, callback, failureCallback);
    }

    getCapacityRules(settingsId, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.get("GetCapacityRules", { settingsId: settingsId }, callback, failureCallback);
    }

    createCapacityRule(rule, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.post("CreateCapacityRule", rule, callback, failureCallback);
    }

    updateCapacityRule(rule, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.put("UpdateCapacityRule", rule, callback, failureCallback);
    }

    deleteCapacityRule(ruleId, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        sf.delete("DeleteCapacityRule", { ruleId: ruleId }, callback, failureCallback);
    }    
}
const applicationService = new ApplicationService();
export default applicationService;