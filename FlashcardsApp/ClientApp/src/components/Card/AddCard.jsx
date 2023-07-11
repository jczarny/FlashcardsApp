﻿import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';
import "../../custom.css"

export default function AddCard({ deckId, handleAdd }) {
    const CARD_REGEX = new RegExp('^[a-zA-Z0-9]([a-zA-Z0-9 ]){1,32}[a-zA-Z0-9]$');
    const { config } = useContext(AuthContext)

    const [front, setFront] = useState('');
    const [validFront, setValidFront] = useState(false);
    const [frontFocus, setFrontFocus] = useState(false);

    const [reverse, setReverse] = useState('');
    const [validReverse, setValidReverse] = useState(false);
    const [reverseFocus, setReverseFocus] = useState(false);

    const [description, setDescription] = useState('');
    const [validDescription, setValidDescription] = useState(false);
    const [descriptionFocus, setDescriptionFocus] = useState(false);

    const [outputMsg, setOutputMsg] = useState('');
    const [isPending, setIsPending] = useState(false);

    useEffect(() => {
        setValidFront(CARD_REGEX.test(front));
    }, [front])

    useEffect(() => {
        setValidReverse(CARD_REGEX.test(reverse));
    }, [reverse])

    useEffect(() => {
        setValidDescription(CARD_REGEX.test(description) || description === "");
    }, [description])

    useEffect(() => {
        setOutputMsg('');
    }, [frontFocus, reverseFocus, descriptionFocus])

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!validFront) {
            setOutputMsg('Invalid Front!')
        }
        else if (!validReverse) {
            setOutputMsg('Invalid Reverse')
        }
        else if (!validDescription) {
            setOutputMsg('Invalid Description!')
        }
        else {
            setIsPending(true);
            axios.post('api/deck/addcard', {
                Id: "-1",
                DeckId: deckId,
                Front: front,
                Reverse: reverse,
                Description: description
            }, config)
                .then(res => {
                    const newCard = {
                        Id: res.data,
                        Front: front,
                        Reverse: reverse,
                        Description: description
                        }
                    handleAdd(newCard)
                    setFront('')
                    setReverse('')
                    setDescription('')
                    setOutputMsg('Card successfully added.')
                    setIsPending(false)
                })
                .catch(err => {
                    setOutputMsg(err)
                    setIsPending(false)
                })
        }
    }

    return (
        <>
            <div className="card m-5">
                <div className="card-header">
                    New card
                </div>
                <div className="card-body text-center">
                    <input
                        className="form-outline w-50 mb-2"
                        type="text"
                        required
                        placeholder="Front"
                        size="30"
                        value={front}
                        onChange={(e) => setFront(e.target.value)}
                        onFocus={() => setFrontFocus(true)}
                        onBlur={() => setFrontFocus(false)}
                    />
                    <br />
                    <input
                        className="form-outline w-50 mb-2"
                        type="text"
                        required
                        placeholder="Reverse"
                        size="30"
                        value={reverse}
                        onChange={(e) => setReverse(e.target.value)}
                        onFocus={() => setReverseFocus(true)}
                        onBlur={() => setReverseFocus(false)}
                    />
                    < br />
                    <textarea
                        className="form-outline w-50 mb-2 p-1"
                        type="text"
                        placeholder="Description"
                        value={description}
                        size="30"
                        cols="60"
                        rows="6"
                        onChange={(e) => setDescription(e.target.value)}
                        onFocus={() => setDescriptionFocus(true)}
                        onBlur={() => setDescriptionFocus(false)}
                    />
                    < br />
                    {!isPending &&
                        <a href="#" onClick={handleSubmit} className="btn btn-primary">Add Card</a>
                    }
                    {isPending &&
                        <a href="#" onClick={handleSubmit} disabled className="btn btn-primary"> Adding... </a>}
                    {outputMsg && <div>{outputMsg}</div>}

                    {frontFocus && !validFront &&
                        <div className="d-flex m-3 align-items-center justify-content-center bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <div>Between 3 and 128 characters long</div>
                                <div>Alphanumeric characters</div>
                            </ul>
                        </div>
                    }
                    {reverseFocus && !validReverse &&
                        <div className="d-flex m-3 align-items-center justify-content-center bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <div>Between 3 and 128 characters long</div>
                                <div>Alphanumeric characters</div>
                            </ul>
                        </div>
                    }
                    {descriptionFocus && !validDescription &&
                        <div className="d-flex m-3 align-items-center justify-content-center bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <div>Between 3 to 128 characters long or empty </div>
                            </ul>
                        </div>
                    }
                </div>
            </div>
        </>
    )
}