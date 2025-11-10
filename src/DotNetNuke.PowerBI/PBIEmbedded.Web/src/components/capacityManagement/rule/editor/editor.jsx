import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import "./editor.less";
import { GridCell, SingleLineInputWithError, DropdownWithError, Button, InputGroup, Switch } from "@dnnsoftware/dnn-react-common";
import TimezoneSelect from "react-timezone-select";
import resx from "../../../../resources";

class CapacityRuleEditor extends Component {
    constructor(props) {
        super(props);

        this.state = {
            ruleDetail: {
                ruleId: 0,
                ruleName: "",
                ruleDescription: "",
                isEnabled: true,
                action: "Start",
                executionTime: "08:00",
                daysOfWeek: "1,2,3,4,5",
                timeZoneId: "UTC"
            },
            error: {
                ruleName: false,
                executionTime: false,
                daysOfWeek: false
            },
            triedToSubmit: false
        };
    }
    
    componentWillMount() {
        const {props} = this;

        const ruleDetail = {
            ruleId: props.ruleId || 0,
            ruleName: props.ruleName || "",
            ruleDescription: props.ruleDescription || "",
            isEnabled: props.isEnabled !== undefined ? props.isEnabled : true,
            action: props.action || "Start",
            executionTime: props.executionTime ? this.formatTimeSpan(props.executionTime) : "08:00",
            daysOfWeek: props.daysOfWeek || "1,2,3,4,5",
            timeZoneId: props.timeZoneId || "UTC"
        };

        const error = {
            ruleName: (props.ruleName === null),
            action: (props.action === null),
            executionTime: (props.executionTime === null),
            daysOfWeek: false
        };

        this.setState({
            ruleDetail: ruleDetail,
            error: error
        });
    }

    componentDidUpdate(prevProps) {
        const {props} = this;
        
        if (props.ruleId !== prevProps.ruleId || 
            props.ruleName !== prevProps.ruleName ||
            props.action !== prevProps.action) {
            
            const ruleDetail = {
                ruleId: props.ruleId || 0,
                ruleName: props.ruleName || "",
                ruleDescription: props.ruleDescription || "",
                isEnabled: props.isEnabled !== undefined ? props.isEnabled : true,
                action: props.action || "Start",
                executionTime: props.executionTime ? this.formatTimeSpan(props.executionTime) : "08:00",
                daysOfWeek: props.daysOfWeek || "1,2,3,4,5",
                timeZoneId: props.timeZoneId || "UTC"
            };

            const error = {
                ruleName: !props.ruleName || props.ruleName === "",
                action: !props.action || props.action === "",
                executionTime: !props.executionTime || props.executionTime === "",
                daysOfWeek: !props.daysOfWeek || props.daysOfWeek === ""
            };
            
            this.setState({
                ruleDetail: ruleDetail,
                triedToSubmit: false,
                error: error
            });
        }
    }

    formatTimeSpan(timeSpan) {
        if (typeof timeSpan === "string" && timeSpan.includes(":")) {
            return timeSpan.substring(0, 5);
        }
        return "08:00";
    }

    parseTimeSpan(timeString) {
        return timeString + ":00";
    }

    isValidTimeFormat(timeString) {
        const timeRegex = /^([0-1]?[0-9]|2[0-3]):([0-5][0-9])$/;
        return timeRegex.test(timeString);
    }

    onSettingChange(key, event) {
        let {state} = this;
        let ruleDetail = Object.assign({}, state.ruleDetail);
        let error = Object.assign({}, state.error);

        switch (key) {
            case "ruleName":
            case "ruleDescription":
                error[key] = event.target.value === "";
                ruleDetail[key] = event.target.value;
                break;
            case "executionTime":
                ruleDetail[key] = event.target.value;
                error[key] = event.target.value === "" || !this.isValidTimeFormat(event.target.value);
                break;
            case "daysOfWeek":
                error[key] = event.target.value === "";
                ruleDetail[key] = event.target.value;
                break;
            case "action":
                error[key] = !event || event.value === "";
                ruleDetail[key] = event ? event.value : "";
                break;
            case "timeZoneId":
                ruleDetail[key] = typeof event === "string" ? event : (event && event.value ? event.value : "UTC");
                break;
            case "isEnabled":
                ruleDetail[key] = typeof event === "boolean" ? event : event.target.checked;
                break;
            default: 
                ruleDetail[key] = typeof (event) === "object" ? event.target.value : event;
                break;
        }

        this.setState({
            ruleDetail: ruleDetail,
            triedToSubmit: false,
            error: error
        });
    }

    onDayChange(day, checked) {
        let days = this.state.ruleDetail.daysOfWeek.split(",").filter(d => d !== "").map(d => parseInt(d));
        if (checked) {
            if (!days.includes(day)) {
                days.push(day);
            }
        } else {
            days = days.filter(d => d !== day);
        }
        
        const daysOfWeek = days.sort((a, b) => a - b).join(",");
        let ruleDetail = Object.assign({}, this.state.ruleDetail);
        ruleDetail.daysOfWeek = daysOfWeek;
        
        this.setState({
            ruleDetail: ruleDetail,
            error: {
                ...this.state.error,
                daysOfWeek: daysOfWeek === ""
            }
        });
    }

    getActionOptions() {
        let options = [
            {
                label: resx.get("Capacity_Rule_Action_Start"),
                value: "Start"
            },
            {
                label: resx.get("Capacity_Rule_Action_Pause"),
                value: "Pause"
            }
        ];
        return options;
    }

    getDayNames() {
        return [
            { value: 0, text: resx.get("Capacity_Rule_Day_Sunday") },
            { value: 1, text: resx.get("Capacity_Rule_Day_Monday") },
            { value: 2, text: resx.get("Capacity_Rule_Day_Tuesday") },
            { value: 3, text: resx.get("Capacity_Rule_Day_Wednesday") },
            { value: 4, text: resx.get("Capacity_Rule_Day_Thursday") },
            { value: 5, text: resx.get("Capacity_Rule_Day_Friday") },
            { value: 6, text: resx.get("Capacity_Rule_Day_Saturday") }
        ];
    }

    getTimeZoneOptions() {
        return [
            { label: "UTC (GMT+0:00)", value: "UTC" },
            { label: "GMT-12:00 (International Date Line West)", value: "Dateline Standard Time" },
            { label: "GMT-11:00 (Coordinated Universal Time-11)", value: "UTC-11" },
            { label: "GMT-10:00 (Hawaii)", value: "Hawaiian Standard Time" },
            { label: "GMT-09:00 (Alaska)", value: "Alaskan Standard Time" },
            { label: "GMT-08:00 (Pacific Time - US & Canada)", value: "Pacific Standard Time" },
            { label: "GMT-07:00 (Mountain Time - US & Canada)", value: "Mountain Standard Time" },
            { label: "GMT-06:00 (Central Time - US & Canada)", value: "Central Standard Time" },
            { label: "GMT-05:00 (Eastern Time - US & Canada)", value: "Eastern Standard Time" },
            { label: "GMT-04:00 (Atlantic Time - Canada)", value: "Atlantic Standard Time" },
            { label: "GMT-03:30 (Newfoundland)", value: "Newfoundland Standard Time" },
            { label: "GMT-03:00 (Brasilia)", value: "E. South America Standard Time" },
            { label: "GMT-02:00 (Mid-Atlantic)", value: "UTC-02" },
            { label: "GMT-01:00 (Azores)", value: "Azores Standard Time" },
            { label: "GMT+0:00 (Dublin, Edinburgh, Lisbon, London)", value: "GMT Standard Time" },
            { label: "GMT+1:00 (Amsterdam, Berlin, Rome, Paris)", value: "W. Europe Standard Time" },
            { label: "GMT+1:00 (Brussels, Copenhagen, Madrid, Paris)", value: "Romance Standard Time" },
            { label: "GMT+1:00 (Belgrade, Bratislava, Budapest, Prague)", value: "Central Europe Standard Time" },
            { label: "GMT+2:00 (Athens, Bucharest, Istanbul)", value: "GTB Standard Time" },
            { label: "GMT+2:00 (Cairo)", value: "Egypt Standard Time" },
            { label: "GMT+2:00 (Jerusalem)", value: "Israel Standard Time" },
            { label: "GMT+3:00 (Moscow, St. Petersburg)", value: "Russian Standard Time" },
            { label: "GMT+3:30 (Tehran)", value: "Iran Standard Time" },
            { label: "GMT+4:00 (Abu Dhabi, Muscat)", value: "Arabian Standard Time" },
            { label: "GMT+4:30 (Kabul)", value: "Afghanistan Standard Time" },
            { label: "GMT+5:00 (Islamabad, Karachi)", value: "Pakistan Standard Time" },
            { label: "GMT+5:30 (Chennai, Kolkata, Mumbai, New Delhi)", value: "India Standard Time" },
            { label: "GMT+5:45 (Kathmandu)", value: "Nepal Standard Time" },
            { label: "GMT+6:00 (Astana, Dhaka)", value: "Central Asia Standard Time" },
            { label: "GMT+6:30 (Yangon)", value: "Myanmar Standard Time" },
            { label: "GMT+7:00 (Bangkok, Hanoi, Jakarta)", value: "SE Asia Standard Time" },
            { label: "GMT+8:00 (Beijing, Chongqing, Hong Kong, Urumqi)", value: "China Standard Time" },
            { label: "GMT+8:00 (Singapore)", value: "Singapore Standard Time" },
            { label: "GMT+8:00 (Perth)", value: "W. Australia Standard Time" },
            { label: "GMT+9:00 (Seoul)", value: "Korea Standard Time" },
            { label: "GMT+9:00 (Tokyo)", value: "Tokyo Standard Time" },
            { label: "GMT+9:30 (Adelaide)", value: "Cen. Australia Standard Time" },
            { label: "GMT+9:30 (Darwin)", value: "AUS Central Standard Time" },
            { label: "GMT+10:00 (Brisbane)", value: "E. Australia Standard Time" },
            { label: "GMT+10:00 (Canberra, Melbourne, Sydney)", value: "AUS Eastern Standard Time" },
            { label: "GMT+11:00 (Solomon Is., New Caledonia)", value: "Central Pacific Standard Time" },
            { label: "GMT+12:00 (Auckland, Wellington)", value: "New Zealand Standard Time" },
            { label: "GMT+12:00 (Fiji)", value: "Fiji Standard Time" },
            { label: "GMT+13:00 (Nuku'alofa)", value: "Tonga Standard Time" }
        ];
    }

    onSave() {
        const {props, state} = this;
        this.setState({
            triedToSubmit: true
        });
        
        if (state.error.ruleName 
            || state.error.action
            || state.error.executionTime
            || state.error.daysOfWeek) {
            return;
        }

        const ruleToSave = {
            ruleId: state.ruleDetail.ruleId,
            ruleName: state.ruleDetail.ruleName,
            ruleDescription: state.ruleDetail.ruleDescription,
            isEnabled: state.ruleDetail.isEnabled,
            action: state.ruleDetail.action,
            executionTime: this.parseTimeSpan(state.ruleDetail.executionTime),
            daysOfWeek: state.ruleDetail.daysOfWeek,
            timeZoneId: state.ruleDetail.timeZoneId
        };

        props.onUpdate(ruleToSave);
        props.Collapse();
    }

    onCancel() {
        const {props} = this;
        props.Collapse();
    }

    render() {
        if (this.state.ruleDetail !== undefined) {
            return (
                <div className="capacity-rule-editor">
                    <InputGroup>
                        <GridCell columnSize={60}>
                            <SingleLineInputWithError
                                withLabel={true}
                                label={resx.get("Capacity_Rule_Name")}
                                tooltipMessage={resx.get("Capacity_Rule_Name.Help")}
                                enabled={true}
                                error={this.state.error.ruleName}
                                errorMessage={resx.get("Capacity_Rule_Error_Name")}
                                value={this.state.ruleDetail.ruleName}
                                onChange={this.onSettingChange.bind(this, "ruleName")}
                            />
                        </GridCell>
                        <GridCell columnSize={40}>
                            <DropdownWithError
                                withLabel={true}
                                label={resx.get("Capacity_Rule_Action")}
                                tooltipMessage={resx.get("Capacity_Rule_Action.Help")}
                                options={this.getActionOptions()}
                                value={this.state.ruleDetail.action}
                                onSelect={this.onSettingChange.bind(this, "action")}
                            />
                        </GridCell>
                    </InputGroup>

                    <InputGroup>
                        <GridCell columnSize={100}>
                            <SingleLineInputWithError
                                withLabel={true}
                                label={resx.get("Capacity_Rule_Description")}
                                tooltipMessage={resx.get("Capacity_Rule_Description.Help")}
                                enabled={true}
                                error={false}
                                value={this.state.ruleDetail.ruleDescription}
                                onChange={this.onSettingChange.bind(this, "ruleDescription")}
                            />
                        </GridCell>
                    </InputGroup>
                    <InputGroup>
                        <GridCell columnSize={100}>
                            <SingleLineInputWithError
                                withLabel={true}
                                label={resx.get("Capacity_Rule_ExecutionTime")}
                                tooltipMessage={resx.get("Capacity_Rule_ExecutionTime.Help")}
                                type="time"
                                enabled={true}
                                error={this.state.error.executionTime && this.state.triedToSubmit}
                                errorMessage={resx.get("Capacity_Rule_Error_ExecutionTime")}
                                value={this.state.ruleDetail.executionTime}
                                onChange={this.onSettingChange.bind(this, "executionTime")}
                            />
                        </GridCell>
                        <GridCell columnSize={100}>
                            <div className="timezone-select-wrapper">
                                <label className="dnn-label">
                                    <strong>
                                        {resx.get("Capacity_Rule_TimeZone")}
                                    </strong>
                                </label>
                                <TimezoneSelect
                                    value={this.state.ruleDetail.timeZoneId}
                                    onChange={this.onSettingChange.bind(this, "timeZoneId")}
                                    labelStyle="abbrev"
                                    displayValue="GMT"
                                />
                            </div>
                        </GridCell>
                    </InputGroup>

                    <div className="days-of-week-section">
                        <label className="dnn-label">
                            <strong>
                                {resx.get("Capacity_Rule_DaysOfWeek")}
                            </strong>
                        </label>
                        <div className="days-selector">
                            {this.getDayNames().map(day => {
                                const isChecked = this.state.ruleDetail.daysOfWeek.split(",").includes(day.value.toString());
                                return (
                                    <label key={day.value} className="day-checkbox">
                                        <input
                                            type="checkbox"
                                            checked={isChecked}
                                            onChange={(e) => this.onDayChange(day.value, e.target.checked)}
                                        />
                                        <span>{day.text}</span>
                                    </label>
                                );
                            })}
                        </div>
                        {this.state.error.daysOfWeek && (
                            <span className="error-message">
                                {resx.get("Capacity_Rule_Error_DaysOfWeek")}
                            </span>
                        )}
                    </div>
                    
                    <div className="editor-buttons-box">
                        <Switch
                            withLabel={true}
                            label={resx.get("Capacity_Rule_Enabled")}
                            tooltipMessage={resx.get("Capacity_Rule_Enabled.Help")}
                            value={this.state.ruleDetail.isEnabled}
                            onChange={this.onSettingChange.bind(this, "isEnabled")}
                        />
                        
                        <div className="buttons-group">
                            <Button
                                type="secondary"
                                onClick={this.onCancel.bind(this)}>
                                {resx.get("Capacity_Rule_Button_Cancel")}
                            </Button>
                            <Button
                                type="primary"
                                onClick={this.onSave.bind(this)}>
                                {this.state.ruleDetail.ruleId === 0 ? resx.get("Capacity_Rule_Button_Create") : resx.get("Capacity_Rule_Button_Update")}
                            </Button>
                        </div>
                    </div>
                </div>
            );
        }
        else return <div />;
    }
}

CapacityRuleEditor.propTypes = {
    dispatch: PropTypes.func.isRequired,
    ruleDetail: PropTypes.object,
    ruleId: PropTypes.number,
    ruleName: PropTypes.string,
    ruleDescription: PropTypes.string,
    isEnabled: PropTypes.bool,
    action: PropTypes.string,
    executionTime: PropTypes.string,
    daysOfWeek: PropTypes.string,
    timeZoneId: PropTypes.string,
    Collapse: PropTypes.func,
    onUpdate: PropTypes.func,
    id: PropTypes.string
};

function mapStateToProps() {
    return {};
}

export default connect(mapStateToProps)(CapacityRuleEditor);
