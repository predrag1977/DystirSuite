import React, { Component } from 'react';
import { ThreeDots } from 'react-loading-icons';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseCompetitions } from './ChooseCompetitions';
import { Sponsors } from './Sponsors';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Fixtures extends Component {
    static displayName = Fixtures.name;

    constructor(props) {
        super(props);

        dystirWebClientService.selectedPage = "fixtures";

        let fixturesData = dystirWebClientService.state.fixturesData;
        if (fixturesData.selectedFixturesCompetitionId !== undefined && fixturesData.selectedFixturesCompetitionId !== "") {
            window.history.replaceState(null, null, "/fixtures/" + fixturesData.selectedFixturesCompetitionId);
        } else {
            fixturesData.selectedFixturesCompetitionId = window.location.pathname.split("/fixtures").pop().replace('/', '');
        }
        
        this.state = {
            matches: fixturesData.matches,
            selectedFixturesCompetitionId: fixturesData.selectedFixturesCompetitionId,
            isLoading: true
        }
        if (this.state.selectedFixturesCompetitionId !== undefined && this.state.selectedFixturesCompetitionId !== "") {
            window.history.replaceState(null, null, "/fixtures/" + this.state.selectedFixturesCompetitionId);
        }
        dystirWebClientService.loadFixturesDataAsync(this.state.selectedFixturesCompetitionId);
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
            matches: dystirWebClientService.state.fixturesData.matches,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadFixturesDataAsync(this.state.selectedFixturesCompetitionId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition() {
        let periodParameter = window.location.pathname.split("/").pop();
        dystirWebClientService.state.fixturesData.selectedFixturesCompetitionId = periodParameter;
        this.setState({
            selectedFixturesCompetitionId: periodParameter
        });
    }

    render() {
        const fixtures = this.filterMatches(this.state.matches)
        const matchesGroup = fixtures.groupBy(match => { return match.matchTypeName });
        const competitions = [];
        Object.keys(matchesGroup).map((group) => {
            competitions.push(group);
        });
        const selectedFixturesCompetitionId = this.state.selectedFixturesCompetitionId !== ""
            ? this.state.selectedFixturesCompetitionId : (competitions.length > 0 ? 0 : "");

        let contents =
            <>
                <ChooseCompetitions onClickCompetition={() => this.onClickCompetition()}
                    competitions={competitions}
                    page="fixtures"
                    selectedCompetition={competitions[selectedFixturesCompetitionId]} />
                <div className="main_container">
                {
                    this.state.isLoading &&
                    <ThreeDots className="loading-spinner-parent" fill= 'dimGray' height="50" width="50" />
                }
                {
                    competitions.length > 0 &&
                    this.renderFixtures(matchesGroup, competitions[selectedFixturesCompetitionId])
                }
                <Sponsors />
                </div>
            </>
        
        return (
            <LayoutDystir page={PageName.FIXTURES}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderFixtures(matchesGroup, competition) {
        if (competition === undefined) {
            return
        }
        const matchesByRound = matchesGroup[competition].groupBy(match => { return match.roundName ?? "" })
        return (
            Object.keys(matchesByRound).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group ?? ""}</div>
                    {
                        matchesByRound[group].map(match =>
                            <MatchView key={match.matchID} match={match} />
                        )
                    }
                </div>
            )
        );
    }

    filterMatches(matches) {
        var fromDate = new MatchDate().dateUtc();

        var fixtures = matches.filter((match) =>
            MatchDate.parse(match.time) >= MatchDate.parse(fromDate)
            && match.statusID < 30);

        return fixtures;
    }
}