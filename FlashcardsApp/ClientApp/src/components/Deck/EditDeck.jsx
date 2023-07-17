import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../../contexts/AuthContext';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import AddCard from "../Card/AddCard";
import CardList from '../Card/CardList';

export default function EditDeck() {
    const { userId, getAuthentication, logout } = useContext(AuthContext)
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
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.get(`api/deck?deckId=${id}`, authHeader)
                    .then(res => {
                        setDeck(res.data)
                        setAllCards(res.data.CardDtos)
                        setAllCards(cards =>
                            cards.map((card, index) => ({
                                ...card,
                                No: index
                            }))
                        )
                        if (res.data.IsOwner)
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
            else
                logout();
        })
    }, [])

    const handleDeleteCard = id => {
        getAuthentication().then(authHeader => {
            if (authHeader) {
                if (deck.IsOwner) {
                    axios.delete('api/deck/card?id=' + id, authHeader)
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
            else
                logout();
        })
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
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.patch('api/deck/publish?id=' + deck.Id, {}, authHeader)
                    .then(res => {
                        setInfoMsg('Deck published!')
                        setDeck(oldDeck => ({ ...oldDeck, isPrivate: false }))
                    })
                    .catch(err => {
                        setDeleteMsg(err.response.data)
                    })
            }
            else
                logout();
        })
    }

    const handleDeleteDeck = () => {
        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.delete('api/deck?id=' + id, authHeader)
                    .then(res => {
                        setDeleteMsg('Deck successfully deleted')
                        navigate('/');
                    })
                    .catch(err => {
                        setDeleteMsg(err.response.data)
                    })
            }
            else
                logout();
        })
    }

    return (
        <>
            <h1>{deck.Title + isOwner}</h1>

            <h3> Manage deck </h3>
            {
                deck.isPrivate &&
                <button type="button" onClick={() => handlePublish()} className="btn btn-bg btn-primary me-2">Make public</button>
            }
            {
                !deck.isPrivate &&
                <button type="button" className="btn btn-bg btn-primary me-3" disabled>Make public</button>
            }
            <button onClick={() => handleDeleteDeck()} className="btn btn-bg btn-primary me-2"> Delete deck </button>
            {infoMsg && <div>{infoMsg}</div>}

            {
                deck.IsOwner && 
                <>
                    <AddCard deckId={id} handleAdd={handleAdd}/>
                    <div className="text-center">
                        <button onClick={() => handleCardList(categories.NEW)} className="btn btn-bg btn-primary me-3"> Recently Added </button>
                        <button onClick={() => handleCardList(categories.ALL)} className="btn btn-bg btn-primary me-3"> All Cards </button>
                    </div>
                </>
            }

            {
                chosenCategory === categories.NEW &&
                <div className="mt-4">
                        <CardList title="New cards" cards={addedCards} HandleDelete={handleDeleteCard} deleteMsg={deleteMsg} />
                </div>
            }
            {
                chosenCategory === categories.ALL &&
                <div className="mt-4">
                    <CardList title="All cards" cards={allCards} HandleDelete={handleDeleteCard} deleteMsg={deleteMsg} />
                </div>
            }
        </>
    )
}