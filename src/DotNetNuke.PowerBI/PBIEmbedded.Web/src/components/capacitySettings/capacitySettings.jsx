import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridCell, Button } from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../actions/settings";
import resx from "../../resources";
import CapacityEditor from "./capacityEditor";
import CapacityRow from "./capacityRow";
import "./capacitySettings.less";

class CapacitySettings extends Component {
    constructor() {
        super();
        this.state = {
            openId: "",
            editingCapacity: null
        };
    }

    componentDidMount() {
        this.props.dispatch(SettingsActions.getCapacities());
    }

    componentDidUpdate(prevProps) {
        // Load capacity status when capacities are loaded
        if (prevProps.capacities !== this.props.capacities && this.props.capacities) {
            this.props.capacities.forEach(capacity => {
                this.props.dispatch(SettingsActions.getCapacityStatus({
                    CapacityId: capacity.CapacityId
                }));
            });
        }
    }

    uncollapse(id) {
        this.setState({
            openId: id
        });
    }

    collapse() {
        this.setState({
            openId: "",
            editingCapacity: null
        });
    }

    toggle(id) {
        if (this.state.openId !== id) {
            this.uncollapse(id);
            // Load capacity rules when opening a capacity for editing
            if (id !== "add") {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(id)
                }));
            }
        }
    }

    onStartCapacity(capacityId) {
        this.props.dispatch(SettingsActions.startCapacity({
            CapacityId: parseInt(capacityId)
        }));
        
        setTimeout(() => {
            this.props.dispatch(SettingsActions.pollCapacityStatus({
                CapacityId: parseInt(capacityId)
            }));
        }, 5000);
    }

    onPauseCapacity(capacityId) {
        this.props.dispatch(SettingsActions.pauseCapacity({
            CapacityId: parseInt(capacityId)
        }));
        
        setTimeout(() => {
            this.props.dispatch(SettingsActions.pollCapacityStatus({
                CapacityId: parseInt(capacityId)
            }));
        }, 5000);
    }

    onDeleteRule(capacityId, ruleId) {
        const payload = {
            RuleId: ruleId
        };
        this.props.dispatch(SettingsActions.deleteCapacityRule(payload, () => {
            this.props.dispatch(SettingsActions.getCapacityRules({
                CapacityId: parseInt(capacityId)
            }));
        }));
    }

    onSaveRule(capacityId, rule, editingRule) {
        const ruleData = {
            RuleName: rule.ruleName,
            RuleDescription: rule.ruleDescription,
            IsEnabled: rule.isEnabled,
            Action: rule.action,
            ExecutionTime: rule.executionTime,
            DaysOfWeek: rule.daysOfWeek,
            TimeZoneId: rule.timeZoneId,
            CapacityId: capacityId
        };
        
        if (editingRule) {
            ruleData.RuleId = editingRule.RuleId;
            this.props.dispatch(SettingsActions.updateCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(capacityId)
                }));
            }));
        } else {
            this.props.dispatch(SettingsActions.createCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(capacityId)
                }));
            }));
        }
    }

    onAddCapacity() {
        this.setState({
            editingCapacity: {
                CapacityId: 0,
                CapacityName: "",
                CapacityDisplayName: "",
                Description: "",
                ServicePrincipalApplicationId: "",
                ServicePrincipalApplicationSecret: "",
                ServicePrincipalTenant: "",
                AzureManagementSubscriptionId: "",
                AzureManagementResourceGroup: "",
                AzureManagementCapacityName: "",
                AzureManagementPollingInterval: 60,
                DisabledCapacityMessage: "",
                IsEnabled: true
            }
        }, () => {
            this.uncollapse("add");
        });
    }

    onUpdateCapacity(capacity) {
        this.setState({
            editingCapacity: capacity
        }, () => {
            this.uncollapse(capacity.CapacityId);
        });
    }

    onDeleteCapacity(capacityId) {
        const capacity = this.props.capacities.find(c => c.CapacityId === capacityId);
        if (capacity && window.confirm(resx.get("ConfirmDeleteCapacity").replace("{0}", capacity.CapacityDisplayName))) {
            this.props.dispatch(SettingsActions.deleteCapacity({ CapacityId: capacityId }, () => {
                this.props.dispatch(SettingsActions.getCapacities());
            }));
        }
    }

    onSaveCapacity(capacity) {
        if (capacity.CapacityId === 0) {
            // Create new capacity
            this.props.dispatch(SettingsActions.createCapacity(capacity, () => {
                this.props.dispatch(SettingsActions.getCapacities());
                this.collapse();
            }));
        } else {
            // Update existing capacity
            this.props.dispatch(SettingsActions.updateCapacity(capacity, () => {
                this.props.dispatch(SettingsActions.getCapacities());
                this.collapse();
            }));
        }
    }

    renderCapacityList() {
        const {capacities, capacityStatuses, capacityRules} = this.props;
        
        if (!capacities || capacities.length === 0) {
            return (
                <div className="no-capacities-message">
                    <p>{resx.get("NoCapacitiesConfigured")}</p>
                </div>
            );
        }

        return capacities.map((capacity) => {
            const capacityStatus = capacityStatuses && capacityStatuses[capacity.CapacityId];
            const currentCapacityRules = this.state.openId === capacity.CapacityId ? capacityRules : [];
            
            return (
                <CapacityRow
                    key={capacity.CapacityId}
                    capacity={capacity}
                    openId={this.state.openId}
                    onDelete={() => this.onDeleteCapacity(capacity.CapacityId)}
                    onUpdate={(updatedCapacity) => this.onSaveCapacity(updatedCapacity)}
                    onCancel={this.collapse.bind(this)}
                    id={capacity.CapacityId}
                    capacityStatus={capacityStatus}
                    onStartCapacity={this.onStartCapacity.bind(this)}
                    onPauseCapacity={this.onPauseCapacity.bind(this)}
                    capacityRules={currentCapacityRules}
                    onDeleteRule={(ruleId) => this.onDeleteRule(capacity.CapacityId, ruleId)}
                    onSaveRule={(rule, editingRule) => this.onSaveRule(capacity.CapacityId, rule, editingRule)}
                />
            );
        });
    }

    render() {
        const {openId, editingCapacity} = this.state;

        return (
            <div className="capacity-settings-container">
                <GridCell className="capacity-settings-header">
                    <h2>{resx.get("CapacitySettings_Title")}</h2>
                    <p>{resx.get("CapacitySettings_Description")}</p>
                </GridCell>

                <GridCell className="add-capacity-button-container">
                    <Button
                        type="primary"
                        onClick={this.onAddCapacity.bind(this)}>
                        {resx.get("AddCapacity")}
                    </Button>
                </GridCell>

                <GridCell>
                    <div className="capacity-list">
                        {this.renderCapacityList()}
                    </div>
                </GridCell>

                {openId === "add" && editingCapacity && (
                    <CapacityEditor
                        capacity={editingCapacity}
                        onSave={(capacity) => this.onSaveCapacity(capacity)}
                        onCancel={this.collapse.bind(this)}
                    />
                )}
            </div>
        );
    }
}

CapacitySettings.propTypes = {
    dispatch: PropTypes.func.isRequired,
    capacities: PropTypes.array,
    loading: PropTypes.bool,
    error: PropTypes.object,
    capacityStatuses: PropTypes.object,
    capacityRules: PropTypes.array
};

function mapStateToProps(state) {
    return {
        capacities: state.capacitySettings.capacities || [],
        loading: state.capacitySettings.loading,
        error: state.capacitySettings.error,
        capacityStatuses: state.capacityManagement.capacityStatuses || {},
        capacityRules: state.capacityManagement.capacityRules || []
    };
}

export default connect(mapStateToProps)(CapacitySettings);
