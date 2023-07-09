import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../contexts/AuthContext';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import AddCard from "./AddCard";
import CardList from './CardList';

export default function EditDeck() {
    const { userId, getAuthentication, config, logout } = useContext(AuthContext)
    const [deck, setDeck] = useState('')

    const [addedCards, setAddedCards] = useState([])
    const [allCards, setAllCards] = useState([])

    const { id } = useParams()
    const navigate = useNavigate();

    const [deleteMsg, setDeleteMsg] = useState('');

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
                    console.log(res.data)
                    setAllCards(res.data.CardDtos)
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

    const handleDelete = id => {
        axios.delete('api/deck/card?id=' + id, config)
            .then(res => {
                setDeleteMsg('Card successfully deleted')
            })
            .catch(err => {
                setDeleteMsg(err.response.data)
            })

        setAddedCards((oldCards) =>
            oldCards.filter((card) => card.Id !== id))
    }

    const handleAdd = card => {
        setAddedCards(oldCards => [...oldCards, { ...card, No: oldCards.length }])
    }

    return (
        <>
            <h1>{deck.Title}</h1>
            
            <AddCard deckId={id} handleAdd={handleAdd} />

            <button>Recently Added </button>

            <CardList title="Added cards" cards={addedCards} HandleDelete={handleDelete} deleteMsg={deleteMsg} />
            <CardList title="All cards" cards={allCards} HandleDelete={handleDelete} deleteMsg={deleteMsg } />
        </>
    )
}