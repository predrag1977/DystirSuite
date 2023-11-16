import React, { Component } from 'react';
import { SelectPeriodName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { format } from 'react-string-format';

export class ChooseCompetitions extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    render() {
        return (
            <div id="competitions_selection">
                <div id="horizontal_menu">
                    <div id="horizontal_menu_wrapper">
                    {
                            this.props.standings.map((standing) =>
                            <div key={standing.standingCompetitionId}
                                    className={"tab " + (this.props.selectedStandingsCompetitionId === standing.standingCompetitionId ? "selected_tab" : "")}
                                onClick={() => this.props.onClickCompetition()}>
                                <NavLink
                                    tag={Link}
                                    to={"/standings/" + standing.standingCompetitionId}>
                                    <span>{standing.standingCompetitionName}</span>
                                </NavLink>
                            </div>
                        )
                    }
                    </div>
                </div>
            </div>
        );
    }
}
