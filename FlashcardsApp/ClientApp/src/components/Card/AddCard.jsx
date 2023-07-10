import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';

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
            <form>
                <h3>New card</h3>
                <div>
                    <label>Front: </label>
                    <input
                        type="text"
                        required
                        value={front}
                        onChange={(e) => setFront(e.target.value)}
                        onFocus={() => setFrontFocus(true)}
                        onBlur={() => setFrontFocus(false)}
                    />
                    {frontFocus && !validFront &&
                        <div className="p-1 bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <li>Between 3 and 128 characters long</li>
                                <li>Alphanumeric characters</li>
                            </ul>
                        </div>
                    }
                </div>

                <div>
                    <label>Reverse: </label>
                    <input
                        type="text"
                        required
                        value={reverse}
                        onChange={(e) => setReverse(e.target.value)}
                        onFocus={() => setReverseFocus(true)}
                        onBlur={() => setReverseFocus(false)}
                    />
                    {reverseFocus && !validReverse &&
                        <div className="p-1 bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <li>Between 3 and 128 characters long</li>
                                <li>Alphanumeric characters</li>
                            </ul>
                        </div>
                    }
                </div>
                    
                <div>
                    <label>Description: </label>
                    <input
                        type="text"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        onFocus={() => setDescriptionFocus(true)}
                        onBlur={() => setDescriptionFocus(false)}
                    />
                    {descriptionFocus && !validDescription &&
                        <div className="p-1 bg-danger-subtle border border-primary-subtle rounded-3">
                            <ul>
                                <li>Between 3 to 128 characters long </li>
                                <li>or empty</li>
                            </ul>
                        </div>
                    }
                </div>
                {!isPending &&
                    <button type="button" onClick={handleSubmit}> Add Card </button>}
                {isPending &&
                    <button type="button" disabled onClick={handleSubmit}> Adding... </button>}
                {outputMsg && <div>{outputMsg}</div> }
            </form>

        </>
    )
}