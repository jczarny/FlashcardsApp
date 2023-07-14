﻿import React, { useEffect, useState, useContext } from 'react';
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';
import DeckCard from './Deck/DeckCard';

export default function Home() {
    const { userId, setUserId, accessToken, setAccessToken, getAuthentication, config, logout } = useContext(AuthContext)
    const [decks, setDecks] = useState([])
    const navigate = useNavigate();

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
        else if (getAuthentication() === 0)
            logout();
        else {
            axios.get(`api/user/owned-decks?id=${userId}`, config)
                .then(res => {
                    console.log(res.data)
                    setDecks(res.data)
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

    const handleLearn = id => {
        navigate(`/learn/${id}`)
    }

    const handleEdit = id => {
        navigate(`/edit/${id}`);
    }

return (
    <>
        <div className="m-3 p-5 w-85 mx-auto border">
            <p className="display-4">Your decks</p>
            <div className="container text-center">
                <div className="row row-cols-2">
                    {
                        decks.map((deck) =>
                            <DeckCard key={deck.Id} title={deck.Title} creatorId={deck.CreatorId}
                                description={deck.Description} id={deck.Id} amountToRevise={deck.CardsToRevise}
                                handleLearn={handleLearn} handleEdit={handleEdit} />
                        )
                    }
                </div>
            </div>
        </div>
    </>
)
}