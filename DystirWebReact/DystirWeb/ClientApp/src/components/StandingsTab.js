import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { StandingView } from './views/StandingView';

export class StandingsTab extends Component {
    static displayName = StandingsTab.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        if (this.props.standings.length === 0) return;
        let standing = this.props.standings.filter(
            (standing) => standing.standingCompetitionName == match.matchTypeName)[0];
        if (standing === undefined) return;
        let contents =
            <StandingView key={standing?.standingCompetitionName} standing={standing} />

        return contents
    }
}