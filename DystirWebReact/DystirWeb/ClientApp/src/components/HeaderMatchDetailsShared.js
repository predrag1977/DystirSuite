import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { FaArrowLeft } from "react-icons/fa6";
import { FaArrowsRotate } from "react-icons/fa6";
import MatchTimeAndColor from '../extentions/matchTimeAndColor';
import '../css/nav-menu.css';

const dystirWebClientService = DystirWebClientService.getInstance();

export class HeaderMatchDetailsShared extends Component {
    static displayName = HeaderMatchDetailsShared.name;

    constructor(props) {
        super(props);
        this.matchTimeAndColor = new MatchTimeAndColor();
        this.state = {
            matchTime: this.matchTimeAndColor.getMatchTime(this.props.match),
            statusColor: this.matchTimeAndColor.getStatusColor(this.props.match?.statusID)
        };
    }

    componentDidMount() {
        document.body.addEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    onMatchTime() {
        var matchTime = this.matchTimeAndColor.getMatchTime(this.props.match);
        this.setState({ matchTime: matchTime, statusColor: this.matchTimeAndColor.getStatusColor(this.props.match?.statusID) });
    }

    render() {
        const match = this.props.match;
        return (
            <div id="header" className="navbar">
                <div id="header_match_details_wrapper">
                    <table id="horizontal_navigation_bar" className="w-100">
                        <tbody>
                            <tr>
                                <td style={{ width: '50px' }} >
                                    <div id="dystir_icon_button" className="navbar-dark">
                                        <a href="">
                                            <img src="images/icons/dystir_icon_dark.png" />
                                        </a>
                                    </div>
                                </td>
                                <td style={{verticalAlign: "middle"}}>
                                    <table className="w-100 text-center">
                                        <tbody>
                                            <tr style={{ fontSize: "18px" }}>
                                                <td className="match_item_team_name text-end">{match?.homeTeam}</td>
                                                <td style={{ width: "20px" }}>-</td>
                                                <td className="match_item_team_name text-start">{match?.awayTeam}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <table className="w-100 text-center">
                                        <tbody>
                                            <tr style={{ fontSize: "16px" }}>
                                                <td></td>
                                                <td className="match_time" style={{ whiteSpace: 'nowrap' }}>
                                                {
                                                    match?.statusID < 14 &&
                                                    <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                                }
                                                </td>
                                                <td></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                                <td style={{ width: '50px' }}>
                                    <div id="dystir_icon_button" className="navbar-dark">
                                        <a href="">
                                            <img src="images/icons/dystir_icon_dark.png" />
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}
