import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import axios from 'axios';
import CardItem from './CardItem';

export default function AddCard({ deckId }) {
    const CARD_REGEX = new RegExp('^[a-zA-Z0-9]([a-zA-Z0-9 ]){1,32}[a-zA-Z0-9]$');
    const { config } = useContext(AuthContext)

    const [addedCards, setAddedCards] = useState([])
    
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
            setOutputMsg('Description is too long!')
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
                    setAddedCards(oldCards => [...oldCards, {
                        Id: res.data,
                        Front: front,
                        Reverse: reverse,
                        Description: description,
                        No: oldCards.length
                    }])
                    setFront('')
                    setReverse('')
                    setDescription('')
                    setOutputMsg('Card successfully added.')
                    setIsPending(false)
                })
                .catch(err => {
                    setOutputMsg(err.response.data)
                    setIsPending(false)
                })
        }
    }

    const handleDelete = id => {
        axios.delete('api/deck/card?id=' + id, config)
            .then(res => {
                setOutputMsg('Card deleted')
            })
            .catch(err => {
                setOutputMsg(err.response.data)
                setIsPending(false)
            })

        setAddedCards((oldCards) => 
            oldCards.filter((card) => card.Id !== id))
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
                <button type="button" onClick={handleSubmit}> Add Card </button>
                {outputMsg && <div>{outputMsg}</div> }
            </form>

            <h3> Added cards </h3>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Front</th>
                        <th scope="col">Reverse</th>
                        <th scope="col"> </th>
                    </tr>
                </thead>
                <tbody>
                    {
                        addedCards.map((card) =>
                            <CardItem key={card.No} Id={card.Id} No={card.No} Front={card.Front}
                                Reverse={card.Reverse} HandleDelete={handleDelete} />
                        )
                    }
                </tbody>
            </table>
        </>
    )
}