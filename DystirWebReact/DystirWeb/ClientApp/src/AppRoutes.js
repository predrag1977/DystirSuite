import { Counter } from "./components/Counter";
import { Matches } from "./components/Matches";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/matches',
    element: <Matches />
  }
];

export default AppRoutes;
