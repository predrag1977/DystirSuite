import React, { Component } from 'react';
import DystirWebClientService from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';

export class Matches extends Component {
    static displayName = Matches.name;

    constructor(props) {
        super(props);
        this.state = { matches: this.filterMatches(DystirWebClientService.matches) };
    }

    componentDidMount() {
        document.body.addEventListener('onFullDataLoaded', this.onFullDataLoaded.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onFullDataLoaded', this.onFullDataLoaded.bind(this));
    }

    onFullDataLoaded(event) {
        this.setState({ matches: this.filterMatches(event.detail.sortedMatches) });
    }

    render() {
        let contents = DystirWebClientService.matches == null
            ? <p><em>Loading...</em></p>
            : this.renderMatchDetails(this.state.matches);
        return (
            <LayoutDystir>
            {
                contents
            }     
            </LayoutDystir>
        );
    }

    renderMatchDetails(matches) {
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

        var fromDate = now.dateUtc().addDays(-10);
        var toDate = now.dateUtc().addDays(10);

        return matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
        );
    }
}
