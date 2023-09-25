import React, { Component } from 'react';
import DystirWebClientService from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';

export class Results extends Component {
    static displayName = Results.name;

    constructor(props) {
        super(props);
        this.state = DystirWebClientService.resultsData;
        if (DystirWebClientService.resultsData.matches == null) {
            this.loadMatchesDataAsync();
        };
    }

    componentDidMount() {
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
    }

    onConnected() {
        this.loadMatchesDataAsync();
    }

    onDisconnected() {
    }

    onReceiveMatchDetails(event) {
        const match = event.detail.matchDetail['match'];
        const list = DystirWebClientService.resultsData.results.filter((item) => item.matchID !== match.matchID)

        list.push(match);

        this.setState({ matches: this.filterMatches(list) });
        DystirWebClientService.resultsData = {
            matches: this.filterMatches(list),
            isMatchesLoading: false
        };
    }

    render() {
        let contents =
            <div>
            {
                (this.state.matches == null || this.state.isMatchesLoading) &&
                <div className="loading-spinner-parent spinner-border" />
            }
            {
                this.renderMatches(this.state.matches)
            }
            </div>
        return (
            <LayoutDystir page="ÚRSLIT">
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderMatches(matches) {
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.roundName });
        return (
            Object.keys(matchesGroup).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group ?? ""}</div>
                    {
                        matchesGroup[group].map(match =>
                            <MatchView key={match.matchID} match={match} />
                        )
                    }
                </div>
            )
        );
    }

    async loadMatchesDataAsync() {
        const response = await fetch('api/matches/results');
        const data = await response.json();
        const sortedMatches = data
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => b.roundID - a.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);
            
            
        this.setState({ matches: this.filterMatches(sortedMatches), isMatchesLoading: false });
        DystirWebClientService.resultsData = {
            matches: this.filterMatches(sortedMatches),
            isMatchesLoading: false
        };
    }

    filterMatches(matches) {
        if (matches == null || matches.lenght == 0) {
            return matches;
        }

        var fromDate = new MatchDate(new MatchDate().getFullYear(), 1, 1);
        var toDate = new MatchDate().dateUtc();

        return matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
            && match.statusID >= 12
            && match.matchTypeName == matches[0].matchTypeName);
    }
}