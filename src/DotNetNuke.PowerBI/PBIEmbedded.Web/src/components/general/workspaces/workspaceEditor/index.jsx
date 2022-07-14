import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import "./style.less";
import { SingleLineInputWithError, GridSystem, Button, InputGroup, DropdownWithError } from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../../../actions/settings";
import util from "../../../../utils";
import resx from "../../../../resources";

class WorkspaceEditor extends Component {
    constructor() {
        super();

        this.state = {
            workspaceDetail: {
                SettingsId: 0,
                SettingsGroupId: "",
                SettingsGroupName: "",
                AuthenticationType: "MasterUser",
                Username: "",
                Password: "",
                ApplicationId: "",
                WorkspaceId: "",
                ServicePrincipalApplicationId: "",
                ServicePrincipalApplicationSecret: "",
                ServicePrincipalTenant: "",
                ContentPageUrl: "",
                DisabledCapacityMessage: "",
                InheritPermissions: false
            },
            error: {
                SettingsGroupName: false,
                AuthenticationType: false,
                Username: false,
                Password: false,
                ApplicationId: false,
                WorkspaceId: false,
                ServicePrincipalApplicationId: false,
                ServicePrincipalApplicationSecret: false,
                ServicePrincipalTenant: false,
                ContentPageUrl: false,
                DisabledCapacityMessage: false
            },
            triedToSubmit: false
        };
    }
    
    componentWillMount() {
        const {props, state} = this;

        state.workspaceDetail["SettingsId"] = props.settingsId || 0;
        state.workspaceDetail["SettingsGroupId"] = props.settingsGroupId || "";
        state.workspaceDetail["SettingsGroupName"] = props.settingsGroupName || "";
        state.workspaceDetail["AuthenticationType"] = props.authenticationType || "MasterUser";
        state.workspaceDetail["Username"] = props.username || "";
        state.workspaceDetail["Password"] = props.password || "";
        state.workspaceDetail["ApplicationId"] = props.applicationId || "";
        state.workspaceDetail["WorkspaceId"] = props.workspaceId || "";
        state.workspaceDetail["ServicePrincipalApplicationId"] = props.servicePrincipalApplicationId || "";
        state.workspaceDetail["ServicePrincipalApplicationSecret"] = props.servicePrincipalApplicationSecret || "";
        state.workspaceDetail["ServicePrincipalTenant"] = props.servicePrincipalTenant || "";
        state.workspaceDetail["ContentPageUrl"] = props.contentPageUrl || "";
        state.workspaceDetail["DisabledCapacityMessage"] = props.disabledCapacityMessage || "";
        state.workspaceDetail["InheritPermissions"] = props.inheritPermissions;

        state.error["SettingsGroupName"] = (props.settingsGroupName === null);
        state.error["AuthenticationType"] = (props.authenticationType === null);
        state.error["Username"] = (props.username === null);
        state.error["Password"] = (props.password === null);
        state.error["ApplicationId"] = (props.applicationId === null);
        state.error["WorkspaceId"] = (props.workspaceId === null);
        state.error["ServicePrincipalApplicationId"] = (props.servicePrincipalApplicationId === null);
        state.error["ServicePrincipalApplicationSecret"] = (props.servicePrincipalApplicationSecret === null);
        state.error["ServicePrincipalTenant"] = (props.servicePrincipalTenant === null);
        state.error["ContentPageUrl"] = (props.contentPageUrl === null);
        state.error["DisabledCapacityMessage"] = false; // (props.disabledCapacityMessage === null);

    }

    /* eslint-disable react/no-did-update-set-state */
    componentDidUpdate(prevProps) {
        const {props, state} = this;
        if ((props !== prevProps) && props.workspaceDetail ) {
            state.error["SettingsGroupName"] = !props.workspaceDetail["SettingsGroupName"] || props.workspaceDetail["SettingsGroupName"] === "";
            state.error["AuthenticationType"] = !props.workspaceDetail["AuthenticationType"] || props.workspaceDetail["AuthenticationType"] === "";
            state.error["Username"] = props.workspaceDetail["AuthenticationType"] === "MasterUser" && (!props.workspaceDetail["Username"] || props.workspaceDetail["Username"] === "");
            state.error["Password"] = props.workspaceDetail["AuthenticationType"] === "MasterUser" && (!props.workspaceDetail["Password"] || props.workspaceDetail["Password"] === "");
            state.error["ApplicationId"] = !props.workspaceDetail["ApplicationId"] || props.workspaceDetail["ApplicationId"] === "";
            state.error["WorkspaceId"] = !props.workspaceDetail["WorkspaceId"] || props.workspaceDetail["WorkspaceId"] === "";
            state.error["ServicePrincipalApplicationId"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalApplicationId"] || props.workspaceDetail["ServicePrincipalApplicationId"] === "");
            state.error["ServicePrincipalApplicationSecret"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalApplicationSecret"] || props.workspaceDetail["ServicePrincipalApplicationSecret"] === "");
            state.error["ServicePrincipalTenant"] = props.workspaceDetail["AuthenticationType"] === "ServicePrincipal" && (!props.workspaceDetail["ServicePrincipalTenant"] || props.workspaceDetail["ServicePrincipalTenant"] === "");
            state.error["ContentPageUrl"] = !props.workspaceDetail["ContentPageUrl"] || props.workspaceDetail["ContentPageUrl"] === "";
            state.error["DisabledCapacityMessage"] = false; //!props.workspaceDetail["DisabledCapacityMessage"] || props.workspaceDetail["DisabledCapacityMessage"] === "";
            this.setState({
                workspaceDetail: Object.assign({}, props.workspaceDetail),
                triedToSubmit: false,
                error: state.error
            });
        }
    }

    onSettingChange(key, event) {
        let {state, props} = this;
        let workspaceDetail = Object.assign({}, state.workspaceDetail);

        switch (key) {
            case "SettingsGroupName":
                state.error[key] = !props.onValidate(workspaceDetail, event);
                break;
            case "Username":
            case "Password":
                state.error[key] = workspaceDetail["AuthenticationType"] === "MasterUser" && event.target.value === "";
                break;
            case "ServicePrincipalApplicationId":
            case "ServicePrincipalApplicationSecret":
            case "ServicePrincipalTenant":
                state.error[key] = workspaceDetail["AuthenticationType"] === "ServicePrincipal" && event.target.value === "";
                break;
            case "DisabledCapacityMessage":
            case "AuthenticationType":
                break;
            case "AppicationId":
            case "WorkspaceId":
            case "ContentPageUrl":                        
            default: 
                state.error[key] = event.target.value === "";
                break;        
        }


        switch (key) {
            case "SettingsGroupName":
            case "Username":
            case "Password":
            case "AppicationId":
            case "WorkspaceId":
            case "ServicePrincipalApplicationId":
            case "ServicePrincipalApplicationSecret":
            case "ServicePrincipalTenant":
            case "ContentPageUrl":      
            case "DisabledCapacityMessage":          
                workspaceDetail[key] = event.target.value;
                break;
            case "AuthenticationType":
                workspaceDetail[key] = event.value;
                break;
            default: 
                workspaceDetail[key] = typeof (event) === "object" ? event.target.value : event;
                break;
        }

        this.setState({
            workspaceDetail: workspaceDetail,
            triedToSubmit: false,
            error: state.error
        });

        props.dispatch(SettingsActions.workspaceClientModified(workspaceDetail));
    }

    getAuthenticationTypeOptions() {
        let options = [
            {
                label: "Master User",
                value: "MasterUser"
            },
            {
                label: "Service Principal",
                value: "ServicePrincipal"
            }
        ];
        return options;
    }

    onSave() {
        const {props, state} = this;
        this.setState({
            triedToSubmit: true
        });
        if (state.error.SettingsGroupName 
            || state.error.AuthenticationType
            || (state.error.AuthenticationType === "MasterUser" 
               && (state.error.Username || state.error.Password))
            || state.error.ApplicationId
            || state.error.WorkspaceId
            || (state.error.AuthenticationType === "ServicePrincipal"
                && (state.error.ServicePrincipalApplicationId
                    || state.error.ServicePrincipalApplicationSecret
                    || state.error.ServicePrincipalTenant))
            || state.error.ContentPageUrl
            || state.error.DisabledCapacityMessage) {
            return;
        }

        props.onUpdate(state.workspaceDetail);
        props.Collapse();
    }

    onCancel() {
        const {props} = this;

        if (props.workspaceClientModified) {
            util.utilities.confirm(resx.get("SettingsRestoreWarning"), resx.get("Yes"), resx.get("No"), () => {
                props.dispatch(SettingsActions.cancelWorkspaceClientModified());
                props.Collapse();
            });
        }
        else {
            props.Collapse();
        }
    }

    renderMasterUserCredentials() {
        return (
            <div>
                <SingleLineInputWithError
                    withLabel={true}
                    label={resx.get("lblUsername")}
                    enabled={true}
                    error={this.state.error.Username}
                    errorMessage={resx.get("lblUsername.Error")}
                    tooltipMessage={resx.get("lblUsername.Help")}
                    value={this.state.workspaceDetail.Username}
                    onChange={this.onSettingChange.bind(this, "Username")}
                />
                <SingleLineInputWithError
                    withLabel={true}
                    label={resx.get("lblPassword")}
                    type="password"
                    enabled={true}
                    error={this.state.error.Password}
                    errorMessage={resx.get("lblPassword.Error")}
                    tooltipMessage={resx.get("lblPassword.Help")}
                    value={this.state.workspaceDetail.Password}
                    autoComplete="off"
                    onChange={this.onSettingChange.bind(this, "Password")}
                />
            </div>
        );
    }

    renderServicePrincipalCredentials() {
        return (
            <div>
                <SingleLineInputWithError
                    withLabel={true}
                    label={resx.get("lblServicePrincipalApplicationId")}
                    enabled={true}
                    error={this.state.error.ServicePrincipalApplicationId}
                    errorMessage={resx.get("lblServicePrincipalApplicationId.Error")}
                    tooltipMessage={resx.get("lblServicePrincipalApplicationId.Help")}
                    value={this.state.workspaceDetail.ServicePrincipalApplicationId}
                    onChange={this.onSettingChange.bind(this, "ServicePrincipalApplicationId")}
                />
                <SingleLineInputWithError
                    withLabel={true}
                    label={resx.get("lblServicePrincipalApplicationSecret")}
                    type="password"
                    enabled={true}
                    error={this.state.error.ServicePrincipalApplicationSecret}
                    errorMessage={resx.get("lblServicePrincipalApplicationSecret.Error")}
                    tooltipMessage={resx.get("lblServicePrincipalApplicationSecret.Help")}
                    value={this.state.workspaceDetail.ServicePrincipalApplicationSecret}
                    autoComplete="off"
                    onChange={this.onSettingChange.bind(this, "ServicePrincipalApplicationSecret")}
                />
                <SingleLineInputWithError
                    withLabel={true}
                    label={resx.get("lblServicePrincipalTenant")}
                    enabled={true}
                    error={this.state.error.ServicePrincipalTenant}
                    errorMessage={resx.get("lblServicePrincipalTenant.Error")}
                    tooltipMessage={resx.get("lblServicePrincipalTenant.Help")}
                    value={this.state.workspaceDetail.ServicePrincipalTenant}
                    onChange={this.onSettingChange.bind(this, "ServicePrincipalTenant")}
                />                    
            </div>      
        );
    }

    /* eslint-disable react/no-danger */
    render() {
        const isMasterUser = this.state.workspaceDetail.AuthenticationType === "MasterUser";
        if (this.state.workspaceDetail !== undefined || this.props.id === "add") {
            const columnOne = <div key="column-one" className="left-column">
                <InputGroup>
                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("lblSettingsGroupName")}
                        enabled={true}
                        error={this.state.error.SettingsGroupName}
                        errorMessage={resx.get("lblSettingsGroupName.Error")}
                        tooltipMessage={resx.get("lblSettingsGroupName.Help")}
                        value={this.state.workspaceDetail.SettingsGroupName}
                        onChange={this.onSettingChange.bind(this, "SettingsGroupName")}
                    />                 
                    <DropdownWithError
                        withLabel={true}
                        label={resx.get("lblAuthenticationType")}
                        tooltipMessage={resx.get("lblAuthenticationType.Help")}
                        error={this.state.error.AuthenticationType}
                        errorMessage={resx.get("ErrorAuthenticationTypeNotValid")}
                        options={this.getAuthenticationTypeOptions()}
                        value={this.state.workspaceDetail.AuthenticationType}
                        onSelect={this.onSettingChange.bind(this, "AuthenticationType")}
                    />
                    { isMasterUser
                        ? this.renderMasterUserCredentials()
                        : this.renderServicePrincipalCredentials()}
                </InputGroup>
            </div>;
            const columnTwo = <div key="column-two" className="right-column">
                <InputGroup>
                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("lblApplicationId")}
                        tooltipMessage={resx.get("lblApplicationId.Help")}
                        inputStyle={{ margin: "0" }}
                        error={this.state.error.ApplicationId}
                        errorMessage={resx.get("lblApplicationId.Error")}
                        value={this.state.workspaceDetail.ApplicationId}
                        onChange={this.onSettingChange.bind(this, "ApplicationId")}
                    />
                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("lblWorkspaceId")}
                        tooltipMessage={resx.get("lblWorkspaceId.Help")}
                        inputStyle={{ margin: "0" }}
                        error={this.state.error.WorkspaceId}
                        errorMessage={resx.get("lblWorkspaceId.Error")}
                        value={this.state.workspaceDetail.WorkspaceId}
                        onChange={this.onSettingChange.bind(this, "WorkspaceId")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("lblContentPageUrl")}
                        tooltipMessage={resx.get("lblContentPageUrl.Help")}
                        inputStyle={{ margin: "0" }}
                        error={this.state.error.ContentPageUrl}
                        errorMessage={resx.get("lblContentPageUrl.Error")}
                        value={this.state.workspaceDetail.ContentPageUrl}
                        onChange={this.onSettingChange.bind(this, "ContentPageUrl")}
                    />      

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("lblDisabledCapacityMessage")}
                        tooltipMessage={resx.get("lblDisabledCapacityMessage.Help")}
                        inputStyle={{ margin: "0" }}
                        error={this.state.error.DisabledCapacityMessage}
                        errorMessage={resx.get("lblDisabledCapacityMessage.Error")}
                        value={this.state.workspaceDetail.DisabledCapacityMessage}
                        onChange={this.onSettingChange.bind(this, "DisabledCapacityMessage")}
                    />                                    

                </InputGroup>
            </div>;

            return (
                <div className="workspace-editor">
                    <GridSystem numberOfColumns={2}>{[columnOne, columnTwo]}</GridSystem>
                    <div className="editor-buttons-box">
                        <Button
                            type="secondary"
                            onClick={this.onCancel.bind(this)}>
                            {resx.get("Cancel")}
                        </Button>
                        <Button
                            type="primary"
                            onClick={this.onSave.bind(this)}>
                            {resx.get("SaveSettings")}
                        </Button>
                    </div>
                </div>
            );
        }
        else return <div />;
    }
}

WorkspaceEditor.propTypes = {
    dispatch: PropTypes.func.isRequired,
    workspaceDetail: PropTypes.object,
    settingsGroupName: PropTypes.string,
    authenticationType: PropTypes.string,
    username: PropTypes.string,
    password: PropTypes.string,
    applicationId: PropTypes.string,
    workspaceId: PropTypes.string,
    servicePrincipalApplicationId: PropTypes.string,
    servicePrincipalApplicationSecret: PropTypes.string,
    servicePrincipalTenant: PropTypes.string,
    contentPageUrl: PropTypes.string,
    disabledCapacityMessage: PropTypes.string,
    inheritPermissions: PropTypes.bool,
    Collapse: PropTypes.func,
    onUpdate: PropTypes.func,
    id: PropTypes.string,
    workspaceClientModified: PropTypes.bool,
    onValidate: PropTypes.func
};

function mapStateToProps() {
    return {
        // profileMappingDetail: state.siteBehavior.aliasDetail,
        // profileMappingClientModified: state.siteBehavior.siteAliasClientModified
    };
}

export default connect(mapStateToProps)(WorkspaceEditor);