import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridCell, GridSystem, DropdownWithError, InputGroup } from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../actions/settings";
import resx from "../../resources";
import CapacityRules from "./rule";
import CapacityStatus from "./capacityStatus";
import "./capacityManagement.less";

class CapacityManagement extends Component {
    constructor() {
        super();
        this.state = {
            selectedWorkspace: "",
            error: {
                selectedWorkspace: false,
                selectedWorkspaceInvalidCredentials: false,
            }
        };
    }

    componentDidMount() {
        this.props.dispatch(SettingsActions.getWorkspaces());

        if (this.props.selectedWorkspace) {
            this.setState({ selectedWorkspace: this.props.selectedWorkspace.settingsId });
            this.props.dispatch(SettingsActions.getCapacityRules(this.props.selectedWorkspace.settingsId));
        }
    }

    componentWillUnmount() {
    }

    componentDidUpdate(prevProps) {
        if (!prevProps.workspaces && this.props.workspaces && this.props.workspaces.length > 0 && !this.state.selectedWorkspace) {
            const firstWorkspace = this.props.workspaces[0];
            this.setState({ selectedWorkspace: firstWorkspace.SettingsId });

            this.onWorkspaceChange(firstWorkspace.SettingsId);
        }
    }

    onWorkspaceChange(workspaceId) {
        this.props.dispatch(SettingsActions.clearCapacityData());

        const workspace = this.props.workspaces.find(w => w.SettingsId === workspaceId);

        const isConfigured = this.isWorkspaceConfigured(workspace);

        this.setState({
            selectedWorkspace: workspaceId,
            error: {
                ...this.state.error,
                selectedWorkspaceInvalidCredentials: !isConfigured,
            }
        });

        if (isConfigured) {
            this.props.dispatch(SettingsActions.getCapacityRules({
                SettingsId: parseInt(workspaceId)
            }));
        }
    }

    getWorkspaceOptions() {
        let options = [];

        if (this.props.workspaces && Array.isArray(this.props.workspaces)) {
            options = this.props.workspaces.map((item) => {
                return { label: item.SettingsGroupName, value: item.SettingsId };
            });
        }
        return options;
    }

    onRuleDeleted(ruleId) {
        const payload = {
            RuleId: ruleId
        };
        this.props.dispatch(SettingsActions.deleteCapacityRule(payload, () => {
            this.props.dispatch(SettingsActions.getCapacityRules({
                SettingsId: parseInt(this.state.selectedWorkspace)
            }));
        }));
    }

    onRuleSaved(rule, editingRule) {
        const ruleData = {
            RuleName: rule.ruleName,
            RuleDescription: rule.ruleDescription,
            IsEnabled: rule.isEnabled,
            Action: rule.action,
            ExecutionTime: rule.executionTime,
            DaysOfWeek: rule.daysOfWeek,
            TimeZoneId: rule.timeZoneId,
            SettingsId: this.state.selectedWorkspace
        };
        
        if (editingRule) {
            ruleData.RuleId = editingRule.RuleId || editingRule.ruleId || rule.ruleId;
            this.props.dispatch(SettingsActions.updateCapacityRule(ruleData));
        } else {
            this.props.dispatch(SettingsActions.createCapacityRule(ruleData));
        }
    }

    isWorkspaceConfigured(workspace) {
        if (!workspace) {
            return false;
        }

        const isValidValue = (value) => {
            return value !== null && value !== undefined && value !== "" && value.toString().trim() !== "";
        };

        const result = isValidValue(workspace.AzureManagementSubscriptionId) &&
            isValidValue(workspace.AzureManagementResourceGroup) &&
            isValidValue(workspace.AzureManagementCapacityName) &&
            isValidValue(workspace.ServicePrincipalApplicationId) &&
            isValidValue(workspace.ServicePrincipalApplicationSecret) &&
            isValidValue(workspace.ServicePrincipalTenant);

        return result;
    }

    render() {
        const { capacityRules, operationError } = this.props;

        return (
            <div className="dnn-pbiembedded-capacitymanagement">
                <h1>{resx.get("Capacity_Management_Title")}</h1>
                <p>{resx.get("Capacity_Management_Description")}</p>

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
                                onSelect={(option) => this.onWorkspaceChange(option.value)}
                            />                  
                        </GridCell>
                        <GridCell columnSize={100}>
                        </GridCell>
                    </GridSystem>
                </InputGroup>

                <div className="capacity-status-section">
                    <GridCell columnSize={100}>
                        {this.state.error.selectedWorkspaceInvalidCredentials && (
                            <div className="workspace-configuration-error">
                                <div className="error-message">
                                    <strong>{resx.get("Workspace_Error_WorkspaceNotValid_Title")}</strong>
                                    <p>{resx.get("Workspace_Error_WorkspaceNotValid_Message")}</p>
                                </div>
                            </div>
                        )}
                    </GridCell>

                    {this.state.selectedWorkspace && !this.state.error.selectedWorkspaceInvalidCredentials && (
                        <>
                            <CapacityStatus 
                                selectedWorkspace={this.state.selectedWorkspace}
                                isWorkspaceConfigured={this.isWorkspaceConfigured(
                                    this.props.workspaces && this.props.workspaces.find(w => w.SettingsId === this.state.selectedWorkspace)
                                )}
                            />

                            <GridCell columnSize={100}>
                                {operationError && (
                                    <div className="capacity-operation-error">
                                        <div className="error-message">
                                            <button 
                                                className="error-close-btn"
                                                onClick={() => this.props.dispatch(SettingsActions.clearOperationError())}
                                                aria-label="Close error message"
                                            >
                                                ×
                                            </button>
                                            <strong>{resx.get("Capacity_Operation_Error_Title")}</strong>
                                            <p>{resx.get("Capacity_Operation_Error_Message")}</p>
                                            {operationError.message && (
                                                <small>{resx.get("Capacity_Operation_Error_Details")}: {operationError.message}</small>
                                            )}
                                        </div>
                                    </div>
                                )}
                            </GridCell>
                            <GridCell columnSize={100}>
                                <CapacityRules
                                    capacityRules={capacityRules || []}
                                    onDeleteRule={(ruleId) => this.onRuleDeleted(ruleId)}
                                    onSaveRule={(rule, editingRule) => this.onRuleSaved(rule, editingRule)}
                                />
                            </GridCell>
                        </>
                    )}
                </div>
            </div>
        );
    }
}

CapacityManagement.propTypes = {
    dispatch: PropTypes.func.isRequired,
    selectedWorkspace: PropTypes.object,
    workspaces: PropTypes.array,
    capacityStatus: PropTypes.object,
    capacityRules: PropTypes.array,
    capacityLoading: PropTypes.bool,
    capacityError: PropTypes.object,
    statusError: PropTypes.object,
    operationError: PropTypes.object
};

function mapStateToProps(state) {
    return {
        selectedWorkspace: state.settings.selectedWorkspace,
        workspaces: state.settings.workspaces || [],
        capacityStatus: state.capacityManagement.capacityStatus,
        capacityRules: state.capacityManagement.capacityRules || [],
        capacityLoading: state.capacityManagement.loading,
        capacityError: state.capacityManagement.error,
        statusError: state.capacityManagement.statusError,
        operationError: state.capacityManagement.operationError
    };
}

export default connect(mapStateToProps)(CapacityManagement);