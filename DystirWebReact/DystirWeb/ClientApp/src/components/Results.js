import React, { Component } from 'react';
import { PuffLoader } from 'react-spinners';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseCompetitions } from './ChooseCompetitions';
import { Sponsors } from './Sponsors';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Results extends Component {
    static displayName = Results.name;

    constructor(props) {
        super(props);

        dystirWebClientService.selectedPage = "results";

        let resultsData = dystirWebClientService.state.resultsData;
        
        if(resultsData.selectedResultsCompetitionId == "") {
            resultsData.selectedResultsCompetitionId = window.location.pathname.split("/").pop();
        }
        if (isNaN(resultsData.selectedResultsCompetitionId) || resultsData.selectedResultsCompetitionId == "") {
            resultsData.selectedResultsCompetitionId = 0
        }
        window.history.replaceState(null, null, "/results/" + resultsData.selectedResultsCompetitionId);

        this.state = {
            matches: resultsData.matches,
            selectedResultsCompetitionId: resultsData.selectedResultsCompetitionId,
            isLoading: true
        }
        
        dystirWebClientService.loadResultDataAsync(this.state.selectedResultsCompetitionId);

        window.onpopstate = () => {
            this.onClickCompetition();
        }
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
        dystirWebClientService.loadResultDataAsync(this.state.selectedResultsCompetitionId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition() {
        let competitionId = window.location.pathname.split("/").pop();
        dystirWebClientService.state.resultsData.selectedResultsCompetitionId = competitionId;
        this.setState({
            selectedResultsCompetitionId: competitionId
        });
    }

    render() {
        const results = this.filterMatches(this.state.matches)
        const matchesGroup = results.groupBy(match => { return match.matchTypeName });
        const competitions = [];
        Object.keys(matchesGroup).map((group) => {
            competitions.push(group);
        });
        const selectedResultsCompetitionId = this.state.selectedResultsCompetitionId !== ""
            ? this.state.selectedResultsCompetitionId : (competitions.length > 0 ? 0 : "");
        let contents =
        <>
            <ChooseCompetitions onClickCompetition={() => this.onClickCompetition()}
                competitions={competitions}
                page="results"
                selectedCompetition={competitions[selectedResultsCompetitionId]} />
            <div className="main_container">
                {
                    this.state.isLoading &&
                    <PuffLoader className="loading-spinner-parent" color="lightGray" height="50" width="50" />
                }
                {
                    competitions.length > 0 &&
                    this.renderResults(matchesGroup, competitions[selectedResultsCompetitionId])
                }
                <Sponsors />
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

    renderResults(matchesGroup, competition) {
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
        var fromDate = new MatchDate(new MatchDate().getFullYear(), 0, 1);
        var toDate = new MatchDate().dateUtc();

        return matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
            && (match.statusID == 12 || match.statusID == 13));
    }
}