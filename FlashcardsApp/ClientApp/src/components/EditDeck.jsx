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

    const [isOwner, setIsOwner] = useState('')

    const [addedCards, setAddedCards] = useState([])
    const [allCards, setAllCards] = useState([])

    const categories = {ALL: 'all', NEW: 'new'}
    const [chosenCategory, setChosenCategory] = useState(categories.ALL)

    const { id } = useParams()
    const navigate = useNavigate();

    const [deleteMsg, setDeleteMsg] = useState('');
    const [infoMsg, setInfoMsg] = useState('');

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
                    if (res.data.CreatorId === userId)
                        setIsOwner(' (Owner) ')
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

    const handleDeleteCard = id => {
        if (deck.CreatorId === userId) {
            axios.delete('api/deck/card?id=' + id, config)
                .then(res => {
                    setDeleteMsg('Card successfully deleted')
                    setAddedCards((oldCards) =>
                        oldCards.filter((card) => card.Id !== id))
                    setAllCards((oldCards) =>
                        oldCards.filter((card) => card.Id !== id))
                })
                .catch(err => {
                    setDeleteMsg(err.response.data)
                })
        }
        else {
            setDeleteMsg('Only owner can delete cards!')
        }
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

    const handlePublish = () => {
        axios.patch('api/deck/publish?id=' + deck.Id, {}, config)
            .then(res => {
                setInfoMsg('Deck published!')
                setDeck(oldDeck => ({ ...oldDeck, isPrivate: false }))
            })
            .catch(err => {
                setDeleteMsg(err.response.data)
            })
    }

    const handleDeleteDeck = () => {
        axios.delete('api/deck?id=' + id, config)
            .then(res => {
                setDeleteMsg('Deck successfully deleted')
                navigate('/');
            })
            .catch(err => {
                setDeleteMsg(err.response.data)
            })
    }

    return (
        <>
            <h1>{deck.Title + isOwner}</h1>
            {
                deck.isPrivate &&
                <button onClick={() => handlePublish()}> Make public  </button>
            }
            {
                !deck.isPrivate &&
                <button disabled> Make public </button>
            }
            <button onClick={() => handleDeleteDeck()}> Delete deck </button>

            {infoMsg && <div>{infoMsg}</div>}

            {
                deck.CreatorId === userId && 
                <>
                    <AddCard deckId={id} handleAdd={handleAdd} /> 
                    <button onClick={() => handleCardList(categories.NEW)}> Recently Added </button>
                    <button onClick={() => handleCardList(categories.ALL)}> All Cards </button>
                </>
            }

            {
                chosenCategory === categories.NEW &&
                <CardList title="New cards" cards={addedCards} HandleDelete={handleDeleteCard} deleteMsg={deleteMsg} />
            }
            {
                chosenCategory === categories.ALL &&
                <CardList title="All cards" cards={allCards} HandleDelete={handleDeleteCard} deleteMsg={deleteMsg} />
            }
        </>
    )
}