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
            selectedCapacity: "",
            error: {
                selectedCapacity: false
            }
        };
    }

    componentDidMount() {
        // Load capacities instead of workspaces
        this.props.dispatch(SettingsActions.getCapacities());
    }

    componentDidUpdate(prevProps) {
        // Auto-select first capacity when loaded
        if (!prevProps.capacities && this.props.capacities && this.props.capacities.length > 0 && !this.state.selectedCapacity) {
            const firstCapacity = this.props.capacities[0];
            this.setState({ selectedCapacity: firstCapacity.CapacityId });
            this.onCapacityChange(firstCapacity.CapacityId);
        }
    }

    onCapacityChange(capacityId) {
        this.props.dispatch(SettingsActions.clearCapacityData());

        this.setState({
            selectedCapacity: capacityId,
            error: {
                selectedCapacity: !capacityId
            }
        });

        if (capacityId) {
            // Load capacity rules
            this.props.dispatch(SettingsActions.getCapacityRules({
                CapacityId: parseInt(capacityId)
            }));
        }
    }

    getCapacityOptions() {
        let options = [];

        if (this.props.capacities && Array.isArray(this.props.capacities)) {
            options = this.props.capacities.map((item) => {
                return { 
                    label: item.CapacityDisplayName, 
                    value: item.CapacityId 
                };
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
                CapacityId: parseInt(this.state.selectedCapacity)
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
            CapacityId: this.state.selectedCapacity  // Changed from SettingsId
        };
        
        if (editingRule) {
            ruleData.RuleId = editingRule.RuleId;
            this.props.dispatch(SettingsActions.updateCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(this.state.selectedCapacity)
                }));
            }));
        } else {
            this.props.dispatch(SettingsActions.createCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(this.state.selectedCapacity)
                }));
            }));
        }
    }

    render() {
        const { capacityRules, operationError } = this.props;
        const capacityOptions = this.getCapacityOptions();

        return (
            <div className="capacity-management-wrapper">
                <h2 className="capacity-management-title">{resx.get("CapacityManagement_Title")}</h2>
                <p className="capacity-management-description">{resx.get("CapacityManagement_Description")}</p>

                <div className="capacity-management-container">
                    <GridCell columnSize={100}>
                        <InputGroup>
                            <DropdownWithError
                                withLabel={true}
                                label={resx.get("SelectCapacity")}
                                tooltipMessage={resx.get("SelectCapacity_Help")}
                                error={this.state.error.selectedCapacity}
                                errorMessage={resx.get("SelectCapacity_Error")}
                                options={capacityOptions}
                                value={this.state.selectedCapacity}
                                onSelect={(option) => this.onCapacityChange(option.value)}
                                enabled={capacityOptions.length > 0}
                            />
                        </InputGroup>
                    </GridCell>

                    {this.state.selectedCapacity && (
                        <>
                            <CapacityStatus 
                                selectedCapacity={this.state.selectedCapacity}
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
    capacities: PropTypes.array,
    capacityStatus: PropTypes.object,
    capacityRules: PropTypes.array,
    capacityLoading: PropTypes.bool,
    capacityError: PropTypes.object,
    statusError: PropTypes.object,
    operationError: PropTypes.object
};

function mapStateToProps(state) {
    return {
        capacities: state.capacitySettings.capacities || [],  // Changed from workspaces
        capacityStatus: state.capacityManagement.capacityStatus,
        capacityRules: state.capacityManagement.capacityRules || [],
        capacityLoading: state.capacityManagement.loading,
        capacityError: state.capacityManagement.error,
        statusError: state.capacityManagement.statusError,
        operationError: state.capacityManagement.operationError
    };
}

export default connect(mapStateToProps)(CapacityManagement);
