import util from "../utils";
class ApplicationService {
    getServiceFramework(controller) {
        let sf = util.utilities.sf;
        return Object.create(sf, {
            controller: {
                value: controller,
                writable: true,
                enumerable: true,
                configurable: true
            }
        });
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

    getCapacityStatus(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const settingsId = payload.SettingsId || payload;
        sf.get("GetCapacityStatus", { SettingsId: settingsId }, callback, failureCallback);
    }

    startCapacity(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const settingsId = payload.SettingsId || payload;
        const url = `StartCapacity?settingsId=${settingsId}`;
        
        sf.post(url, {}, 
            (data) => {
                callback(data);
            }, 
            (error) => {
                failureCallback(error);
            }
        );
    }

    pauseCapacity(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const settingsId = payload.SettingsId || payload;
        const url = `PauseCapacity?settingsId=${settingsId}`;
        
        sf.post(url, {}, 
            (data) => {
                callback(data);
            }, 
            (error) => {
                failureCallback(error);
            }
        );
    }

    getCapacityRules(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const settingsId = payload.SettingsId || payload;
        sf.get("GetCapacityRules", { SettingsId: settingsId }, 
            (data) => {
                callback(data);
            }, 
            (error) => {
                failureCallback(error);
            }
        );
    }

    createCapacityRule(rule, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const url = `CreateCapacityRule?settingsId=${rule.settingsId}`;
        sf.post(url, rule, callback, failureCallback);
    }

    updateCapacityRule(rule, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const url = "UpdateCapacityRule";
        sf.post(url, rule, callback, failureCallback);
    }

    deleteCapacityRule(payload, callback, failureCallback) {
        const sf = this.getServiceFramework("CapacityManagement");
        const ruleId = payload.RuleId || payload;
        const url = `DeleteCapacityRule?ruleId=${ruleId}`;
        sf.post(url, {}, callback, failureCallback);
    }
}
const applicationService = new ApplicationService();
export default applicationService;