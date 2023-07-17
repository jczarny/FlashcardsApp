import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';
import DeckItem from "./DeckItem";

export default function BrowseDecks() {
    const { getAuthentication, logout } = useContext(AuthContext)
    const [publicDecks, setPublicDecks] = useState([])
    const [ownedDecksIds, setOwnedDecksIds] = useState([])

    useEffect(() => {
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.get(`api/deck/getPublic`, authHeader)
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

                axios.get(`api/user/owned-decks`, authHeader)
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
            else
                logout();
        })
    }, [])

    const handleAcquire = deckId => {
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.post(`api/user/acquire?id=` + deckId, {}, authHeader)
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
                        else
                            console.log(err.response)
                    })
            }
            else
                logout();
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
                            <th scope="col">Get</th>
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