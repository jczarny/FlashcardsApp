import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';
import DeckItem from "./DeckItem";

export default function BrowseDecks() {
    const { userId, getAuthentication, config, logout } = useContext(AuthContext)
    const [publicDecks, setPublicDecks] = useState([])
    const [ownedDecksIds, setOwnedDecksIds] = useState([])
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
                    setPublicDecks(res.data)
                    setPublicDecks(decks =>
                        decks.map((deck, index) => ({
                            ...deck,
                            No: index
                        })))
                })
                .catch(err => {
                    if (err.response.status === 401) {
                        console.log('Not authorised')
                        logout()
                    }
                    else if (err.response.status === 404)
                        console.log('Not found');
                })

            axios.get(`api/user/owned-decks?id=${userId}`, config)
                .then(res => {
                    setOwnedDecksIds(res.data)
                    setOwnedDecksIds(decks =>
                        decks.map(deck => deck.Id))
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

    const handleAcquire = deckId => {
        axios.post(`api/user/acquire?id=` + deckId, {}, config)
            .then(res => {
                setOwnedDecksIds(oldIds => [...oldIds, deckId])
            })
            .catch(err => {
                if (err.response.status === 401) {
                    console.log('Unauthorized')
                    logout()
                }
                else if (err.response.status === 404)
                    console.log('Not found');
            })
    }

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
                            publicDecks.map((deck) =>
                                <DeckItem key={deck.Id} Id={deck.Id} No={deck.No} Author={deck.CreatorName} Title={deck.Title}
                                    Description={deck.Description} ownedDecks={ownedDecksIds} handleAcquire={handleAcquire} />
                            )
                        }
                    </tbody>
                </table>
            </>
        </>
    )
}