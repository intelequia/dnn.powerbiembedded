import React, { Component } from "react";
import PropTypes from "prop-types";
import { GridCell, GridSystem, SingleLineInputWithError, Button, InputGroup, Switch, DnnTabs as Tabs } from "@dnnsoftware/dnn-react-common";
import resx from "../../resources";
import CapacityRules from "../capacityManagement/rule";
import "./capacitySettings.less";

class CapacityEditor extends Component {
    constructor(props) {
        super(props);
        this.state = {
            capacity: props.capacity || {},
            errors: {},
            triedToSubmit: false,
            selectedTab: 0
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.capacity !== this.props.capacity) {
            this.setState({
                capacity: this.props.capacity,
                errors: {},
                triedToSubmit: false,
                selectedTab: 0
            });
        }
    }

    onSelectTab(index) {
        this.setState({
            selectedTab: index
        });
    }

    onSettingChange(key, event) {
        const { capacity, errors } = this.state;
        const value = typeof event === "object" && event.target ? event.target.value : event;

        const updatedCapacity = {
            ...capacity,
            [key]: value
        };

        // Validate the field
        const updatedErrors = { ...errors };
        if (this.isRequired(key) && !value) {
            updatedErrors[key] = true;
        } else if ((key === "ServicePrincipalApplicationId" || key === "ServicePrincipalTenant") && value && !this.isValidGuid(value)) {
            updatedErrors[key] = true;
        } else {
            delete updatedErrors[key];
        }

        this.setState({
            capacity: updatedCapacity,
            errors: updatedErrors
        });
    }

    onToggle(key) {
        const { capacity } = this.state;
        this.setState({
            capacity: {
                ...capacity,
                [key]: !capacity[key]
            }
        });
    }

    isRequired(key) {
        const requiredFields = [
            "CapacityName",
            "CapacityDisplayName",
            "ServicePrincipalApplicationId",
            "ServicePrincipalApplicationSecret",
            "ServicePrincipalTenant",
            "AzureManagementSubscriptionId",
            "AzureManagementResourceGroup",
            "AzureManagementCapacityName"
        ];
        return requiredFields.includes(key);
    }

    isValidGuid(value) {
        if (!value) return false;
        const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
        return guidRegex.test(value);
    }

    validateForm() {
        const { capacity } = this.state;
        const errors = {};

        // Validate all required fields
        if (!capacity.CapacityName) errors.CapacityName = true;
        if (!capacity.CapacityDisplayName) errors.CapacityDisplayName = true;
        
        // Validate ServicePrincipalApplicationId as required GUID
        if (!capacity.ServicePrincipalApplicationId) {
            errors.ServicePrincipalApplicationId = true;
        } else if (!this.isValidGuid(capacity.ServicePrincipalApplicationId)) {
            errors.ServicePrincipalApplicationId = true;
        }
        
        if (!capacity.ServicePrincipalApplicationSecret) errors.ServicePrincipalApplicationSecret = true;
        
        // Validate ServicePrincipalTenant as required GUID
        if (!capacity.ServicePrincipalTenant) {
            errors.ServicePrincipalTenant = true;
        } else if (!this.isValidGuid(capacity.ServicePrincipalTenant)) {
            errors.ServicePrincipalTenant = true;
        }
        
        if (!capacity.AzureManagementSubscriptionId) errors.AzureManagementSubscriptionId = true;
        if (!capacity.AzureManagementResourceGroup) errors.AzureManagementResourceGroup = true;
        if (!capacity.AzureManagementCapacityName) errors.AzureManagementCapacityName = true;

        // Validate polling interval
        const pollingInterval = parseInt(capacity.AzureManagementPollingInterval, 10);
        if (isNaN(pollingInterval) || pollingInterval < 1) {
            errors.AzureManagementPollingInterval = true;
        }

        return errors;
    }

    onSave() {
        this.setState({ triedToSubmit: true });

        const errors = this.validateForm();
        if (Object.keys(errors).length > 0) {
            this.setState({ errors });
            return;
        }

        this.props.onSave(this.state.capacity);
    }

    onCancel() {
        this.props.onCancel();
    }

    render() {
        const { capacity, errors, triedToSubmit, selectedTab } = this.state;
        const isNew = capacity.CapacityId === 0;

        const columnOne = (
            <div key="column-one" className="left-column">
                <InputGroup>
                    <h3>{resx.get("CapacitySettings_BasicInfo")}</h3>
                    
                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("CapacityName")}
                        enabled={true}
                        error={errors.CapacityName && triedToSubmit}
                        errorMessage={resx.get("CapacityName_Error")}
                        tooltipMessage={resx.get("CapacityName_Help")}
                        value={capacity.CapacityName || ""}
                        onChange={this.onSettingChange.bind(this, "CapacityName")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("CapacityDisplayName")}
                        enabled={true}
                        error={errors.CapacityDisplayName && triedToSubmit}
                        errorMessage={resx.get("CapacityDisplayName_Error")}
                        tooltipMessage={resx.get("CapacityDisplayName_Help")}
                        value={capacity.CapacityDisplayName || ""}
                        onChange={this.onSettingChange.bind(this, "CapacityDisplayName")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("Description")}
                        enabled={true}
                        tooltipMessage={resx.get("Description_Help")}
                        value={capacity.Description || ""}
                        onChange={this.onSettingChange.bind(this, "Description")}
                    />

                    <h3>{resx.get("CapacitySettings_Authentication")}</h3>

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("ServicePrincipalApplicationId")}
                        enabled={true}
                        error={errors.ServicePrincipalApplicationId && triedToSubmit}
                        errorMessage={resx.get("ServicePrincipalApplicationId_Error")}
                        tooltipMessage={resx.get("ServicePrincipalApplicationId_Help")}
                        value={capacity.ServicePrincipalApplicationId || ""}
                        onChange={this.onSettingChange.bind(this, "ServicePrincipalApplicationId")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("ServicePrincipalApplicationSecret")}
                        type="password"
                        enabled={true}
                        error={errors.ServicePrincipalApplicationSecret && triedToSubmit}
                        errorMessage={resx.get("ServicePrincipalApplicationSecret_Error")}
                        tooltipMessage={resx.get("ServicePrincipalApplicationSecret_Help")}
                        value={capacity.ServicePrincipalApplicationSecret || ""}
                        autoComplete="new-password"
                        onChange={this.onSettingChange.bind(this, "ServicePrincipalApplicationSecret")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("ServicePrincipalTenant")}
                        enabled={true}
                        error={errors.ServicePrincipalTenant && triedToSubmit}
                        errorMessage={resx.get("ServicePrincipalTenant_Error")}
                        tooltipMessage={resx.get("ServicePrincipalTenant_Help")}
                        value={capacity.ServicePrincipalTenant || ""}
                        onChange={this.onSettingChange.bind(this, "ServicePrincipalTenant")}
                    />
                </InputGroup>
            </div>
        );

        const columnTwo = (
            <div key="column-two" className="right-column">
                <InputGroup>
                    <h3>{resx.get("CapacitySettings_AzureManagement")}</h3>

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("AzureManagementSubscriptionId")}
                        enabled={true}
                        error={errors.AzureManagementSubscriptionId && triedToSubmit}
                        errorMessage={resx.get("AzureManagementSubscriptionId_Error")}
                        tooltipMessage={resx.get("AzureManagementSubscriptionId_Help")}
                        value={capacity.AzureManagementSubscriptionId || ""}
                        onChange={this.onSettingChange.bind(this, "AzureManagementSubscriptionId")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("AzureManagementResourceGroup")}
                        enabled={true}
                        error={errors.AzureManagementResourceGroup && triedToSubmit}
                        errorMessage={resx.get("AzureManagementResourceGroup_Error")}
                        tooltipMessage={resx.get("AzureManagementResourceGroup_Help")}
                        value={capacity.AzureManagementResourceGroup || ""}
                        onChange={this.onSettingChange.bind(this, "AzureManagementResourceGroup")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("AzureManagementCapacityName")}
                        enabled={true}
                        error={errors.AzureManagementCapacityName && triedToSubmit}
                        errorMessage={resx.get("AzureManagementCapacityName_Error")}
                        tooltipMessage={resx.get("AzureManagementCapacityName_Help")}
                        value={capacity.AzureManagementCapacityName || ""}
                        onChange={this.onSettingChange.bind(this, "AzureManagementCapacityName")}
                    />

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("AzureManagementPollingInterval")}
                        type="number"
                        enabled={true}
                        error={errors.AzureManagementPollingInterval && triedToSubmit}
                        errorMessage={resx.get("AzureManagementPollingInterval_Error")}
                        tooltipMessage={resx.get("AzureManagementPollingInterval_Help")}
                        value={capacity.AzureManagementPollingInterval || 60}
                        onChange={this.onSettingChange.bind(this, "AzureManagementPollingInterval")}
                        min={1}
                    />

                    <h3>{resx.get("CapacitySettings_Advanced")}</h3>

                    <SingleLineInputWithError
                        withLabel={true}
                        label={resx.get("DisabledCapacityMessage")}
                        enabled={true}
                        tooltipMessage={resx.get("DisabledCapacityMessage_Help")}
                        value={capacity.DisabledCapacityMessage || ""}
                        onChange={this.onSettingChange.bind(this, "DisabledCapacityMessage")}
                    />

                    <div className="capacity-enabled-toggle">
                        <label>{resx.get("IsEnabled")}</label>
                        <Switch
                            value={capacity.IsEnabled !== false}
                            onChange={this.onToggle.bind(this, "IsEnabled")}
                        />
                    </div>
                </InputGroup>
            </div>
        );

        const generalSettingsTab = (
            <div className="capacity-editor-body">
                <GridSystem numberOfColumns={2}>{[columnOne, columnTwo]}</GridSystem>
            </div>
        );

        const rulesTab = !isNew ? (
            <div className="capacity-rules-tab">
                <CapacityRules
                    capacityRules={this.props.capacityRules || []}
                    onDeleteRule={this.props.onDeleteRule}
                    onSaveRule={this.props.onSaveRule}
                />
            </div>
        ) : null;

        const editorContent = (
            <>
                {isNew ? (
                    generalSettingsTab
                ) : (
                    <Tabs
                        onSelect={this.onSelectTab.bind(this)}
                        selectedIndex={selectedTab}
                        tabHeaders={[resx.get("GeneralSettings"), resx.get("Capacity_Rules_Title")]}>
                        {generalSettingsTab}
                        {rulesTab}
                    </Tabs>
                )}
                <div className="capacity-editor-footer">
                    <Button
                        type="secondary"
                        onClick={this.onCancel.bind(this)}>
                        {resx.get("Cancel")}
                    </Button>
                    <Button
                        type="primary"
                        onClick={this.onSave.bind(this)}>
                        {resx.get("Save")}
                    </Button>
                </div>
            </>
        );

        // Inline mode (for collapsible)
        if (this.props.inline) {
            return (
                <div className="capacity-editor-inline">
                    {editorContent}
                </div>
            );
        }

        // Modal mode (for adding new capacity)
        return (
            <div className="capacity-editor-modal">
                <div className="capacity-editor-overlay" onClick={this.onCancel.bind(this)} />
                <div className="capacity-editor-content">
                    <div className="capacity-editor-header">
                        <h2>{isNew ? resx.get("AddCapacity") : resx.get("EditCapacity")}</h2>
                        <button className="close-button" onClick={this.onCancel.bind(this)}>×</button>
                    </div>
                    {editorContent}
                </div>
            </div>
        );
    }
}

CapacityEditor.propTypes = {
    capacity: PropTypes.object.isRequired,
    onSave: PropTypes.func.isRequired,
    onCancel: PropTypes.func.isRequired,
    inline: PropTypes.bool,
    capacityRules: PropTypes.array,
    onDeleteRule: PropTypes.func,
    onSaveRule: PropTypes.func
};

export default CapacityEditor;
