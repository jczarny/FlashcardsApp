import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';

export default function Register() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isPending, setIsPending] = useState(false);
    const [errMsg, setErrMsg] = useState('');

    const navigate = useNavigate();

    useEffect(() => {
        const match = password === confirmPassword
        if (match === false)
            setErrMsg('Passwords doesnt match!')
        else 
            setErrMsg('')
    }, [password, confirmPassword])

    const handleSubmit = (e) => {
        e.preventDefault();
        setIsPending(true);

        if (!errMsg)
            axios.post('api/auth/register', {
                username: username,
                password: password
            })
                .then(res => {
                    navigate('/login')
                    setIsPending(false)
                })
                .catch(err => {
                    setErrMsg(err.response.data)
                    setIsPending(false)
                })
        else {
            setIsPending(false)
        }

    }

    return (
        <>
            <h2> Registration form</h2>
            <form onSubmit={handleSubmit}>
                <label>username: </label>
                <input
                    type="text"
                    required
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />
                <label>password: </label>
                <input
                    type="password"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                <label>confirm password: </label>
                <input
                    type="password"
                    required
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                />
                {!isPending && <button>Confirm</button>}
                {isPending && <button disabled> Registering </button>}
                {errMsg && <div>{errMsg}</div> }
            </form>
        </>
    )
}