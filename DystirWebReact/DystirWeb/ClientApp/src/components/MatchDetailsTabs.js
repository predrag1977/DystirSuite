import React, { Component } from 'react';
import { TabName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { format } from 'react-string-format';
import { GiSoccerField } from "react-icons/gi";

export class MatchDetailsTabs extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        let matchID = match?.matchID != null ? match.matchID : 0;
        const selectedTab = this.props.selectedTab !== undefined && this.props.selectedTab !== "" ? this.props.selectedTab : TabName.SUMMARY;
        let page = "/" + this.props.page;
        if (this.props.page === undefined) {
            page = "";
        }
        let fullUrl = page + "/matchdetails/" + matchID + "/";
        return (
            <div id="days_selection">
                <div id="horizontal_menu_days">
                    <div className={"tab " + (selectedTab == TabName.SUMMARY ? "selected_tab" : "")}
                        onClick={() => this.props.onClickTab()}>
                        <NavLink
                            tag={Link}
                            to={fullUrl + TabName.SUMMARY}>
                            <span>Úrtak</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedTab == TabName.LINEUPS ? "selected_tab" : "")}
                        onClick={() => this.props.onClickTab()}>
                        <NavLink
                            tag={Link}
                            to={fullUrl + TabName.LINEUPS}>
                            <span>Leikarar</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedTab == TabName.COMMENTARY ? "selected_tab" : "")}
                        onClick={() => this.props.onClickTab()}>
                        <NavLink
                            tag={Link}
                            to={fullUrl + TabName.COMMENTARY}>
                            <span>Hendingar</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedTab == TabName.STATISTICS ? "selected_tab" : "")}
                        onClick={() => this.props.onClickTab()}>
                        <NavLink
                            tag={Link}
                            to={fullUrl + TabName.STATISTICS}>
                            <span>Hagtøl</span>
                        </NavLink>
                    </div>
                    <div className={"tab " + (selectedTab == TabName.STANDINGS ? "selected_tab" : "")}
                        onClick={() => this.props.onClickTab()}>
                        <NavLink
                            tag={Link}
                            to={fullUrl + TabName.STANDINGS}>
                            <span>Støðan</span>
                        </NavLink>
                    </div>
                    <div className="live_matches" onClick={() => this.props.onMoreLiveMatchClick()}>
                        <GiSoccerField className="live_matches_icon" />
                        <div className="live_matches_number">5</div>
                    </div>
                </div>
            </div>
        );
    }
}
