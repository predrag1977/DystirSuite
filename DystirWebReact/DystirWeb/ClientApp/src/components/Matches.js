import React, { Component } from 'react';
import DystirWebClientService from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { ChooseDays } from './ChooseDays';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';

export class Matches extends Component {
    static displayName = Matches.name;

    constructor(props) {
        super(props);
        this.state = DystirWebClientService.matchesData;
        if (DystirWebClientService.matchesData.matches == null) {
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
        const list = DystirWebClientService.matchesData.matches.filter((item) => item.matchID !== match.matchID)

        list.push(match);

        this.setState({ matches: this.filterMatches(list) });
        DystirWebClientService.matchesData = this.state;
    }

    render() {
        let contents =
            <>
                <ChooseDays />
                <div>
                    {
                        (this.state.matches == null || this.state.isMatchesLoading) &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        this.renderMatches(this.state.matches)
                    }
                </div>
            </>
        return (
            <LayoutDystir page="DYSTIR">
                
            {
                

                contents
            }     
            </LayoutDystir>
        );
    }

    renderMatches(matches) {
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName });
        return (
            Object.keys(matchesGroup).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group}</div>
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
        const response = await fetch('api/matches');
        const data = await response.json();
        const sortedMatches = data.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));
        this.setState({ matches: this.filterMatches(sortedMatches), isMatchesLoading: false });
        DystirWebClientService.matchesData = {
            matches: this.filterMatches(sortedMatches),
            isMatchesLoading: false
        };
    }

    filterMatches(matches) {
        if (matches == null) {
            return matches;
        }
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.dateUtc().addDays(-10);
        var toDate = now.dateUtc().addDays(0);

        if (this.state.selectedPeriod == 0) {
            fromDate = now.dateUtc().addDays(0)
        }


        return matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
        );
    }
}
