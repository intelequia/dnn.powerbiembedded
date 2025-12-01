import React, { Component } from "react";
import PropTypes from "prop-types";
import { Collapsible, SvgIcons } from "@dnnsoftware/dnn-react-common";
import resx from "../../resources";
import CapacityEditor from "./capacityEditor";
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

    render() {
        const { capacity } = this.props;
        const statusClass = capacity.IsEnabled ? "status-enabled" : "status-disabled";
        const statusText = capacity.IsEnabled ? resx.get("Enabled") : resx.get("Disabled");

        return (
            <div className="capacity-row">
                <div className="capacity-row-header">
                    <div className="capacity-info">
                        <span className="capacity-name">{capacity.CapacityDisplayName}</span>
                        <span className="capacity-id">({capacity.CapacityName})</span>
                        <span className={`capacity-status ${statusClass}`}>{statusText}</span>
                    </div>
                    <div className="capacity-actions">
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
    onCancel: PropTypes.func
};

export default CapacityRow;
