import Login from "./components/Login/Login";
import Home from "./components/Home";
import Register from "./components/Login/Register";
import Learn from "./components/Learn";
import CreateDeck from "./components/Deck/CreateDeck";
import EditDeck from "./components/Deck/EditDeck";
import BrowseDecks from "./components/Deck/BrowseDecks";
import Logout from "./components/Login/Logout";

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
        path: '/learn/:deckId',
        element: <Learn />
    },
    {
        path: '/create',
        element: <CreateDeck />
    },
    {
        path: '/edit/:id',
        element: <EditDeck />
    },
    {
        path: '/browse',
        element: <BrowseDecks />
    },
    {
        path: '/logout',
        element: <Logout />
    }
];

export default AppRoutes;
