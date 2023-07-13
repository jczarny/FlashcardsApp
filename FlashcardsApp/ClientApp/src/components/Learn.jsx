import React, { useState, useContext, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';
import axios from 'axios';

export default function Learn() {
    const { config, logout } = useContext(AuthContext)
    const { id } = useParams()
    const [isFrontSide, setIsFrontSide] = useState(true)
    const [cardsQueue, setCardsQueue] = useState([])
    const [cardsCounter, setCardsCounter] = useState(1)

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
        if(cardsQueue.length === 0)
            getLearningCards();
    }, [cardsQueue])

    const reverseCard = (e) => {
        setIsFrontSide(false)
    }

    const handleSubmit = (number) => {
        // handle sending stuff to db
        axios.post('api/learn/rate', {}, config)
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

    const getLearningCards = (e) => {
        console.log('getting more learning cards...')
        axios.get(`api/learn`, config)
            .then(res => {
                setCardsQueue(res.data)
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

    return (
        <div className="cardContainer">
            <div className="card">
                <h5 className="card-header">
                    Card #{cardsCounter}
                </h5>
                {isFrontSide && cardsQueue.length !== 0 &&
                    <>
                        <div className="card-body text-center">
                        <h1 className="card-title">{ cardsQueue[0].Front }</h1>
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
                            <button onClick={() => handleSubmit(1)} className="btn btn-primary">Hard (1)</button>
                            <button onClick={() => handleSubmit(2)} className="btn btn-primary">Medium (2)</button>
                            <button onClick={() => handleSubmit(3)} className="btn btn-primary">Easy (3)</button>
                        </div>
                    </>
                }
            </div>
        </div>
    )
}