import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../contexts/AuthContext';
import axios from 'axios';
import DeckItem from "./DeckItem";

export default function CreateDeck() {
    const { userId, getAuthentication, config, logout } = useContext(AuthContext)
    const [decks, setDecks] = useState([])
    const navigate = useNavigate()

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
        else if (getAuthentication() === 0)
            logout();
        else {
            axios.get(`api/deck/getPublic`, config)
                .then(res => {
                    setDecks(res.data)
                    setDecks(decks =>
                        decks.map((deck, index) => ({
                            ...deck,
                            No: index
                        }))
                    )
                })
                .catch(err => {
                    if (err.response.status === 401) {
                        console.log('Not authorised')
                        logout()
                    }
                    else if (err.response.status === 404)
                        console.log('Not found');
                })
        }
    }, [])

    return (
        <>
            <>
                <h3> Browse Decks </h3>
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Author</th>
                            <th scope="col">Title</th>
                            <th scope="col">Description</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            decks.map((deck) =>
                                <DeckItem key={deck.Id} No={deck.No} Author={deck.CreatorName} Title={deck.Title}
                                    Description={deck.Description} />
                            )
                        }
                    </tbody>
                </table>
            </>
        </>
    )
}