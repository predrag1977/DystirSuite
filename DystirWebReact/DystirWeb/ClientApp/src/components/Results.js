import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseDays } from './ChooseDays';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Results extends Component {
    static displayName = Results.name;

    constructor(props) {
        super(props);
        let resultsData = dystirWebClientService.state.resultsData;
        this.state = {
            matches: resultsData.matches,
            isLoading: true
        }
        dystirWebClientService.loadResultDataAsync();
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
    }

    onReloadData() {
        this.setState({
            matches: dystirWebClientService.state.resultsData.matches,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadResultDataAsync();
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    render() {
        let contents =
            <>
                <ChooseDays />
                <div className="main_container">
                    {
                        (this.state.matches === null || this.state.isLoading) &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        this.renderResults(this.filterMatches(this.state.matches))
                    }
                </div>
            </>
        return (
            <LayoutDystir page={PageName.RESULTS}>
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