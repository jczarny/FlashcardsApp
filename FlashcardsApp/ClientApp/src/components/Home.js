import React, { useEffect, useState, useContext } from 'react';
import { useNavigate } from "react-router-dom";
import Cookies from 'universal-cookie';
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';

export default function Home() {
    const { userId, setUserId, accessToken, setAccessToken, getAuthentication, config } = useContext(AuthContext)
    const navigate = useNavigate();

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
    }, [])

    function logout() {
        axios.post('api/auth/logout', {}, config)
            .then(res => {
                navigate('/')
                setAccessToken('')
                setUserId('')
                navigate('/login')
            })
            .catch(err => {
                console.log(err.response.status);
                console.log('Couldnt log out');
            })
    }

    function doSomething() {
        if (getAuthentication() === 0)
            logout();
        axios.post('api/user/login', {}, config)
            .then(res => {
                console.log(res)
            })
            .catch(err => {
                if (err.response.status === 401) {
                    const x = doSomething();
                }
                console.log(err.response.status);
                console.log('Not authorised');
            })
    }


return (
    <>
        <div>sample</div>
        <button onClick={logout}>Wyloguj</button>
        <button onClick={doSomething}>Autoryzowany przycisk</button>
        {accessToken && <div>Access Token: { accessToken }</div> }
    </>
)
}