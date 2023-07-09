import React, { Component, createContext, useContext, useState } from 'react';
import axios from 'axios';
import { useNavigate } from "react-router-dom";

export const AuthContext = createContext();

export default function AuthContextProvider({ children }) {
    const [userId, setUserId] = useState('')
    const [accessToken, setAccessToken] = useState('')
    const navigate = useNavigate();

    const config = {
        headers: {
            userId: userId,
            'Authorization': 'Bearer ' + accessToken
        }
    };

    function getAuthentication() {
        axios.post('api/auth/refresh-token', {}, config)
            .then(res => {
                setAccessToken(res.data.accessToken)
                return 1;
            })
            .catch(err => {
                return 0;
            })
    }

    function logout() {
        axios.post('api/auth/logout', {}, config)
            .then(res => {
                navigate('/')
                setAccessToken('')
                setUserId('')
            })
            .catch(err => {
                console.log(err.response.status);
                console.log('Couldnt log out');
            })
        navigate('/login')
    }

    return (
        <AuthContext.Provider value={{ userId, setUserId, accessToken, setAccessToken, getAuthentication, config, logout }}>
            { children }
        </AuthContext.Provider>
    )
}