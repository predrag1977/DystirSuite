import React, { Component } from 'react';
import { PageName } from '../services/dystirWebClientService';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { FaArrowsRotate } from "react-icons/fa6";
import { Link } from 'react-router-dom';

export class Header extends Component {
    static displayName = Header.name;

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
                                <td id="header_title">
                                    <span>{this.props.page}</span>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page == PageName.MATCHES ? "active" : "") + " text-dark"} to="/">{PageName.MATCHES}</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page == PageName.RESULTS ? "active" : "") + " text-dark"} to="/results">{PageName.RESULTS}</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page == PageName.FIXTURES ? "active" : "") + " text-dark"} to="/fixtures">{PageName.FIXTURES}</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page == PageName.STANDINGS ? "active" : "") + " text-dark"} to="/standings">{PageName.STANDINGS}</NavLink>
                                </td>
                                <td className="nav-item header_item">
                                    <NavLink tag={Link} className={(this.props.page == PageName.STATISTICS ? "active" : "") + " text-dark"} to="/statistics">{PageName.STATISTICS}</NavLink>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <div style={{ height: "53px", margin: "10px 0"}}>
                                        <div id="back_button" 
                                            onClick={() => window.location.reload(false)}>
                                            <FaArrowsRotate fill="#a6a6a6" />
                                        </div>
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
                        <li className="nav-item" onClick={this.toggleNavbar}>
                            <NavLink tag={Link} className={(this.props.page == PageName.MATCHES ? "active" : "") + " text-dark nav-link"} to="/">{PageName.MATCHES}</NavLink>
                        </li>
                        <li className="nav-item" onClick={this.toggleNavbar}>
                            <NavLink tag={Link} className={(this.props.page == PageName.RESULTS ? "active" : "") + " text-dark nav-link"} to="/results">{PageName.RESULTS}</NavLink>
                        </li>
                        <li className="nav-item" onClick={this.toggleNavbar}>
                            <NavLink tag={Link} className={(this.props.page == PageName.FIXTURES ? "active" : "") + " text-dark nav-link"} to="/fixtures">{PageName.FIXTURES}</NavLink>
                        </li>
                        <li className="nav-item" onClick={this.toggleNavbar}>
                            <NavLink tag={Link} className={(this.props.page == PageName.STANDINGS ? "active" : "") + " text-dark nav-link"} to="/standings">{PageName.STANDINGS}</NavLink>
                        </li>
                        <li className="nav-item" onClick={this.toggleNavbar}>
                            <NavLink tag={Link} className={(this.props.page == PageName.STATISTICS ? "active" : "") + " text-dark nav-link"} to="/statistics">{PageName.STATISTICS}</NavLink>
                        </li>
                    </ul>
                </div>
            </>
        );
    }
}
