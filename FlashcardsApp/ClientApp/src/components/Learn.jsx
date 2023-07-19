import React, { useState, useContext, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';
import { useNavigate } from "react-router-dom";
import axios from 'axios';

export default function Learn() {
    const { config, logout, getAuthentication } = useContext(AuthContext)
    const { deckId } = useParams()

    const [isFrontSide, setIsFrontSide] = useState(true)
    const [cardsQueue, setCardsQueue] = useState([])
    const [cardsCounter, setCardsCounter] = useState(1)
    const [noCardsLeft, setNoCardsLeft] = useState(false)
    const [firstLoading, setFirstLoading] = useState(true)

    const navigate = useNavigate();

    const handleKeyDown = e => {
        if (e.key === ' ' && isFrontSide)
            reverseCard()
        if (['1', '2', '3'].includes(e.key) && !isFrontSide)
            handleSubmit(e.keyCode - 48)
    }

    useEffect(() => {
        // setup shortcuts for learning
        document.addEventListener('keydown', handleKeyDown)
        return () => {
            document.removeEventListener('keydown', handleKeyDown)
        }
    }, [handleKeyDown])

    useEffect(() => {
        // get new cards to learn from from database
        if (cardsQueue.length === 0)
            getLearningCards();
    }, [cardsQueue])

    const reverseCard = (e) => {
        setIsFrontSide(false)
    }

    const handleSubmit = (number) => {
        // handle sending stuff to db
        console.log(cardsQueue[0].Id, parseInt(deckId, 10), number)

        getAuthentication().then(authHeader => {
            if (authHeader) {
                axios.post('api/learn/evaluate',
                    {
                        CardId: cardsQueue[0].Id,
                        DeckId: parseInt(deckId, 10),
                        Response: number
                    }, authHeader)
                    .then(res => {
                        // set front side to true
                        setIsFrontSide(true)

                        // pop the card we were currently using
                        setCardsQueue((cards) => cards.filter((card, index) => index !== 0))

                        // increment cards counter
                        setCardsCounter(oldCount => oldCount + 1)
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
                logout()
        })
    }

    const getLearningCards = (e) => {
        console.log('getting more learning cards...')
        getAuthentication().then(authRes => {
            if (authRes) {
                axios.get(`api/learn?deckId=${deckId}&amount=5`, config)
                    .then(res => {
                        if (Object.keys(res.data).length !== 0)
                            setCardsQueue(res.data)
                        else
                            setNoCardsLeft(true)

                        setFirstLoading(false)
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
                logout()
        })
    }

    const handleGoBack = e => {
        navigate(`/`)
    }

    return (
        <>
            {!noCardsLeft && !firstLoading &&
                <>
                    <div className="cardContainer">
                        <div className="card">
                            <h5 className="card-header">
                                Card #{cardsCounter}
                            </h5>

                            {isFrontSide && cardsQueue.length !== 0 &&
                                <>
                                    <div className="card-body text-center">
                                        <h1 className="card-title">{cardsQueue[0].Front}</h1>
                                    </div>
                                    <div className="card-footer">
                                        <button onClick={() => reverseCard()} className="btn btn-primary">Check (backspace)</button>
                                    </div>
                                </>
                            }

                            {!isFrontSide &&
                                <>
                                    <div className="card-body text-center">
                                        <h1 className="card-title">{cardsQueue[0].Reverse}</h1>
                                        <div> {cardsQueue[0].Description}</div>
                                    </div>
                                    <div className="card-footer">
                                        <button onClick={() => handleSubmit(1)} className="btn btn-danger">Again (1)</button>
                                        <button onClick={() => handleSubmit(2)} className="btn btn-warning">Hard (2)</button>
                                        <button onClick={() => handleSubmit(3)} className="btn btn-info">Medium (3)</button>
                                        <button onClick={() => handleSubmit(4)} className="btn btn-success">Easy (4)</button>
                                    </div>
                                </>
                            }
                        </div>
                    </div>
                </>

            }
            {noCardsLeft && !firstLoading &&
                <>
                    <div className="cardContainer">
                        <div className="card">
                            <h5 className="card-header">
                               
                            </h5>

                            <div className="card-body text-center">
                                <h1 className="card-title">Good Job!</h1>
                                <div> That's the end for today, come back tomorrow to learn more!</div>
                            </div>
                            <div className="card-footer">
                            <button onClick={() => handleGoBack()} className="btn btn-primary">Back</button>
                            </div>

                        </div>
                    </div>
                </>
            }
        </>

    )
}