import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import '../css/nav-menu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <>
                <div id="header" className="navbar">
                    <table id="horizontal_navigation_bar">
                        <tbody>
                            <tr>
                                <td>
                                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2 navbar-dark" />
                                </td>
                                {/*@if (ShowBackButton())*/}
                                {/*{*/}
                                {/*    <td>*/}
                                {/*        <span id="back_button" onclick="goBack()">*/}
                                {/*            <span className="fas fa-arrow-left"></span>*/}
                                {/*        </span>*/}
                                {/*    </td>*/}
                                {/*}*/}
                                <td id="header_title">
                                    <span>{this.props.page}</span>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page.toUpper != "DYSTIR" ? "active" : "") + " text-dark"} to="/">DYSTIR</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page.toUpper != "ÚRSLIT" ? "active" : "") + " text-dark"} to="/results">ÚRSLIT</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className="text-dark" to="/football/fixtures">KOMANDI</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className="text-dark" to="/football/standings">STØÐAN</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className="text-dark" to="/football/statistics">HAGTØL</NavLink>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <div id="store_buttons">
                                        <span>
                                            <a href='https://play.google.com/store/apps/details?id=fo.Dystir&pcampaignid=MKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1'>
                                                <img alt='Get it on Google Play' src="images/icons/google-play-square.png" />
                                            </a>
                                        </span>
                                        <span>
                                            <a href='https://apps.apple.com/us/app/dystir/id1460781430?ls=1'>
                                                <img alt='Download on Apple Store' src="images/icons/apple-store-square.png" />
                                            </a>
                                        </span>
                                    </div>
                                </td>
                                <td>
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

                <div id="navigation_menu" className={this.state.collapsed ? "collapse" : ""} >
                    <ul className="nav flex-column">
                        <li className="nav-item">
                            <NavLink className="nav-link" href="" >
                                <span>DYSTIR</span>
                            </NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" href="results">
                                <span>ÚRSLIT</span>
                            </NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" href="football/fixtures">
                                <span>KOMANDI</span>
                            </NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" href="football/standings">
                                <span>STØÐAN</span>
                            </NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" href="football/statistics">
                                <span>HAGTØL</span>
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </>
        );
    }
}
