import React, { useEffect, useState, useContext } from 'react';
import { useNavigate } from "react-router-dom";
import Cookies from 'universal-cookie';
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';
import DeckItem from './DeckItem';

export default function Home() {
    const { userId, setUserId, accessToken, setAccessToken, getAuthentication, config } = useContext(AuthContext)
    const [decks, setDecks] = useState([])
    const navigate = useNavigate();

    useEffect(() => {
        if (!userId) {
            navigate('/login');
        }
        else if (getAuthentication() === 0)
            logout();
        else {
            axios.get(`api/user/owned-decks?id=${userId}`, config)
                .then(res => {
                    setDecks(res.data)
                    console.log(res.data)
                    //decks.map(deck => {
                    //    console.log(deck.Title)
                    //})
                })
                .catch(err => {
                    if (err.response.status === 401) {
                        console.log('Not authorised')
                        logout()
                    }
                    else if (err.response.status === 404)
                        console.log('Not found');
                })
        }
    }, [])

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

    const handleLearn = id => {
        navigate(`/learn/${id}`)
    }

return (
    <>
        <button onClick={logout}>Wyloguj</button>
        {accessToken && <div>Access Token: {accessToken}</div>}
        <div className="m-3 p-5 w-85 mx-auto border">
            <p className="display-4">Decks</p>
            <div className="container text-center">
                <div className="row row-cols-2">
                    {
                        decks.map((deck) => 
                            <DeckItem key={deck.Id} title={deck.Title} description={deck.Description} id={deck.Id} handleLearn={handleLearn}  />
                        )
                    }
                </div>
            </div>
        </div>
    </>
)
}