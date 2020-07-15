import React, {Component} from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import SettingsActions from "../../../actions/settings";
import { PermissionGrid}  from "@dnnsoftware/dnn-react-common";
import "./style.less";
import utils from "../../../utils";
import resx from "../../../resources";

class PbiObjectsListView extends Component {

    constructor() {
        super(); 

        this.state = {
            selectedWorkspace: "",
            powerBiObjects: null,
            selectedObjectId: "",
            error: {
                selectedWorkspace: false
            }
        };
    }

    UNSAFE_componentWillMount() {
        const {props, state} = this;

        state.selectedWorkspace = props.selectedWorkspace || -1;
        state.powerBiObjects = props.powerBiObjects;
        state.workspaces = props.workspaces;
        state.selectedObjectId = props.selectedObjectId || "";
        props.dispatch(SettingsActions.getWorkspaces());
    }

    UNSAFE_componentWillReceiveProps(nextProps) {
        const {state} = this;
        state.selectedWorkspace = nextProps.selectedWorkspace;
        state.powerBiObjects = nextProps.powerBiObjects;
        state.selectedObjectId = nextProps.selectedObjectId;
    }      

    selectObject(objId) {
        const {props,state} = this;
        state.selectedObjectId = objId;
        
        props.dispatch(SettingsActions.selectObject(objId));
    }
    getSelectedObject(pbiObjects) {
        const {state} = this;
        if (!pbiObjects)
            pbiObjects = state.powerBiObjects;
        if (state.selectedObjectId !== "" && pbiObjects && pbiObjects.length > 0) {
            return pbiObjects.find(x => x.Id === state.selectedObjectId);
        }
        return null;
    }

    getObjects(objType, noObjectsResKey) {
        const {state} = this;
        let objects = [];
        if (!state.powerBiObjects)
            return (<li className="none">{resx.get(noObjectsResKey)}</li>);

        let pbiObjects = state.powerBiObjects.filter(obj => obj.PowerBiType === objType);
        if (pbiObjects.length > 0) {
            pbiObjects.forEach(obj => {
                objects.push(<li><a className={"pbiObject" + (obj.Id === state.selectedObjectId ? " selected": "")} onClick={this.selectObject.bind(this, obj.Id)}>{obj.Name}</a></li>);
            });
            return objects;
        }
        else {
            return (<li className="none">{resx.get(noObjectsResKey)}</li>);
        }
    }

    renderDashboards() {
        return (
            <div className="dashboards">
                <label>{resx.get("lblDashboards")}</label>
                <ul>
                    {this.getObjects(1, "NoDashboards")}
                </ul>
            </div>
        );
    }  
    
    renderWorkspace() {
        return (
            <div className="workspace">
                <label>{resx.get("DefaultWorkspacePermissions")}</label>                
                <ul>
                    {this.getObjects(-1, "NoWorkspaces")}
                </ul>
            </div>
        );
    }  


    renderReports() {
        return (
            <div className="reports">
                <label>{resx.get("lblReports")}</label>
                <ul>
                    {this.getObjects(0, "NoReports")}
                </ul>                
            </div>
        );
    }

    onPermissionsChanged(permissions) {
        const {props,state} = this;

        let pbiObjects = JSON.parse(JSON.stringify(state.powerBiObjects));

        let selectedObject = this.getSelectedObject(pbiObjects);
        selectedObject.Permissions.rolePermissions = permissions.rolePermissions;
        selectedObject.Permissions.userPermissions = permissions.userPermissions;

        props.dispatch(SettingsActions.permissionsChanged(pbiObjects));        
    }

    getPermissions() {
        let permissions = {
            permissionDefinitions: [
                {
                    allowAccess: false,
                    fullControl: false,
                    permissionCode: null,
                    permissionId: 1,
                    permissionKey: null,
                    permissionName: "View",
                    view: true
                }
            ],
            rolePermissions: [],
            userPermissions: []
        };
        let selectedObject = this.getSelectedObject();    
        if (selectedObject) {
            permissions = JSON.parse(JSON.stringify(selectedObject.Permissions));
        }
        return permissions;
    }

    /* eslint-disable react/no-danger */
    render() {

        let sf = utils.utilities.sf;

        return (
            <div className="dnn-pbiembedded-objectlist">
                <div className="listview">
                    {this.renderWorkspace()}
                    {this.renderDashboards()}
                    {this.renderReports()}
                </div>
                <div className="permissions">
                    <PermissionGrid
                        permissions={this.getPermissions()}
                        service={sf}
                        onPermissionsChanged={this.onPermissionsChanged.bind(this)}
                    />
                </div>
            </div>
        );
    }

}


PbiObjectsListView.propTypes = {
    dispatch: PropTypes.func.isRequired,
    powerBiObjects: PropTypes.Object,
    selectedWorkspace: PropTypes.Number,
    selectedObjectId: PropTypes.string
};


function mapStateToProps(state) {
    return {
        powerBiObjects: state.settings.powerBiObjects,
        selectedWorkspace: state.settings.selectedWorkspace,
        selectedObjectId: state.settings.selectedObjectId
    };
}
export default connect(mapStateToProps)(PbiObjectsListView);