import React, { Component, createContext, useContext, useState } from 'react';
import axios from 'axios';

export const AuthContext = createContext();

export default function AuthContextProvider({ children }) {
    const [userId, setUserId] = useState('')
    const [accessToken, setAccessToken] = useState('')
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
                if (err.response.status === 401) {
                    return 0;
                }
            })
    }

    return (
        <AuthContext.Provider value={{ userId, setUserId, accessToken, setAccessToken, getAuthentication, config }}>
            { children }
        </AuthContext.Provider>
    )
}