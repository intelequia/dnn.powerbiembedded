import React, { Component } from "react";
import PropTypes from "prop-types";
import { Button } from "@dnnsoftware/dnn-react-common";
import resx from "../../../resources";
import CapacityRuleEditor from "./editor/editor.jsx";
import "./capacityRules.less";

class CapacityRules extends Component {
    constructor(props) {
        super(props);
        this.state = {
            showAddEditor: false,
            editingRuleId: null,
            isCollapsed: false
        };
    }

    toggleCollapse() {
        this.setState({
            isCollapsed: !this.state.isCollapsed
        });
    }

    onShowAddEditor() {
        this.setState({
            showAddEditor: true
        });
    }

    onCloseAddEditor() {
        this.setState({
            showAddEditor: false
        });
    }

    onEditRule(ruleId) {
        this.setState({
            editingRuleId: ruleId
        });
    }

    onCancelEdit() {
        this.setState({
            editingRuleId: null
        });
    }

    onDeleteRule(ruleId) {
        if (confirm(resx.get("Capacity_Rule_Confirmation_Delete"))) {
            if (this.props.onDeleteRule) {
                this.props.onDeleteRule(ruleId);
            }
        }
    }

    onUpdateRule(rule, isNew = false) {
        if (this.props.onSaveRule) {
            const editingRule = isNew ? null : rule;
            this.props.onSaveRule(rule, editingRule);
        }
        
        if (isNew) {
            this.onCloseAddEditor();
        } else {
            this.onCancelEdit();
        }
    }

    formatTimeSpan(timeSpan) {
        if (typeof timeSpan === "string" && timeSpan.includes(":")) {
            return timeSpan.substring(0, 5);
        }
        return "08:00";
    }

    getDayName(dayNumber) {
        const days = {
            0: resx.get("Capacity_Rule_Day_Sunday"),
            1: resx.get("Capacity_Rule_Day_Monday"),
            2: resx.get("Capacity_Rule_Day_Tuesday"),
            3: resx.get("Capacity_Rule_Day_Wednesday"),
            4: resx.get("Capacity_Rule_Day_Thursday"),
            5: resx.get("Capacity_Rule_Day_Friday"),
            6: resx.get("Capacity_Rule_Day_Saturday")
        };
        return days[dayNumber] || "";
    }

    renderDayTags(daysOfWeek) {
        if (!daysOfWeek) return null;
        
        const dayNumbers = daysOfWeek.split(",").filter(d => d !== "").map(d => parseInt(d));
        
        return (
            <div className="rule-days-tags">
                {dayNumbers.map(dayNum => (
                    <span key={dayNum} className="day-tag">
                        {this.getDayName(dayNum)}
                    </span>
                ))}
            </div>
        );
    }

    render() {
        const { capacityRules } = this.props;

        return (
            <div className="capacity-rules-section">
                <div className="rules-header">
                    <div className="header-left">
                        <h2 onClick={this.toggleCollapse.bind(this)} style={{ cursor: "pointer", display: "flex", alignItems: "center", gap: "10px" }}>
                            <span className={`collapse-icon ${this.state.isCollapsed ? "collapsed" : "expanded"}`}>
                                {this.state.isCollapsed ? "▶" : "▼"}
                            </span>
                            {resx.get("Capacity_Rules_Title")}
                        </h2>
                    </div>
                    <Button
                        type="primary"
                        onClick={this.onShowAddEditor.bind(this)}
                    >
                        {resx.get("Capacity_Rules_Button_Add")}
                    </Button>
                </div>

                {!this.state.isCollapsed && (
                    <>
                        {this.state.showAddEditor && (
                            <CapacityRuleEditor
                                id="add-new-rule"
                                Collapse={this.onCloseAddEditor.bind(this)}
                                onUpdate={(rule) => this.onUpdateRule(rule, true)}
                            />
                        )}
                        
                        {capacityRules && capacityRules.length > 0 && (
                            <div className="rules-list">
                                {capacityRules.map(rule => {
                                    return (
                                        <div key={rule.RuleId} className="rule-item">
                                            {this.state.editingRuleId === rule.RuleId ? (
                                                <CapacityRuleEditor
                                                    id={`edit-rule-${rule.RuleId}`}
                                                    ruleId={rule.RuleId}
                                                    ruleName={rule.RuleName}
                                                    ruleDescription={rule.RuleDescription}
                                                    isEnabled={rule.IsEnabled}
                                                    action={rule.Action}
                                                    executionTime={rule.ExecutionTime}
                                                    daysOfWeek={rule.DaysOfWeek}
                                                    timeZoneId={rule.TimeZoneId}
                                                    Collapse={this.onCancelEdit.bind(this)}
                                                    onUpdate={(updatedRule) => this.onUpdateRule(updatedRule, false)}
                                                />
                                            ) : (
                                                <>
                                                    <div className="rule-info">
                                                        <div className="rule-name">
                                                            <span className={`rule-id-tag rule-action-${rule.Action.toLowerCase()}`}>
                                                                {rule.Action}
                                                            </span>
                                                            {rule.RuleName}
                                                        </div>
                                                        <div className="rule-details">
                                                            {rule.Action} {resx.get("Capacity_Rule_Text_At")} {this.formatTimeSpan(rule.ExecutionTime)} ({rule.TimeZoneId})
                                                            {rule.IsEnabled ? "" : ` (${resx.get("Capacity_Rule_Text_Disabled")})`}
                                                        </div>
                                                        <div className="rule-description">{rule.RuleDescription}</div>
                                                        {this.renderDayTags(rule.DaysOfWeek)}
                                                    </div>
                                                    <div className="rule-actions">
                                                        <Button
                                                            type="secondary"
                                                            size="small"
                                                            onClick={() => this.onEditRule(rule.RuleId)}
                                                        >
                                                            {resx.get("Capacity_Rule_Button_Edit")}
                                                        </Button>
                                                        <Button
                                                            type="secondary"
                                                            size="small"
                                                            onClick={() => this.onDeleteRule(rule.RuleId)}
                                                        >
                                                            {resx.get("Capacity_Rule_Button_Delete")}
                                                        </Button>
                                                    </div>
                                                </>
                                            )}
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </>
                )}
            </div>
        );
    }
}

CapacityRules.propTypes = {
    capacityRules: PropTypes.array,
    showRuleForm: PropTypes.bool,
    editingRule: PropTypes.object,
    newRule: PropTypes.object,
    onShowRuleForm: PropTypes.func,
    onEditRule: PropTypes.func,
    onDeleteRule: PropTypes.func,
    onSaveRule: PropTypes.func,
    onCancelRuleForm: PropTypes.func
};

CapacityRules.defaultProps = {
    capacityRules: [],
    showRuleForm: false,
    editingRule: null,
    newRule: {},
    onShowRuleForm: null,
    onEditRule: null,
    onDeleteRule: null,
    onSaveRule: null,
    onCancelRuleForm: null
};

export default CapacityRules;