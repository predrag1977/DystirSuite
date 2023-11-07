import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Fixtures } from "./components/Fixtures";
import { Matches } from "./components/Matches";
import { Results } from "./components/Results";

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
        path: '/fixtures',
        element: <Fixtures />
    },
    {
        path: '/info/fetch-data',
        element: <FetchData />
    },
    {
        path: '/portal',
        element: <Counter />
    },
    {
        path: '/info/*',
        element: <Counter />
    }
];

export default AppRoutes;
