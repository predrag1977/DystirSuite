import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../../services/dystirWebClientService';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { FaArrowLeft } from "react-icons/fa6";
import { FaArrowsRotate } from "react-icons/fa6";
import MatchTimeAndColor from '../../extentions/matchTimeAndColor';

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

    onMatchTime(event) {
        var connected = event.detail.connected;
        var matchTime = this.matchTimeAndColor.getMatchTime(this.props.match);
        var statusColor = this.matchTimeAndColor.getStatusColor(this.props.match.statusID);
        this.setState({ matchTime: matchTime, statusColor: statusColor });
    }

    render() {
        let logo = "";
        let link = "";
        let url = window.location.href.toLowerCase();
        if (url.includes("info")) {
            logo = "https://in.fo/fileadmin/templates/ext/bootstrap_package/Resources/Public/Images/logo/info-logo.svg";
            link = "https://www.in.fo";
        } else if (url.includes("portal")) {
            logo = "https://www.dystir.fo/images/icons/portal_icon.png";
            link = "https://www.portal.fo";
        } else if (url.includes("roysni")) {
            logo = "https://www.dystir.fo/images/icons/roysni_icon.png";
            link = "https://www.roysni.fo";
        }
        const match = this.props.match;
        var matchTime = this.matchTimeAndColor.getMatchTime(this.props.match);
        this.state.matchTime = matchTime;
        this.state.statusColor = this.matchTimeAndColor.getStatusColor(match?.statusID);
        return (
            <div id="header" className="navbar">
                <div id="header_match_details_wrapper">
                    <table id="horizontal_navigation_bar" className="w-100">
                        <tbody>
                            <tr>
                                <td style={{ width: '0px' }} >
                                    <div id="logo_icon_button" className="navbar-dark">
                                        <a href={link} target="_parent">
                                            <img src={logo} />
                                        </a>
                                    </div>
                                </td>
                                <td style={{verticalAlign: "middle"}}>
                                    
                                </td>
                                <td style={{ width: '0px' }}>
                                    <div id="dystir_icon_button_shared" className="navbar-dark">
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
