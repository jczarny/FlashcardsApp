import React, { useState, useContext } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import { useNavigate } from "react-router-dom";

export default function DeckCard({ title, description, id, amountToRevise, handleLearn, handleEdit }) {
    const navigate = useNavigate()

    const navigateToBrowse = e => {
        navigate('/browse')
    }
    return (
        <>
            {id >= 0 && 
                <div className="card">
                    <div className="card-body">
                        <h5 className="card-title">{title}</h5>
                        <p className="card-text">{description}</p>
                        <div className="d-flex flex-row bd-highlight justify-content-center">
                            <button onClick={() => { handleLearn(id) }} type="button" className="btn btn-primary btn-block mb-4 me-2">Learn</button>
                            <button onClick={() => { handleEdit(id) }} type="button" className="btn btn-primary btn-block mb-4">Edit</button>
                        </div>
                        {amountToRevise === 0 &&
                            <div className="alert alert-success">No cards to revise today!</div>
                        }
                        {amountToRevise !== 0 &&
                            <div className="alert alert-danger">{amountToRevise} cards need a revision!</div>
                        }
                    </div>
                </div>
            }
            {
                id === -1 &&
                <div className="card">
                        <div className="card-body d-flex flex-column justify-content-center">
                        <h5 className="card-title">Browse</h5>
                        <p className="card-text">Get more decks from other users!</p>
                        <div className="d-flex flex-row bd-highlight justify-content-center">
                            <button onClick={() => { navigateToBrowse() }} type="button" className="btn btn-primary btn-block">Browse</button>
                        </div>
                    </div>
                </div>
            }

        </>

    )
}