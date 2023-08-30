import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Matches } from "./components/Matches";

const AppRoutes = [
    {
        index: true,
        element: <Matches />
    },
    {
        path: '/counter',
        element: <Counter />
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
