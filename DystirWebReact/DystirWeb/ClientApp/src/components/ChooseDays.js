import React, { Component } from 'react';
import DystirWebClientService, { SelectPeriodName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { format } from 'react-string-format';

export class ChooseDays extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    render() {
        const match = this.props.match;
        const selectedPeriod = this.props.selectedPeriod !== undefined && this.props.selectedPeriod !== "" ? this.props.selectedPeriod : SelectPeriodName.TODAY;
        return (
            <div id="days_selection">
                <div id="horizontal_menu_days">
                    <div className={"tab " + (selectedPeriod == SelectPeriodName.BEFORE ? "selected_tab" : "")}
                        onClick={() => this.props.onClickPeriod()}>
                        <NavLink
                            tag={Link}
                            to="/matches/before">
                            <span>-10 dagar</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedPeriod == SelectPeriodName.YESTERDAY ? "selected_tab" : "")}
                        onClick={() => this.props.onClickPeriod()}>
                        <NavLink 
                            tag={Link}
                            to="/matches/yesterday">
                            <span>Í gjár</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedPeriod == SelectPeriodName.TODAY ? "selected_tab" : "")}
                        onClick={() => this.props.onClickPeriod()}>
                        <NavLink
                            tag={Link}
                            to="/matches/today">
                            <span>Í dag</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedPeriod == SelectPeriodName.TOMORROW ? "selected_tab" : "")}
                        onClick={() => this.props.onClickPeriod()}>
                        <NavLink
                            tag={Link}
                            to="/matches/tomorrow">
                            <span>Í morgin</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedPeriod == SelectPeriodName.NEXT ? "selected_tab" : "")}
                        onClick={() => this.props.onClickPeriod()}>
                        <NavLink
                            tag={Link}
                            to="/matches/next">
                            <span>+10 dagar</span>
                        </NavLink>
                    </div>
                </div>
            </div>
        );
    }
}
