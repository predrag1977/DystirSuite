import React, { Component } from 'react';
import { PageName } from '../services/dystirWebClientService';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { FaArrowLeft } from "react-icons/fa6";
import { FaArrowsRotate } from "react-icons/fa6";
import '../css/nav-menu.css';

export class HeaderMatchDetails extends Component {
    static displayName = HeaderMatchDetails.name;

    constructor(props) {
        super(props);

    }

    render() {
        return (
            <div id="header" className="navbar">
                <div id="header_match_details_wrapper">
                    <table id="horizontal_navigation_bar" className="w-100 mb-1 mt-1">
                        <tbody>
                            <tr>
                                <td style={{ width: '50px' }} >
                                    <span id="back_button" onClick={() => window.history.back()}>
                                        <FaArrowLeft />
                                    </span>
                                </td>
                                <td className="match_item_team_name pr-1 text-end">
                                    <span className="match_details_header_text">{this.props.match?.homeTeam}</span>
                                </td>
                                <td className="text-center" style={{ width: '20px' }} >
                                    <div className="match_details_header_text" > -  </div>
                                </td>
                                <td className="match_item_team_name pr-1 text-start">
                                    <span className="match_details_header_text" >{this.props.match?.awayTeam}</span>
                                </td>
                                <td style={{ width: '50px' }}>
                                    <span id="back_button" onClick={() => window.location.reload(false)}>
                                        <FaArrowsRotate />
                                    </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                
            </div>
        );
    }
}
