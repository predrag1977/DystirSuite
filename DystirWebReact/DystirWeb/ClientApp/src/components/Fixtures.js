import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Fixtures extends Component {
    static displayName = Fixtures.name;

    constructor(props) {
        super(props);
        let fixturesData = dystirWebClientService.state.fixturesData;
        this.state = {
            matches: fixturesData.matches,
            isLoading: fixturesData.isLoading
        }
        if (this.state.matches === null) {
            dystirWebClientService.loadFixturesDataAsync();
        };
    }

    componentDidMount() {
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.addEventListener('onFixturesDataLoaded', this.onFixturesDataLoaded.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.removeEventListener('onFixturesDataLoaded', this.onFixturesDataLoaded.bind(this));
    }

    onConnected() {
        dystirWebClientService.loadFixturesDataAsync();
    }

    onDisconnected() {
    }

    onReceiveMatchDetails(event) {
        const match = event.detail.matchDetail['match'];
        const list = dystirWebClientService.fixturesData.results.filter((item) => item.matchID !== match.matchID)

        list.push(match);

        this.setState({
            matches: list
        });
    }

    onFixturesDataLoaded() {
        this.setState({
            matches: dystirWebClientService.state.fixturesData.matches,
            isLoading: dystirWebClientService.state.fixturesData.isLoading
        });
    }

    render() {
        let contents =
            <div>
            {
                (this.state.matches === null || this.state.isLoading) &&
                <div className="loading-spinner-parent spinner-border" />
            }
            {
                this.renderFixtures(this.filterMatches(this.state.matches))
            }
            </div>
        return (
            <LayoutDystir page={PageName.FIXTURES}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderFixtures(matches) {
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

        var fromDate = new MatchDate().dateUtc();

        return matches.filter((match) =>
            MatchDate.parse(match.time) >= MatchDate.parse(fromDate)
            && match.statusID < 30
            && match.matchTypeName == matches[0].matchTypeName);
    }
}