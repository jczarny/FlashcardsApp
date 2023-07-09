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

    const categories = {ALL: 'all', NEW: 'new'}
    const [chosenCategory, setChosenCategory] = useState(categories.ALL)

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
                    setAllCards(res.data.CardDtos)
                    setAllCards(cards =>
                        cards.map((card,index) => ({
                            ...card,
                            No: index
                        }))
                    )
                    console.log(allCards)
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
        setAllCards((oldCards) =>
            oldCards.filter((card) => card.Id !== id))
    }

    const handleAdd = card => {
        setAddedCards(oldCards => [...oldCards, { ...card, No: oldCards.length }])
        setAllCards(oldCards => [...oldCards, { ...card, No: oldCards.length }])
    }

    const handleCardList = decision => {
        if (decision === categories.NEW && chosenCategory === categories.ALL)
            setChosenCategory(categories.NEW)
        else if (decision === categories.ALL && chosenCategory === categories.NEW)
            setChosenCategory(categories.ALL)
    }

    return (
        <>
            <h1>{deck.Title}</h1>
            
            <AddCard deckId={id} handleAdd={handleAdd} />

            <button onClick={() => handleCardList(categories.NEW) }> Recently Added </button>
            <button onClick={() => handleCardList(categories.ALL)}> All Cards </button>

            {
                chosenCategory === categories.NEW &&
                <CardList title="New cards" cards={addedCards} HandleDelete={handleDelete} deleteMsg={deleteMsg} />
            }
            {
                chosenCategory === categories.ALL &&
                <CardList title="All cards" cards={allCards} HandleDelete={handleDelete} deleteMsg={deleteMsg} />
            }
        </>
    )
}