import React, {Component} from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import SettingsActions from "../../../actions/settings";
import WorkspaceRow from "./workspaceRow";
import WorkspaceEditor from "./workspaceEditor";
import { Collapsible } from "@dnnsoftware/dnn-react-common";
import "./style.less";
import { SvgIcons } from "@dnnsoftware/dnn-react-common";
import utils from "../../../utils";
import resx from "../../../resources";

class Workspaces extends Component {

    constructor() {
        super();
        this.state = {
            openId: "",
            tableFields: [],
            error: {
                workspaceSettings: false
            }
        };
    }
    UNSAFE_componentWillMount() {
        const {props} = this;

        props.dispatch(SettingsActions.getWorkspaces());
    }

    UNSAFE_componentWillReceiveProps(nextProps) {
        const {state} = this;

        state.error["workspaceSettings"] = (nextProps.workspaceSettings === null);
    }

    onValidateWorkspaceSetting(workspaceSettingsDetail, event) {
        if (event.target.value === "")
            return false;
        if (workspaceSettingsDetail.SettingsGroupName !== event.target.value) {
            // The PropertyName of this row has changed. Let's see if that property has already been mapped
            if (this.props.workspaces.find(p => p.SettingsId !== workspaceSettingsDetail.SettingsId && p.SettingsGroupName === event.target.value) !== undefined) {
                return false; // Not valid; it's already in the list
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    }
    onUpdateWorkspaceSetting(workspaceSettingsDetail) {
        const {props} = this;

        let payload = {
            workspaceSettingsDetail: workspaceSettingsDetail
        };
        props.dispatch(SettingsActions.updateWorkspace(payload, () => {
            utils.utilities.notify(resx.get("WorkspaceUpdateSuccess"));
            this.collapse();
            props.dispatch(SettingsActions.getWorkspaces());
        }, (error) => {
            const errorMessage = JSON.parse(error.responseText);
            utils.utilities.notifyError(errorMessage.Message);
        }));
    }

    onDeleteWorkspaceSetting(settingsId) {
        const {props} = this;
        utils.utilities.confirm(resx.get("WorkspaceSettingDeletedWarning"), resx.get("Yes"), resx.get("No"), () => {            
            let payload = {
                SettingsId: settingsId
            };
            props.dispatch(SettingsActions.deleteWorkspace(payload, () => {
                utils.utilities.notify(resx.get("WorkspaceDeleteSuccess"));
                this.collapse();
                props.dispatch(SettingsActions.getWorkspaces());
            }, (error) => {
                const errorMessage = JSON.parse(error.responseText);
                utils.utilities.notifyError(errorMessage.Message);
            }));
        });
    }

    onClickCancel() {
        utils.utilities.closePersonaBar();
    }

    /* eslint-disable react/no-did-update-set-state */
    componentDidUpdate(prevProps) {
        const {props} = this;
        if (props !== prevProps) {
            let tableFields = [];
            if (tableFields.length === 0) {
                tableFields.push({ "name": resx.get("Workspaces.Id.Header"), "id": "SettingsId" });
                tableFields.push({ "name": resx.get("Workspaces.GroupId.Header"), "id": "SettingsGroupId" });
                tableFields.push({ "name": resx.get("Workspaces.GroupName.Header"), "id": "SettingsGroupName" });
                tableFields.push({ "name": resx.get("Workspaces.AuthenticationType.Header"), "id": "AuthenticationType" });
                tableFields.push({ "name": resx.get("Workspaces.ContentPage.Header"), "id": "ContentPageUrl" });
            }
            this.setState({tableFields});
        }
    }

    uncollapse(id) {
        this.setState({
            openId: id
        });
    }

    collapse() {
        if (this.state.openId !== "") {
            this.setState({
                openId: ""
            });
        }
    }

    toggle(openId) {
        if (openId !== "") {
            this.uncollapse(openId);
        }
    }

    renderHeader() {
        let tableHeaders = this.state.tableFields.map((field) => {
            let className = "header-" + field.id;
            return <div className={className} key={"header-" + field.id}>
                <span>{field.name}&nbsp; </span>
            </div>;
        });
        return <div className="header-row">{tableHeaders}</div>;
    }

    renderedWorkspaces() {
        let i = 0;
        if (this.props.workspaces) {
            return this.props.workspaces
                .slice().sort((a, b) => {
                    return a.SettingsGroupName.localeCompare(b.SettingsGroupName);
                }).map((item, index) => {
                    let id = "row-" + i++;
                    let settingsId = item.SettingsId;
                    return (
                        <WorkspaceRow
                            settingsId={settingsId}
                            settingsGroupId={item.SettingsGroupId}
                            settingsGroupName={item.SettingsGroupName}
                            authenticationType={item.AuthenticationType}
                            contentPageUrl={item.ContentPageUrl}
                            index={index}
                            key={"settings-" + index}
                            closeOnClick={true}
                            openId={this.state.openId}
                            OpenCollapse={this.toggle.bind(this)}
                            Collapse={this.collapse.bind(this)}
                            onDelete={this.onDeleteWorkspaceSetting.bind(this, settingsId)}
                            id={id}>
                            <WorkspaceEditor
                                workspaceDetail={item}
                                settingsId={settingsId}
                                settingsGroupId={item.SettingsGroupId}
                                settingsGroupName={item.SettingsGroupName}
                                authenticationType={item.AuthenticationType}
                                username={item.Username}
                                password={item.Password}
                                servicePrincipalApplicationId={item.ServicePrincipalApplicationId}
                                servicePrincipalApplicationSecret={item.ServicePrincipalApplicationSecret}
                                servicePrincipalTenant={item.ServicePrincipalTenant}
                                applicationId={item.ApplicationId}
                                workspaceId={item.WorkspaceId}
                                contentPageUrl={item.ContentPageUrl}
                                disabledCapacityMessage={item.DisabledCapacityMessage}
                                inheritPermissions={item.InheritPermissions}
                                Collapse={this.collapse.bind(this)}
                                onUpdate={this.onUpdateWorkspaceSetting.bind(this)}
                                onValidate={this.onValidateWorkspaceSetting.bind(this)}
                                id={id}
                                openId={this.state.openId} />
                        </WorkspaceRow>
                    );
                });
        }
    }

    /* eslint-disable react/no-danger */
    render() {
        let opened = (this.state.openId === "add");
        let newWorkspaceDetail = {
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
        };
        return (
            <div className="dnn-pbiEmbedded-workspaces">
                <div className="workspaces-items">
                    <div className="AddItemRow">
                        <div className="sectionTitle">{resx.get("lblWorkspaces")}</div>
                        <div className={opened ? "AddItemBox-active" : "AddItemBox"} onClick={this.toggle.bind(this, opened ? "" : "add")}>
                            <div className="add-icon" dangerouslySetInnerHTML={{ __html: SvgIcons.AddIcon }}>
                            </div> {resx.get("cmdAddWorkspace")}
                        </div>
                    </div>
                    <div className="workspaces-items-grid">
                        {this.renderHeader()}
                        <Collapsible isOpened={opened} style={{width: "100%", overflow: opened ? "visible" : "hidden"}}>
                            <WorkspaceRow
                                settingsGroupName={"-"}
                                authenticationType={"-"}
                                contentPageUrl={"-"}
                                deletable={false}
                                editable={false}
                                index={"add"}
                                key={"profileItem-add"}
                                closeOnClick={true}
                                openId={this.state.openId}
                                OpenCollapse={this.toggle.bind(this)}
                                Collapse={this.collapse.bind(this)}
                                onDelete={this.onDeleteWorkspaceSetting.bind(this)}
                                id={"add"}>
                                <WorkspaceEditor
                                    workspaceDetail={newWorkspaceDetail}
                                    Collapse={this.collapse.bind(this)}
                                    authenticationType={"MasterUser"}
                                    onUpdate={this.onUpdateWorkspaceSetting.bind(this)}
                                    onValidate={this.onValidateWorkspaceSetting.bind(this)}
                                    id={"add"}
                                    openId={this.state.openId} />
                            </WorkspaceRow>
                        </Collapsible>
                        {this.renderedWorkspaces()}
                    </div>
                </div>
            </div>
        );
    }
}

Workspaces.propTypes = {
    workspaces: PropTypes.array
};


function mapStateToProps(state) {
    return {
        workspaces: state.settings.workspaces
    };
}

export default connect(mapStateToProps)(Workspaces);