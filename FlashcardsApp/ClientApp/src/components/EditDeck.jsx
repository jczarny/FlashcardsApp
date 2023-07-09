import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../contexts/AuthContext';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import AddCard from "./AddCard";

export default function EditDeck() {
    const { userId, getAuthentication, config, logout } = useContext(AuthContext)
    const [deck, setDeck] = useState('')
    const { id } = useParams()
    const navigate = useNavigate();

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
        else if (getAuthentication() === 0)
            logout();
        else {
            axios.get(`api/deck?deckId=${id}`, config)
                .then(res => {
                    setDeck(res.data)
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
            <h1>{deck.Title}</h1>
            
            <AddCard deckId={id} />
        </>
    )
}