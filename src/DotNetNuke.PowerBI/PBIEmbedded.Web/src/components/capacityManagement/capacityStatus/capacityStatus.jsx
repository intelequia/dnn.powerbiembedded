import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridCell, Button } from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../../actions/settings";
import resx from "../../../resources";
import CapacityState from "./capacityState";
import "./capacityStatus.less";

class CapacityStatus extends Component {
    constructor() {
        super();
        this.state = {
            localLoading: false
        };
        this.pollingInterval = null;
        this.pollingTimeout = null;
        this.lastLoadTimestamp = null;
        this.isPollingActive = false;
    }

    componentDidMount() {
        if (this.props.selectedWorkspace && this.props.isWorkspaceConfigured) {
            this.loadCapacityStatus();
            this.startPolling();
        }
    }

    componentDidUpdate(prevProps) {
        if (prevProps.selectedWorkspace !== this.props.selectedWorkspace) {
            if (this.props.selectedWorkspace && this.props.isWorkspaceConfigured) {
                this.loadCapacityStatus();
                this.startPolling();
            } else {
                this.stopPolling();
            }
        } else if (this.props.capacityStatus && this.props.capacityStatus.State) {
            if (this.isTransitionalState(this.props.capacityStatus.State) && !this.isPollingActive) {
                this.startPolling();
            }
        }
    }

    componentWillUnmount() {
        this.stopPolling();
    }

    loadCapacityStatus(showLoading = true) {
        if (this.state.localLoading) {
            return;
        }

        if (showLoading) {
            this.setState({ localLoading: true });
        }
        
        this.props.dispatch(SettingsActions.pollCapacityStatus({
            SettingsId: parseInt(this.props.selectedWorkspace)
        }, () => {
            this.setState({ localLoading: false });
            this.lastLoadTimestamp = Date.now();
        }));
    }

    startPolling() {
        if (this.isPollingActive) {
            return;
        }

        this.stopPolling();
        this.isPollingActive = true;
        
        this.pollingInterval = setInterval(() => {
            if (this.isFinalState(this.props.capacityStatus && this.props.capacityStatus.State)) {
                this.stopPolling();
                return;
            }
            
            if (this.state.localLoading) {
                return;
            }
            
            if (this.lastLoadTimestamp) {
                const timeSinceLastLoad = Date.now() - this.lastLoadTimestamp;
                if (timeSinceLastLoad < 5000) {
                    return;
                }
            }
            
            if (this.props.selectedWorkspace && this.props.isWorkspaceConfigured) {
                this.loadCapacityStatus(false);
            }
        }, 5000);

        this.pollingTimeout = setTimeout(() => {
            this.stopPolling();
        }, 120000);
    }

    stopPolling() {
        this.isPollingActive = false;
        
        if (this.pollingInterval) {
            clearInterval(this.pollingInterval);
            this.pollingInterval = null;
        }
        if (this.pollingTimeout) {
            clearTimeout(this.pollingTimeout);
            this.pollingTimeout = null;
        }
    }

    onRefreshStatus() {
        this.loadCapacityStatus(true);
    }

    onStartCapacity() {
        this.stopPolling();
        
        this.props.dispatch(SettingsActions.startCapacity({
            SettingsId: parseInt(this.props.selectedWorkspace)
        }));
        
        setTimeout(() => {
            this.loadCapacityStatus(true);
            this.startPolling();
        }, 5000);
    }

    onPauseCapacity() {
        this.stopPolling();
        
        this.props.dispatch(SettingsActions.pauseCapacity({
            SettingsId: parseInt(this.props.selectedWorkspace)
        }));
        
        setTimeout(() => {
            this.loadCapacityStatus(true);
            this.startPolling();
        }, 5000);
    }

    getCapacityStatusText(status) {
        if (!status) {
            return resx.get("Capacity_Status_Text_Unknown");
        }
        
        const statusLower = status.toLowerCase();
        
        switch (statusLower) {
            case CapacityState.ACTIVE:
            case CapacityState.SUCCEEDED:
                return resx.get("Capacity_Status_Text_Running");
            case CapacityState.RESUMING:
                return resx.get("Capacity_Status_Text_Resuming");
            case CapacityState.PRESUSPENDED:
                return resx.get("Capacity_Status_Text_PreSuspended");
            case CapacityState.PAUSING:
                return resx.get("Capacity_Status_Text_Pausing");
            case CapacityState.SUSPENDED:
            case CapacityState.PAUSED:
                return resx.get("Capacity_Status_Text_Paused");
            case CapacityState.DELETING:
                return resx.get("Capacity_Status_Text_Deleting");
            case CapacityState.DELETED:
                return resx.get("Capacity_Status_Text_Deleted");
            case CapacityState.PROVISIONING:
                return resx.get("Capacity_Status_Text_Provisioning");
            case CapacityState.UPDATING_SKU:
                return resx.get("Capacity_Status_Text_Updating");
            case CapacityState.NOT_ACTIVATED:
                return resx.get("Capacity_Status_Text_NotActivated");
            case CapacityState.PROVISION_FAILED:
                return resx.get("Capacity_Status_Text_ProvisionFailed");
            case CapacityState.INVALID:
                return resx.get("Capacity_Status_Text_Invalid");
            default:
                return resx.get("Capacity_Status_Text_Unknown");
        }
    }

    getCapacityStatusClass(status) {
        if (!status) {
            return "capacity-status-unknown";
        }
        
        const statusLower = status.toLowerCase();
        
        switch (statusLower) {
            case CapacityState.ACTIVE:
            case CapacityState.SUCCEEDED:
                return "capacity-status-running";
            case CapacityState.RESUMING:
                return "capacity-status-resuming";
            case CapacityState.SUSPENDED:
            case CapacityState.PRESUSPENDED:
            case CapacityState.PROVISIONING:
            case CapacityState.NOT_ACTIVATED:
            case CapacityState.INVALID:
                return "capacity-status-orange";
            case CapacityState.PAUSING:
                return "capacity-status-pausing";
            case CapacityState.PAUSED:
                return "capacity-status-paused";
            case CapacityState.DELETING:
                return "capacity-status-deleting";
            case CapacityState.DELETED:
            case CapacityState.PROVISION_FAILED:
                return "capacity-status-deleted";
            case CapacityState.UPDATING_SKU:
                return "capacity-status-updating";
            default:
                return "capacity-status-unknown";
        }
    }

    canStart(status) {
        if (!status) return false;
        const statusLower = status.toLowerCase();
        return statusLower === CapacityState.PAUSED;
    }

    canPause(status) {
        if (!status) return false;
        const statusLower = status.toLowerCase();
        return statusLower === CapacityState.ACTIVE || statusLower === CapacityState.SUCCEEDED;
    }

    isTransitionalState(status) {
        if (!status) return false;
        const statusLower = status.toLowerCase();
        return statusLower === CapacityState.RESUMING || statusLower === CapacityState.PAUSING;
    }

    isFinalState(status) {
        if (!status) return false;
        const statusLower = status.toLowerCase();
        return statusLower === CapacityState.ACTIVE || 
               statusLower === CapacityState.SUCCEEDED || 
               statusLower === CapacityState.PAUSED;
    }

    render() {
        const { capacityStatus, statusError } = this.props;
        const { localLoading } = this.state;

        return (
            <>
                <GridCell columnSize={60}>
                    <h2>Capacity Status</h2>
                    {localLoading && !capacityStatus ? (
                        <div className="capacity-loading">
                            <div className="spinner-container">
                                <div className="spinner"></div>
                                <p>{resx.get("Capacity_Status_Message_Loading")}</p>
                            </div>
                        </div>
                    ) : statusError ? (
                        <div className="capacity-error">
                            <div className="error-message">
                                <strong>{resx.get("Capacity_Status_Error_Title")}</strong>
                                <p>{resx.get("Capacity_Status_Error_Message")}</p>
                                {statusError.message && (
                                    <small>{resx.get("Capacity_Status_Error_Details")}: {statusError.message}</small>
                                )}
                            </div>
                        </div>
                    ) : capacityStatus ? (
                        <div className="capacity-status-info">
                            <div className={`capacity-status-indicator ${this.getCapacityStatusClass(capacityStatus.State)}`}>
                                {this.getCapacityStatusText(capacityStatus.State)}
                            </div>
                            <div className="capacity-details">
                                <div><strong>{resx.get("Capacity_Status_Label_DisplayName")}</strong>: {capacityStatus.DisplayName}</div>
                                <div><strong>{resx.get("Capacity_Status_Label_Region")}</strong>: {capacityStatus.Region}</div>
                                <div><strong>{resx.get("Capacity_Status_Label_SKU")}</strong>: {capacityStatus.Sku}</div>
                            </div>
                        </div>
                    ) : (
                        <div className="capacity-no-data">
                            <p>No capacity data available</p>
                        </div>
                    )}
                </GridCell>
                <GridCell columnSize={40}>
                    <div className="capacity-actions">
                        <Button
                            type="secondary"
                            onClick={this.onRefreshStatus.bind(this)}
                            disabled={localLoading}
                        >
                            {resx.get("Capacity_Action_Button_Refresh")}
                        </Button>
                        <Button
                            type="secondary"
                            onClick={this.onStartCapacity.bind(this)}
                            disabled={localLoading || !capacityStatus || !capacityStatus.State || !this.canStart(capacityStatus.State)}
                        >
                            {resx.get("Capacity_Action_Button_Start")}
                        </Button>
                        <Button
                            type="secondary"
                            onClick={this.onPauseCapacity.bind(this)}
                            disabled={localLoading || !capacityStatus || !capacityStatus.State || !this.canPause(capacityStatus.State)}
                        >
                            {resx.get("Capacity_Action_Button_Pause")}
                        </Button>
                    </div>
                </GridCell>
            </>
        );
    }
}

CapacityStatus.propTypes = {
    dispatch: PropTypes.func.isRequired,
    selectedWorkspace: PropTypes.number,
    isWorkspaceConfigured: PropTypes.bool,
    capacityStatus: PropTypes.object,
    statusError: PropTypes.object
};

function mapStateToProps(state) {
    return {
        capacityStatus: state.capacityManagement.capacityStatus,
        statusError: state.capacityManagement.statusError
    };
}

export default connect(mapStateToProps)(CapacityStatus);
