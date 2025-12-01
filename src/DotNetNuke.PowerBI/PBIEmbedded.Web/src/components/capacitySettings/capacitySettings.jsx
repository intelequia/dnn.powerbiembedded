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
        const {capacities} = this.props;
        
        if (!capacities || capacities.length === 0) {
            return (
                <div className="no-capacities-message">
                    <p>{resx.get("NoCapacitiesConfigured")}</p>
                </div>
            );
        }

        return capacities.map((capacity) => {
            return (
                <CapacityRow
                    key={capacity.CapacityId}
                    capacity={capacity}
                    openId={this.state.openId}
                    onDelete={() => this.onDeleteCapacity(capacity.CapacityId)}
                    onUpdate={(updatedCapacity) => this.onSaveCapacity(updatedCapacity)}
                    onCancel={this.collapse.bind(this)}
                    id={capacity.CapacityId}
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
    error: PropTypes.object
};

function mapStateToProps(state) {
    return {
        capacities: state.capacitySettings.capacities || [],
        loading: state.capacitySettings.loading,
        error: state.capacitySettings.error
    };
}

export default connect(mapStateToProps)(CapacitySettings);
