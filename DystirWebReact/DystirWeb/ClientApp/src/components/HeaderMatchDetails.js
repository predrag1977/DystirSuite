import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { FaArrowLeft } from "react-icons/fa6";
import { FaArrowsRotate } from "react-icons/fa6";
import MatchDate from '../extentions/matchDate';
import MatchTimeAndColor from '../extentions/matchTimeAndColor';
import { format } from 'react-string-format';

const dystirWebClientService = DystirWebClientService.getInstance();

export class HeaderMatchDetails extends Component {
    static displayName = HeaderMatchDetails.name;

    constructor(props) {
        super(props);
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
                                    <span id="back_button">
                                        <NavLink tag={Link} to={"/" + dystirWebClientService.selectedPage}>
                                            <FaArrowLeft fill="#a6a6a6" />
                                        </NavLink>
                                    </span>
                                </td>
                                <td style={{verticalAlign: "middle"}}>
                                    <table className="w-100 text-center">
                                        <tbody>
                                            <tr style={{ fontSize: "1.1rem" }}>
                                                <td className="match_item_team_name text-end">
                                                {
                                                    format('{0} {1} {2}', match?.homeTeam, match?.homeSquadName, match?.homeCategoriesName)
                                                }
                                                </td>
                                                <td style={{ width: "20px" }}>-</td>
                                                <td className="match_item_team_name text-start">
                                                {
                                                    format('{0} {1} {2}', match?.awayTeam, match?.awaySquadName, match?.awayCategoriesName)
                                                }
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    
                                </td>
                                <td style={{ width: '50px' }}>
                                    <div id="back_button" onClick={() => window.location.reload(false)}>
                                        <FaArrowsRotate fill="#a6a6a6" />
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
