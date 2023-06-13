import Login from "./components/Login";
import Home from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/login',
    element: <Login />
  }
];

export default AppRoutes;
