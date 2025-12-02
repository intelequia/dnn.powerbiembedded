import React, { Component } from "react";
import PropTypes from "prop-types";
import { Collapsible, SvgIcons, Button } from "@dnnsoftware/dnn-react-common";
import resx from "../../resources";
import CapacityEditor from "./capacityEditor";
import CapacityState from "../capacityManagement/capacityStatus/capacityState";
import "./capacitySettings.less";

class CapacityRow extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isOpen: false
        };
        this.onDelete = this.onDelete.bind(this);
        this.onEdit = this.onEdit.bind(this);
        this.onCancel = this.onCancel.bind(this);
        this.onSave = this.onSave.bind(this);
        this.onStartCapacity = this.onStartCapacity.bind(this);
        this.onPauseCapacity = this.onPauseCapacity.bind(this);
    }

    componentDidUpdate(prevProps) {
        if (this.props.openId !== prevProps.openId) {
            this.setState({
                isOpen: this.props.openId === this.props.id
            });
        }
    }

    onDelete(event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        if (this.props.onDelete) {
            this.props.onDelete();
        }
    }

    onEdit(event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        this.setState({ isOpen: true });
        if (this.props.onEdit) {
            this.props.onEdit();
        }
    }

    onCancel() {
        this.setState({ isOpen: false });
        if (this.props.onCancel) {
            this.props.onCancel();
        }
    }

    onSave(capacity) {
        if (this.props.onUpdate) {
            this.props.onUpdate(capacity);
        }
        this.setState({ isOpen: false });
    }

    onStartCapacity(event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        if (this.props.onStartCapacity) {
            this.props.onStartCapacity(this.props.capacity.CapacityId);
        }
    }

    onPauseCapacity(event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        if (this.props.onPauseCapacity) {
            this.props.onPauseCapacity(this.props.capacity.CapacityId);
        }
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
            case CapacityState.SUSPENDED:
            case CapacityState.PAUSED:
                return resx.get("Capacity_Status_Text_Paused");
            case CapacityState.PAUSING:
                return resx.get("Capacity_Status_Text_Pausing");
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
            case CapacityState.PAUSED:
                return "capacity-status-paused";
            case CapacityState.PAUSING:
                return "capacity-status-pausing";
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

    render() {
        const { capacity, capacityStatus } = this.props;
        const statusClass = capacity.IsEnabled ? "status-enabled" : "status-disabled";
        const statusText = capacity.IsEnabled ? resx.get("Enabled") : resx.get("Disabled");
        
        const status = capacityStatus && capacityStatus.State ? capacityStatus.State : null;
        const runningStatusText = this.getCapacityStatusText(status);
        const runningStatusClass = this.getCapacityStatusClass(status);

        return (
            <div className="capacity-row">
                <div className="capacity-row-header">
                    <div className="capacity-info">
                        <span className="capacity-name">{capacity.CapacityDisplayName}</span>
                        <span className="capacity-id">({capacity.CapacityName})</span>
                        <span className={`capacity-status ${statusClass}`}>{statusText}</span>
                        {status && (
                            <span className={`capacity-running-status ${runningStatusClass}`}>{runningStatusText}</span>
                        )}
                    </div>
                    <div className="capacity-actions">
                        {status && (
                            <>
                                <Button
                                    type="secondary"
                                    size="small"
                                    onClick={this.onStartCapacity}
                                    disabled={!this.canStart(status)}
                                    title={resx.get("Capacity_Action_Button_Start")}>
                                    {resx.get("Capacity_Action_Button_Start")}
                                </Button>
                                <Button
                                    type="secondary"
                                    size="small"
                                    onClick={this.onPauseCapacity}
                                    disabled={!this.canPause(status)}
                                    title={resx.get("Capacity_Action_Button_Pause")}>
                                    {resx.get("Capacity_Action_Button_Pause")}
                                </Button>
                            </>
                        )}
                        <button
                            className="edit-button"
                            onClick={this.onEdit}
                            title={resx.get("Edit")}
                            dangerouslySetInnerHTML={{ __html: SvgIcons.EditIcon }}>
                        </button>
                        <button
                            className="delete-button"
                            onClick={this.onDelete}
                            title={resx.get("Delete")}
                            dangerouslySetInnerHTML={{ __html: SvgIcons.TrashIcon }}>                                
                            

                        </button>
                    </div>
                </div>
                <Collapsible
                    isOpened={this.state.isOpen}
                    className="capacity-row-details">
                    <CapacityEditor
                        capacity={capacity}
                        onSave={this.onSave}
                        onCancel={this.onCancel}
                        inline={true}
                        capacityRules={this.props.capacityRules}
                        onDeleteRule={this.props.onDeleteRule}
                        onSaveRule={this.props.onSaveRule}
                    />
                </Collapsible>
            </div>
        );
    }
}

CapacityRow.propTypes = {
    capacity: PropTypes.object.isRequired,
    id: PropTypes.number.isRequired,
    openId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    onEdit: PropTypes.func,
    onDelete: PropTypes.func.isRequired,
    onUpdate: PropTypes.func.isRequired,
    onCancel: PropTypes.func,
    capacityStatus: PropTypes.object,
    onStartCapacity: PropTypes.func,
    onPauseCapacity: PropTypes.func,
    capacityRules: PropTypes.array,
    onDeleteRule: PropTypes.func,
    onSaveRule: PropTypes.func
};

export default CapacityRow;
