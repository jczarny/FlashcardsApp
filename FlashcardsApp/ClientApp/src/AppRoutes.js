import Login from "./components/Login";
import Home from "./components/Home";
import Register from "./components/Register";
import Learn from "./components/Learn";

const AppRoutes = [
  {
      index: true,
      element: <Home />
  },
  {
      path: '/login',
      element: <Login />
  },
  {
      path: '/register',
      element: <Register />
    },
  {
      path: '/learn/:id',
      element: <Learn />
  }
];

export default AppRoutes;
