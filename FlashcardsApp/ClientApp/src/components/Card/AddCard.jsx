import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';
import "../../custom.css"

export default function AddCard({ deckId, handleAdd }) {
    const { config, logout, getAuthentication } = useContext(AuthContext)

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
        if (front.length > 128 || front.length < 3)
            setValidFront(false)
        else 
            setValidFront(true)
    }, [front])

    useEffect(() => {
        if (reverse.length > 128 || reverse.length < 3)
            setValidReverse(false)
        else
            setValidReverse(true)
    }, [reverse])

    useEffect(() => {
        if (description.length > 128 || (description.length < 3 && description.length !== 0))
            setValidDescription(false)
        else
            setValidDescription(true)
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
            getAuthentication().then(authRes => {
                if (authRes) {
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
                else
                    logout();
            })
        }
    }

    return (
        <div className="cardContainer p-1">
            <div className="card m-3">
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
        </div>
    )
}