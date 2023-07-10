import React, { useState, useContext, useEffect, useRef } from 'react';
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../../contexts/AuthContext';

export default function Register() {
    const USERNAME_REGEX = new RegExp('^[a-zA-Z0-9]([a-zA-Z0-9]){3,18}[a-zA-Z0-9]$');
    const PWD_REGEX = new RegExp('^(?=.*?[A-Z])(?=.*?[a-z]).{8,}$');

    const [username, setUsername] = useState('');
    const [validUsername, setValidUsername] = useState(false);
    const [usernameFocus, setUsernameFocus] = useState(false);

    const [password, setPassword] = useState('');
    const [validPassword, setValidPassword] = useState(false);
    const [passwordFocus, SetPasswordFocus] = useState(false);

    const [matchPassword, setMatchPassword] = useState('');
    const [validMatch, setValidMatch] = useState(true);
    const [matchFocus, setMatchFocus] = useState(false);

    const [isPending, setIsPending] = useState(false);
    const [errMsg, setErrMsg] = useState('');
    const [sucMsg, setSucMsg] = useState(false); 

    const navigate = useNavigate();

    useEffect(() => {
        setValidUsername(USERNAME_REGEX.test(username));
    }, [username])

    useEffect(() => {
        setValidPassword(PWD_REGEX.test(password));
        setValidMatch(password === matchPassword);
    }, [password, matchPassword])

    useEffect(() => {
        setErrMsg('');
    }, [username, password, matchPassword])

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log(validPassword)
        if (!validUsername) {
            setErrMsg('Invalid username!')
        }
        else if (!validPassword) {
            setErrMsg('Invalid password')
        }
        else if (!validMatch) {
            setErrMsg('Passwords dont match')
        }
        else {
            setIsPending(true);
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
        }
    }

    return (
        <>

            <div className="m-3 p-5 w-50 mx-auto border">

                <p className="display-4 text-center">Register</p>

                <form onSubmit={handleSubmit}>
                    <div className="form-outline mb-4">
                        <label className="form-label" htmlFor="form2Example1">Username</label>
                        <input
                            type="text"
                            className="form-control"
                            required
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            onFocus={() => setUsernameFocus(true)}
                            onBlur={() => setUsernameFocus(false)}
                        />
                        {usernameFocus && !validUsername &&
                            <div className="p-1 bg-danger-subtle border border-primary-subtle rounded-3">
                                <ul>
                                    <li>Between 5 to 20 characters long</li>
                                    <li>Alphanumeric characters</li>
                                </ul>
                            </div>
                        }
                    </div>

                    <div className="form-outline mb-4">
                        <label className="form-label" htmlFor="form2Example2">Password</label>
                        <input
                            type="password"
                            className="form-control"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            onFocus={() => SetPasswordFocus(true)}
                            onBlur={() => SetPasswordFocus(false)}
                        />
                        {passwordFocus && !validPassword &&
                            <ul>
                                <li>Minimum 8 characters</li>
                                <li>1 capital letter, 1 small letter and 1 number</li>
                            </ul>
                        }
                    </div>

                    <div className="form-outline mb-4">
                        <label className="form-label" htmlFor="form2Example2">Confirm password</label>
                        <input
                            type="password"
                            className="form-control"
                            required
                            value={matchPassword}
                            onChange={(e) => setMatchPassword(e.target.value)}
                            onFocus={() => setMatchFocus(true)}
                            onBlur={() => setMatchFocus(false)}
                        />
                        {matchFocus && !validMatch &&
                            <ul>
                                <li>Must match first password</li>
                            </ul>
                        }
                    </div>
                    {!isPending &&
                        <button type="button" onClick={handleSubmit} className="btn btn-primary btn-block mb-4 ">Sign up</button>}
                    {isPending &&
                        <button disabled type="button" onClick={handleSubmit} className="btn btn-primary btn-block mb-4">Sign up</button>}
                </form>
                {errMsg && <div>{errMsg}</div>}
            </div>
        </>
    )
}