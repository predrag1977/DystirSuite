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
    path: '/fetch-data',
    element: <FetchData />
  }
];

export default AppRoutes;
