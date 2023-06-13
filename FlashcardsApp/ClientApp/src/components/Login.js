import React, { useState, useContext } from 'react';
import { useNavigate, Link } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';

export default function Login() {
    const { userId, setUserId, accessToken, setAccessToken } = useContext(AuthContext)
    const [username, setUsername] = useState('faewafa');
    const [password, setPassword] = useState('pwd1111');
    const [isPending, setIsPending] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();
        setIsPending(true);

        axios.post('api/auth/login', {
            username: username,
            password: password
        })
            .then(res => {
                setAccessToken(res.data.accessToken)
                setUserId(res.data.userId)
                setIsPending(false)
                navigate('/')
            })
            .catch(err => {
                console.log(err.response.status)
                setIsPending(false)
            })
    }

    return (
        <>
            <h2> Login </h2>
            <form onSubmit = { handleSubmit }>
                <label>username: </label>
                <input
                    type="text"
                    required
                    value={username}
                    onChange={(e) => setUsername(e.target.value) }
                />
                <label>password: </label>
                <input
                    type="password"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                { !isPending && <button>Confirm</button> }
                { isPending && <button disabled> Logging in </button> }
            </form>
            <Link to="../register">Sign up</Link>
        </>
    )
}