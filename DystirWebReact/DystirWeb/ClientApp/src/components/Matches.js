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
        if (this.state.matches == null) {
            DystirWebClientService.loadMatchesDataAsync();
        };
    }

    componentDidMount() {
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.addEventListener('onMatchesDataLoaded', this.onMatchesDataLoaded.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.removeEventListener('onMatchesDataLoaded', this.onMatchesDataLoaded.bind(this));
    }

    onConnected() {
        DystirWebClientService.loadMatchesDataAsync();
    }

    onDisconnected() {
    }

    onReceiveMatchDetails(event) {
        const match = event.detail.matchDetail['match'];
        const list = DystirWebClientService.matchesData.matches.filter((item) => item.matchID !== match.matchID)

        list.push(match);

        DystirWebClientService.matchesData = {
            matches: list
        };
        this.setState({
            matches: list
        });
    }

    onMatchesDataLoaded() {
        console.log("onMatchesDataLoaded");
        this.setState({
            matches: DystirWebClientService.matchesData.matches,
            isLoading: DystirWebClientService.matchesData.isLoading
        });
    }

    periodSelected(periodIndex) {
        console.log(periodIndex);
        DystirWebClientService.matchesData = {
            selectedPeriod: periodIndex
        };
        this.setState({
            selectedPeriod: periodIndex
        });
    }

    render() {
        let contents =
            <>
                <ChooseDays periodSelected={(periodIndex) => this.periodSelected(periodIndex)} />
                <div>
                    {
                        (this.state.matches == null || this.state.isLoading) &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        this.renderMatches(this.filterMatches(this.state.matches))
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

    filterMatches(matches) {
        if (matches == null) {
            return matches;
        }
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.dateUtc().addDays(-1);
        var toDate = now.dateUtc().addDays(1);

        if (this.state.selectedPeriod === 2) {
            fromDate = now.dateUtc().addDays(-10);
            toDate = now.dateUtc().addDays(10);
            console.log(this.state.selectedPeriod);
        }

        var list = matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
        );

        return list;
    }
}
