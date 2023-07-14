import React, { useState, useContext, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { AuthContext } from '../../contexts/AuthContext';
import axios from 'axios';

export default function CreateDeck() {
    const { userId, getAuthentication, logout, config } = useContext(AuthContext)
    const navigate = useNavigate();

    const [title, setTitle] = useState('');
    const [validTitle, setValidTitle] = useState(false);
    const [titleFocus, setTitleFocus] = useState(false);

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
        if (title.length > 32 || title.length < 6)
            setValidTitle(false)
        else
            setValidTitle(true)
        setOutputMsg('');
    }, [title])

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!validTitle)
            setOutputMsg("Invalid title!");
        else if (getAuthentication() === 0)
            logout();
        else {
            setIsPending(true);
            axios.post('api/deck/create', {
                userId: userId,
                Title: title,
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
            <div className="cardContainer">
                <div className="card">
                    <h5 className="card-header">
                        Create Deck
                    </h5>

                    <div className="card-body text-center">
                        <h2 className="card-title">
                            <div>Title</div>
                        </h2>
                        <input
                            className="form-outline w-50 mb-2"
                            type="text"
                            required
                            placeholder="Enter title"
                            size="5"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            onFocus={() => setTitleFocus(true)}
                            onBlur={() => setTitleFocus(false)}
                        />
                        <br/>
                        {titleFocus && !validTitle &&
                            <>
                                <div>Title has to be between 6 to 32 characters long.</div>
                                <br />
                            </>
                        }

                        <label> Description: </label>
                        <br />
                        <textarea
                            className="form-outline w-50 mb-2 p-1"
                            type="text"
                            placeholder="Enter description"
                            value={description}
                            size="30"
                            cols="60"
                            rows="6"
                            onChange={(e) => setDescription(e.target.value)}
                        />
                        {outputMsg && <div>{outputMsg}</div>}
                    </div>
                    <div className="card-footer">
                        <button onClick={handleSubmit} className="btn btn-primary">Submit</button>
                    </div>
                </div>
            </div>
        </>
    )
}