import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Matches } from "./components/Matches";
import { Results } from "./components/Results";

const AppRoutes = [
    {
        index: true,
        element: <Matches />
    },
    {
        path: '/results',
        element: <Results />
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
