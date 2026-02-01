import React, { Component } from "react";
import PropTypes from "prop-types";
import { Collapsible, SvgIcons } from "@dnnsoftware/dnn-react-common";
import "./style.less";

class WorkspaceRow extends Component {
    /* eslint-disable react/no-did-mount-set-state */
    componentDidMount() {
        let opened = (this.props.openId !== "" && this.props.id === this.props.openId);
        this.setState({
            opened
        });
    }

    toggle() {
        if ((this.props.openId !== "" && this.props.id === this.props.openId)) {
            this.props.Collapse();
        }
        else {
            this.props.OpenCollapse(this.props.id);
        }
    }

    onBrowse() {
        if ((this.props.settingsGroupId !== "")) {
            window.open("https://app.powerbi.com/groups/" + this.props.settingsGroupId);
        }
    }

    /* eslint-disable react/no-danger */
    render() {
        const {props} = this;
        let opened = (this.props.openId !== "" && this.props.id === this.props.openId);
        return (
            <div className={"collapsible-component-profile" + (opened ? " row-opened" : "")}>
                <div className={"collapsible-profile " + !opened} >
                    <div className={"row"}>
                        <div title={props.settingsGroupName} className="profile-item item-row-settingsGroupName">
                            {props.settingsGroupName}</div>
                        <div className="profile-item item-row-authenticationType">
                            {props.authenticationType}</div>
                        <div className="profile-item item-row-authenticationType">
                            {props.contentPageUrl}</div>

                        <div className="profile-item item-row-actionButtons">
                            {props.deletable &&
                                <div className={opened ? "delete-icon-hidden" : "delete-icon"} onClick={props.onDelete.bind(this)}><SvgIcons.TrashIcon /></div>
                            }
                            {props.editable &&
                                <div className={opened ? "edit-icon-active" : "edit-icon"} onClick={this.toggle.bind(this)}><SvgIcons.EditIcon /></div>
                            }
                            {props.editable && props.settingsGroupId !== "" &&
                                <div className={opened ? "pbi-icon-hidden" : "pbi-icon"} onClick={this.onBrowse.bind(this)}><SvgIcons.PreviewIcon /></div>
                            }                            
                        </div>
                    </div>
                </div>
                <Collapsible fixedHeight={515} keepContent={true} isOpened={opened} style={{ float: "left", width: "100%", overflow: "inherit" }}>{opened && props.children}</Collapsible>
            </div>
        );
    }
}

WorkspaceRow.propTypes = {
    settingsId: PropTypes.string,
    settingsGroupId: PropTypes.string,
    settingsGroupName: PropTypes.string,
    authenticationType: PropTypes.string,
    contentPageUrl: PropTypes.string,
    deletable: PropTypes.bool,
    editable: PropTypes.bool,
    OpenCollapse: PropTypes.func,
    Collapse: PropTypes.func,
    onDelete: PropTypes.func,
    id: PropTypes.string,
    openId: PropTypes.string
};

WorkspaceRow.defaultProps = {
    collapsed: true,
    deletable: true,
    editable: true
};
export default (WorkspaceRow);
