import React, { useState, useContext } from 'react';
import { useNavigate, Link } from "react-router-dom";
import axios from 'axios';
import '../../custom.css'
import { AuthContext } from '../../contexts/AuthContext';

export default function Login() {
    const { userId, setUserId, accessToken, setAccessToken } = useContext(AuthContext)
    const [username, setUsername] = useState('EssaPlejer15');
    const [password, setPassword] = useState('Eurobeat123');
    const [isPending, setIsPending] = useState(false);
    const [validCred, setValidCred] = useState(true);
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
                setValidCred(false)
            })
    }

    return (
        <>
            <div className="m-3 p-5 w-50 mx-auto border">

                {!validCred &&
                    <div className="p-1 bg-danger border border-primary-subtle rounded-3">
                        <p className="text-center fw-bold"> Invalid credentials! </p>
                    </div>}

                <p className="display-4 text-center">Login</p>

                <form onSubmit={handleSubmit}>
                    <div className="form-outline mb-4">
                        <label className="form-label" htmlFor="form2Example1">Username</label>
                        <input
                            type="text"
                            className="form-control"
                            required
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                        />
                    </div>

                    <div className="form-outline mb-4">
                        <label className="form-label" htmlFor="form2Example2">Password</label>
                        <input
                            type="password"
                            className="form-control"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                    </div>
                    {!isPending &&
                        <button type="button" onClick={handleSubmit} className="btn btn-primary btn-block mb-4">Sign in</button>}
                    {isPending &&
                        <button disabled type="button" onClick={handleSubmit} className="btn btn-primary btn-block mb-4">Sign in</button>}
                    <Link className="ms-2" to="../register">Sign up</Link>
                </form>
            </div>
        </>
    )
}