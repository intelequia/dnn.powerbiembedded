import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridCell, GridSystem, SingleLineInputWithError, Button, SwitchBox, Select, CheckBox, Label } from "@dnnsoftware/dnn-react-common";
import CapacityManagementActions from "../../actions/capacityManagement";
import resx from "../../resources";
import "./capacityManagement.less";

class CapacityManagement extends Component {
    constructor() {
        super();
        this.state = {
            showRuleForm: false,
            editingRule: null,
            newRule: {
                ruleName: "",
                ruleDescription: "",
                isEnabled: true,
                action: "Start",
                executionTime: "08:00",
                daysOfWeek: "1,2,3,4,5", // Monday to Friday
                timeZoneId: "UTC"
            }
        };
    }

    componentDidMount() {
        if (this.props.selectedWorkspace) {
            this.props.dispatch(CapacityManagementActions.getCapacityStatus(this.props.selectedWorkspace.settingsId));
            this.props.dispatch(CapacityManagementActions.getCapacityRules(this.props.selectedWorkspace.settingsId));
        }
    }

    componentDidUpdate(prevProps) {
        if (prevProps.selectedWorkspace !== this.props.selectedWorkspace && this.props.selectedWorkspace) {
            this.props.dispatch(CapacityManagementActions.getCapacityStatus(this.props.selectedWorkspace.settingsId));
            this.props.dispatch(CapacityManagementActions.getCapacityRules(this.props.selectedWorkspace.settingsId));
        }
    }

    onRefreshStatus() {
        if (this.props.selectedWorkspace) {
            this.props.dispatch(CapacityManagementActions.getCapacityStatus(this.props.selectedWorkspace.settingsId));
        }
    }

    onStartCapacity() {
        if (this.props.selectedWorkspace) {
            this.props.dispatch(CapacityManagementActions.startCapacity(this.props.selectedWorkspace.settingsId));
        }
    }

    onPauseCapacity() {
        if (this.props.selectedWorkspace) {
            this.props.dispatch(CapacityManagementActions.pauseCapacity(this.props.selectedWorkspace.settingsId));
        }
    }

    onShowRuleForm() {
        this.setState({
            showRuleForm: true,
            editingRule: null,
            newRule: {
                ruleName: "",
                ruleDescription: "",
                isEnabled: true,
                action: "Start",
                executionTime: "08:00",
                daysOfWeek: "1,2,3,4,5",
                timeZoneId: "UTC"
            }
        });
    }

    onEditRule(rule) {
        this.setState({
            showRuleForm: true,
            editingRule: rule,
            newRule: {
                ...rule,
                executionTime: this.formatTimeSpan(rule.executionTime)
            }
        });
    }

    onDeleteRule(ruleId) {
        if (confirm("Are you sure you want to delete this rule?")) {
            this.props.dispatch(CapacityManagementActions.deleteCapacityRule(ruleId));
        }
    }

    onCancelRuleForm() {
        this.setState({
            showRuleForm: false,
            editingRule: null
        });
    }

    onSaveRule() {
        const rule = {
            ...this.state.newRule,
            settingsId: this.props.selectedWorkspace.settingsId,
            executionTime: this.parseTimeSpan(this.state.newRule.executionTime)
        };

        if (this.state.editingRule) {
            rule.ruleId = this.state.editingRule.ruleId;
            this.props.dispatch(CapacityManagementActions.updateCapacityRule(rule));
        } else {
            this.props.dispatch(CapacityManagementActions.createCapacityRule(rule));
        }

        this.setState({
            showRuleForm: false,
            editingRule: null
        });
    }

    onRuleFieldChange(field, value) {
        this.setState({
            newRule: {
                ...this.state.newRule,
                [field]: value
            }
        });
    }

    onDayChange(day, checked) {
        let days = this.state.newRule.daysOfWeek.split(",").map(d => parseInt(d));
        if (checked) {
            if (!days.includes(day)) {
                days.push(day);
            }
        } else {
            days = days.filter(d => d !== day);
        }
        this.onRuleFieldChange("daysOfWeek", days.join(","));
    }

    formatTimeSpan(timeSpan) {
        // Convert TimeSpan to HH:MM format
        if (typeof timeSpan === "string" && timeSpan.includes(":")) {
            return timeSpan.substring(0, 5); // Take only HH:MM part
        }
        return "08:00";
    }

    parseTimeSpan(timeString) {
        // Convert HH:MM to TimeSpan format
        return timeString + ":00";
    }

    getCapacityStatusText(status) {
        switch (status) {
            case "Active":
                return resx.get("CapacityRunning");
            case "Paused":
                return resx.get("CapacityPaused");
            default:
                return resx.get("CapacityUnknown");
        }
    }

    getCapacityStatusClass(status) {
        switch (status) {
            case "Active":
                return "capacity-status-running";
            case "Paused":
                return "capacity-status-paused";
            default:
                return "capacity-status-unknown";
        }
    }

    getDayNames() {
        return [
            { value: 0, text: "Sunday" },
            { value: 1, text: "Monday" },
            { value: 2, text: "Tuesday" },
            { value: 3, text: "Wednesday" },
            { value: 4, text: "Thursday" },
            { value: 5, text: "Friday" },
            { value: 6, text: "Saturday" }
        ];
    }

    render() {
        const { selectedWorkspace, capacityStatus, capacityRules, capacityLoading } = this.props;

        if (!selectedWorkspace) {
            return (
                <GridSystem>
                    <GridCell>
                        <Label>{resx.get("lblSelectedWorkspace.Text")}</Label>
                    </GridCell>
                </GridSystem>
            );
        }

        const isConfigured = selectedWorkspace.azureManagementSubscriptionId && 
                           selectedWorkspace.azureManagementResourceGroup && 
                           selectedWorkspace.azureManagementCapacityName && 
                           selectedWorkspace.azureManagementClientId && 
                           selectedWorkspace.azureManagementClientSecret && 
                           selectedWorkspace.azureManagementTenantId;

        return (
            <GridSystem>
                <GridCell>
                    <Label>{resx.get("lblCapacityStatus.Text")}</Label>
                    {!isConfigured && (
                        <div className="capacity-not-configured">
                            {resx.get("CapacityNotConfigured")}
                        </div>
                    )}
                    {isConfigured && (
                        <div className="capacity-status-section">
                            {capacityStatus && (
                                <div className="capacity-status-info">
                                    <div className={`capacity-status-indicator ${this.getCapacityStatusClass(capacityStatus.state)}`}>
                                        {this.getCapacityStatusText(capacityStatus.state)}
                                    </div>
                                    <div className="capacity-details">
                                        <div><strong>Name:</strong> {capacityStatus.displayName}</div>
                                        <div><strong>Region:</strong> {capacityStatus.region}</div>
                                        <div><strong>SKU:</strong> {capacityStatus.sku}</div>
                                    </div>
                                </div>
                            )}
                            <div className="capacity-actions">
                                <Button
                                    type="secondary"
                                    onClick={this.onRefreshStatus.bind(this)}
                                    disabled={capacityLoading}
                                >
                                    {resx.get("RefreshStatus")}
                                </Button>
                                <Button
                                    type="primary"
                                    onClick={this.onStartCapacity.bind(this)}
                                    disabled={capacityLoading || (capacityStatus && capacityStatus.state === "Active")}
                                >
                                    {resx.get("StartCapacity")}
                                </Button>
                                <Button
                                    type="secondary"
                                    onClick={this.onPauseCapacity.bind(this)}
                                    disabled={capacityLoading || (capacityStatus && capacityStatus.state === "Paused")}
                                >
                                    {resx.get("PauseCapacity")}
                                </Button>
                            </div>
                        </div>
                    )}
                </GridCell>
                <GridCell>
                    <Label>{resx.get("lblCapacityRules.Text")}</Label>
                    {isConfigured && (
                        <div className="capacity-rules-section">
                            <div className="rules-header">
                                <Button
                                    type="primary"
                                    onClick={this.onShowRuleForm.bind(this)}
                                >
                                    {resx.get("AddRule")}
                                </Button>
                            </div>
                            {capacityRules && capacityRules.length > 0 && (
                                <div className="rules-list">
                                    {capacityRules.map(rule => (
                                        <div key={rule.ruleId} className="rule-item">
                                            <div className="rule-info">
                                                <div className="rule-name">{rule.ruleName}</div>
                                                <div className="rule-details">
                                                    {rule.action} at {this.formatTimeSpan(rule.executionTime)} 
                                                    {rule.isEnabled ? "" : " (Disabled)"}
                                                </div>
                                            </div>
                                            <div className="rule-actions">
                                                <Button
                                                    type="secondary"
                                                    size="small"
                                                    onClick={() => this.onEditRule(rule)}
                                                >
                                                    {resx.get("EditRule")}
                                                </Button>
                                                <Button
                                                    type="secondary"
                                                    size="small"
                                                    onClick={() => this.onDeleteRule(rule.ruleId)}
                                                >
                                                    {resx.get("DeleteRule")}
                                                </Button>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                            {this.state.showRuleForm && (
                                <div className="rule-form">
                                    <GridSystem>
                                        <GridCell columnSize={50}>
                                            <SingleLineInputWithError
                                                label={resx.get("RuleName")}
                                                value={this.state.newRule.ruleName}
                                                onChange={(e) => this.onRuleFieldChange("ruleName", e.target.value)}
                                            />
                                        </GridCell>
                                        <GridCell columnSize={50}>
                                            <Select
                                                label={resx.get("RuleAction")}
                                                value={this.state.newRule.action}
                                                options={[
                                                    { value: "Start", text: "Start" },
                                                    { value: "Stop", text: "Stop" }
                                                ]}
                                                onSelect={(option) => this.onRuleFieldChange("action", option.value)}
                                            />
                                        </GridCell>
                                        <GridCell columnSize={50}>
                                            <SingleLineInputWithError
                                                label={resx.get("RuleTime")}
                                                type="time"
                                                value={this.state.newRule.executionTime}
                                                onChange={(e) => this.onRuleFieldChange("executionTime", e.target.value)}
                                            />
                                        </GridCell>
                                        <GridCell columnSize={50}>
                                            <SwitchBox
                                                label={resx.get("RuleEnabled")}
                                                value={this.state.newRule.isEnabled}
                                                onChange={(value) => this.onRuleFieldChange("isEnabled", value)}
                                            />
                                        </GridCell>
                                        <GridCell>
                                            <Label>{resx.get("RuleDays")}</Label>
                                            <div className="days-selector">
                                                {this.getDayNames().map(day => (
                                                    <CheckBox
                                                        key={day.value}
                                                        label={day.text}
                                                        value={this.state.newRule.daysOfWeek.split(",").includes(day.value.toString())}
                                                        onChange={(checked) => this.onDayChange(day.value, checked)}
                                                    />
                                                ))}
                                            </div>
                                        </GridCell>
                                        <GridCell>
                                            <div className="rule-form-actions">
                                                <Button
                                                    type="primary"
                                                    onClick={this.onSaveRule.bind(this)}
                                                >
                                                    {resx.get("SaveSettings")}
                                                </Button>
                                                <Button
                                                    type="secondary"
                                                    onClick={this.onCancelRuleForm.bind(this)}
                                                >
                                                    {resx.get("Cancel")}
                                                </Button>
                                            </div>
                                        </GridCell>
                                    </GridSystem>
                                </div>
                            )}
                        </div>
                    )}
                </GridCell>
            </GridSystem>
        );
    }
}

CapacityManagement.propTypes = {
    dispatch: PropTypes.func.isRequired,
    selectedWorkspace: PropTypes.object,
    capacityStatus: PropTypes.object,
    capacityRules: PropTypes.array,
    capacityLoading: PropTypes.bool
};

function mapStateToProps(state) {
    return {
        selectedWorkspace: state.settings.selectedWorkspace,
        capacityStatus: state.capacityManagement.capacityStatus,
        capacityRules: state.capacityManagement.capacityRules,
        capacityLoading: state.capacityManagement.loading
    };
}

export default connect(mapStateToProps)(CapacityManagement);