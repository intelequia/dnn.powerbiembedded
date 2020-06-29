import React, {Component} from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridSystem, GridCell }  from "@dnnsoftware/dnn-react-common";
import Workspaces from "./workspaces";
import resx from "../../resources";
import "./generalSettings.less";

class GeneralSettings extends Component {

    constructor() {
        super();

        this.state = {

        };
    }
    /*UNSAFE_componentWillMount() {
        const {props} = this;        
    }*/

    /*UNSAFE_componentWillReceiveProps(nextProps) {
        const {state} = this;
    } */  


    render() {
        return (
            <div className="dnn-pbiembedded-generalSettings">
                <GridCell columnSize={50}>
                    <p className="panel-description">{resx.get("lblTabDescription")}</p>
                </GridCell>
                <GridCell columnSize={50}>
                    <div className="logo"></div>
                </GridCell>
                <GridSystem className="with-right-border top-half">
                    <GridCell columnSize={90}>
                        <h1>{resx.get("lblWorkspaceSettings")}</h1>
                        <Workspaces />
                    </GridCell>
                    <GridCell columnSize={100}>

                    </GridCell>
                </GridSystem>
            </div>
        );
    }
}

GeneralSettings.propTypes = {
    dispatch: PropTypes.func.isRequired,

};


function mapStateToProps(
    
) {
    return {
      
    };
}

export default connect(mapStateToProps)(GeneralSettings);