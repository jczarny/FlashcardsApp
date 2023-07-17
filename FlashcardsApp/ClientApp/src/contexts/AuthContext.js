import React, { Component, createContext, useContext, useState } from 'react';
import axios from 'axios';
import { useNavigate } from "react-router-dom";

export const AuthContext = createContext();

export default function AuthContextProvider({ children }) {
    const [accessToken, setAccessToken] = useState('')
    const [isLoggedIn, setIsLoggedIn] = useState(false)

    const navigate = useNavigate();

    const config = {
        headers: {
            authorization: 'Bearer ' + accessToken
        }
    };

    function getAuthentication() {
        console.log("authenticating..." + config.headers.authorization)
        return axios.post('api/auth/refresh-tokens', {}, config)
            .then(res => {
                const cfg = {
                    headers: {
                        Authorization: 'Bearer ' + res.data.accessToken
                        }
                }
                setIsLoggedIn(true);
                setAccessToken(res.data.accessToken)
                return cfg;
            })
            .catch(err => {
                setIsLoggedIn(false);
                return false;
            })
    }
     
    function logout() {
        axios.post('api/auth/logout', {}, config)
            .then(res => {
                navigate('/login')
                setAccessToken('')
                setIsLoggedIn(false)
            })
            .catch(err => {
                console.log(err.response);
                //console.log('Couldnt log out');
                navigate('/login')
                setAccessToken('')
                setIsLoggedIn(false)
            })
    }

    return (
        <AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn, accessToken, setAccessToken, getAuthentication, config, logout }}>
            { children }
        </AuthContext.Provider>
    )
}