import React, {Component} from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridSystem, GridCell, Button, InputGroup, DropdownWithError}  from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../actions/settings";
import resx from "../../resources";
import "./permissions.less";
import utils from "../../utils";
import  PbiObjectListView from "./pbiObjectsListView";

class Permissions extends Component {

    constructor() {
        super();

        this.state = {
            selectedWorkspace: "",
            selectedObjectId: "",
            powerBiObjects: null,
            workspaces: null,
            error: {
                selectedWorkspace: false
            }
        };
    }

    UNSAFE_componentWillMount() {
        const {props, state} = this;

        state.selectedWorkspace = props.selectedWorkspace || "";
        state.selectedObjectId = props.selectedObjectId || "";
        state.powerBiObjects = props.powerBiObjects;
        state.workspaces = props.workspaces;
        props.dispatch(SettingsActions.getWorkspaces());
    }

    UNSAFE_componentWillReceiveProps(nextProps) {
        const {props,state} = this;

        if (state.selectedWorkspace === "" && nextProps.workspaces.length > 0) {
            state.selectedWorkspace = nextProps.workspaces[0].SettingsId;
            props.dispatch(SettingsActions.getPowerBiObjectList(state.selectedWorkspace));
        }
        state.powerBiObjects = nextProps.powerBiObjects;
        if (state.selectedObjectId === "" && nextProps.powerBiObjects && nextProps.powerBiObjects.length > 0) {
            state.selectedObjectId = nextProps.powerBiObjects[0].Id;
            props.dispatch(SettingsActions.selectObject(state.selectedObjectId));
        }
        else {
            if (state.selectedObjectId === "" && nextProps.powerBiObjects && nextProps.powerBiObjects.length > 0) {
                if (!nextProps.powerBiObjects.find(x => x.Id === state.selectedObjectId)) {
                    state.selectedObjectId = "";
                    props.dispatch(SettingsActions.selectObject(state.selectedObjectId));        
                }
            }
        }
        state.error["selectedWorkspace"] = (!state.selectedWorkspace || state.selectedWorkspace === "");
    }      

    onSettingChange(key, event) {
        let {state, props} = this;

        switch (key) {
            case "SelectedWorkspace":
                state.error["selectedWorkspace"] = event.value === "";
                state.selectedWorkspace = event.value;
                props.dispatch(SettingsActions.getPowerBiObjectList(event.value));
                break;
            default:
                break;
        }

        this.setState({
            selectedWorkspace: state.selectedWorkspace,
            workspaces: state.workspaces,
            powerBiObjects: state.powerBiObjects,
            triedToSubmit: false
        });


    }    

    onClickCancel() {
        utils.utilities.closePersonaBar();
    }
    
    onClickSave() {
        event.preventDefault();
        let {props} = this;

        props.dispatch(SettingsActions.updatePermissions({
            settingsId: props.selectedWorkspace,
            powerBiObjects: props.powerBiObjects
        }, () => {
            utils.utilities.notify(resx.get("PermissionsUpdateSuccess"));
            this.setState({
                clientModified: false
            });            
        }, () => {
            utils.utilities.notifyError(resx.get("PermissionsUpdateError"));
        }));
    }
    
    getWorkspaceOptions() {
        let options = [];
    
        if (this.props.workspaces !== undefined) {
            options = this.props.workspaces.map((item) => {
                return { label: item.SettingsGroupName, value: item.SettingsId };
            });
        }
        return options;
    }


    /* eslint-disable react/no-danger */
    render() {
        return (
            <div className="dnn-pbiembedded-permissions">
                <h1>{resx.get("lblWorkspacePermissions")}</h1>      
                <p>{resx.get("lblPermissionsDescription")}</p>

                <InputGroup>                    
                    <GridSystem numberOfColumns={2}>
                        <GridCell columnSize={90}>  
                            <DropdownWithError
                                withLabel={true}
                                label={resx.get("lblSelectedWorkspace")}
                                tooltipMessage={resx.get("lblSelectedWorkspace.Help")}
                                error={this.state.error.selectedWorkspace}
                                errorMessage={resx.get("ErrorWorskpaceNotValid")}
                                options={this.getWorkspaceOptions()}
                                value={this.state.selectedWorkspace}
                                onSelect={this.onSettingChange.bind(this, "SelectedWorkspace")}
                            />                  
                            <div className={"dnn-label"}><label>{resx.get("lblWorkspacePermissions")}</label></div>
                        </GridCell>                           
                        <GridCell columnSize={100}>
                        </GridCell>                         
                    </GridSystem>
                </InputGroup>
                <InputGroup>
                    <PbiObjectListView selectedObjectId />
                </InputGroup>
                <InputGroup>         
                    <GridCell columnSize={100}>
                        <div className="buttons-box">
                            <Button disabled={false} type="secondary" onClick={this.onClickCancel.bind(this)}>
                                {resx.get("Cancel")}
                            </Button>
                            <Button
                                disabled={this.state.error.aadAppClientId || this.state.error.aadAppSecret }
                                type="primary"
                                onClick={this.onClickSave.bind(this)}>
                                {resx.get("SavePermissions")}
                            </Button>                        
                        </div>                    
                    </GridCell>  
                </InputGroup>
            </div>
        );
    }
}

Permissions.propTypes = {
    dispatch: PropTypes.func.isRequired,
    workspaces: PropTypes.Array,
    powerBiObjects: PropTypes.Object,
    selectedWorkspace: PropTypes.String,
    selectedObjectId: PropTypes.String
};


function mapStateToProps(state) {
    return {
        workspaces: state.settings.workspaces,
        powerBiObjects: state.settings.powerBiObjects,
        selectedWorkspace: state.settings.selectedWorkspace,
        selectedObjectId: state.settings.selectedObjectId
    };
}
export default connect(mapStateToProps)(Permissions);