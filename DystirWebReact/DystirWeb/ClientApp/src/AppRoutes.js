import { Fixtures } from "./components/Fixtures";
import { Matches } from "./components/Matches";
import { Results } from "./components/Results";
import { Standings } from "./components/Standings";
import { Statistics } from "./components/Statistics";
import { MatchDetails } from "./components/MatchDetails";
import { InfoMatches } from "./components/sharedComponents/InfoMatches";
import { MatchDetailsShared } from "./components/sharedComponents/MatchDetailsShared";

const AppRoutes = [
    {
        index: true,
        element: <Matches />
    },
    {
        path: '/matches/*',
        element: <Matches />
    },
    {
        path: '/results',
        element: <Results />
    },
    {
        path: '/results/*',
        element: <Results />
    },
    {
        path: '/fixtures',
        element: <Fixtures />
    },
    {
        path: '/fixtures/*',
        element: <Fixtures />
    },
    {
        path: '/standings',
        element: <Standings />
    },
    {
        path: '/standings/*',
        element: <Standings />
    },
    {
        path: '/statistics',
        element: <Statistics />
    },
    {
        path: '/statistics/*',
        element: <Statistics />
    },
    {
        path: '/matchDetails/*',
        element: <MatchDetails />
    },
    {
        path: '/info/matches',
        element: <InfoMatches />
    },
    {
        path: '/portal/matches',
        element: <InfoMatches />
    },
    {
        path: '/info/standings',
        element: <Standings />
    },
    {
        path: '/info/matchdetails/*',
        element: <MatchDetails />
    },
    {
        path: '/portal/matchdetails/*',
        element: <MatchDetails />
    }
];

export default AppRoutes;
