import React, { useEffect, useState, useContext } from 'react';
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';
import DeckCard from './Deck/DeckCard';

export default function Home() {
    const { getAuthentication, logout } = useContext(AuthContext)
    const [decks, setDecks] = useState([])
    const navigate = useNavigate();

    useEffect(() => {
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.get(`api/user/owned-decks`, authHeader)
                    .then(res => {
                        setDecks(res.data)
                        setDecks(decks => [...decks, { Id: -1 }])
                    })
                    .catch(err => {
                        if (err.response.status === 401) {
                            console.log('Not authorised')
                            logout()
                        }
                        else if (err.response.status === 404)
                            console.log('Not found');
                        else
                            console.log(err)
                    })
            }
            else
                logout();
        })

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