import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../contexts/AuthContext';
import axios from 'axios';

export default function CreateDeck() {
    const NAME_REGEX = new RegExp('^[a-zA-Z0-9]([a-zA-Z0-9 ]){4,32}[a-zA-Z0-9]$');

    const { userId, getAuthentication, logout, config } = useContext(AuthContext)
    const navigate = useNavigate();

    const [name, setName] = useState('');
    const [validName, setValidName] = useState(false);
    const [nameFocus, setNameFocus] = useState(false);

    const [description, setDescription] = useState('');

    const [outputMsg, setOutputMsg] = useState('');
    const [isPending, setIsPending] = useState(false);

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
        else if (getAuthentication() === 0)
            logout();
    }, [])

    useEffect(() => {
        setValidName(NAME_REGEX.test(name));
        setOutputMsg('');
    }, [name])

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!validName)
            setOutputMsg("Invalid username!");
        else if (getAuthentication() === 0)
            logout();
        else {
            setIsPending(true);
            axios.post('api/deck/create', {
                userId: userId,
                Title: name,
                description: description
            }, config)
                .then(res => {
                    navigate('/')
                    setIsPending(false);
                })
                .catch(err => {
                    setOutputMsg(err.response.data)
                    setIsPending(false);
                })
        }
    }

    return (
        <>
            <div>essa?</div>
            <form onSubmit={handleSubmit}>
                <label> Name: </label>
                <input
                    type="text"
                    value={name}
                    required
                    onChange={(e) => setName(e.target.value)}
                    onFocus={() => setNameFocus(true)}
                    onBlur={() => setNameFocus(false)} />
                {nameFocus && !validName &&
                        <ul>
                            <li>Between 6 to 32 characters long</li>
                            <li>Alphanumeric characters</li>
                        </ul>
                }
                <br/>
                <label> Description: </label>
                <input type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
                <button> Next </button>
            </form>
            {outputMsg && <div>{outputMsg}</div>}
        </>
    )
}