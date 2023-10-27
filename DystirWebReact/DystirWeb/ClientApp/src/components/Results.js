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
        if (this.state.matches == null) {
            DystirWebClientService.loadResultDataAsync();
        };
    }

    componentDidMount() {
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.addEventListener('onResultDataLoaded', this.onResultDataLoaded.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.removeEventListener('onResultDataLoaded', this.onResultDataLoaded.bind(this));
    }

    onConnected() {
        DystirWebClientService.loadResultDataAsync();
    }

    onDisconnected() {
    }

    onReceiveMatchDetails(event) {
        const match = event.detail.matchDetail['match'];
        const list = DystirWebClientService.resultsData.results.filter((item) => item.matchID !== match.matchID)

        list.push(match);
      
        DystirWebClientService.resultsData = {
            matches: list,
            isMatchesLoading: false
        };
        this.setState({
            matches: list
        });
    }

    onResultDataLoaded() {
        console.log("onResultDataLoaded");
        this.setState({
            matches: DystirWebClientService.resultsData.matches,
            isLoading: DystirWebClientService.resultsData.isLoading
        });
    }

    render() {
        let contents =
            <div>
            {
                (this.state.matches == null || this.state.isLoading) &&
                <div className="loading-spinner-parent spinner-border" />
            }
            {
                this.renderResults(this.filterMatches(this.state.matches))
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

    renderResults(matches) {
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