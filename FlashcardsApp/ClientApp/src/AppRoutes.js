import Login from "./components/Login";
import Home from "./components/Home";
import Register from "./components/Register";
import Learn from "./components/Learn";
import CreateDeck from "./components/CreateDeck";
import EditDeck from "./components/EditDeck";

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
  },
  {
      path: '/create',
      element: <CreateDeck />
  },
  {
      path: '/edit/:id',
      element: <EditDeck />
  }
];

export default AppRoutes;
