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
}
const applicationService = new ApplicationService();
export default applicationService;