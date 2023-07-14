import React, { useState, useContext } from 'react';
import { AuthContext } from '../../contexts/AuthContext';

export default function DeckCard({ title, description, id, amountToRevise, handleLearn, handleEdit }) {
    const { userId } = useContext(AuthContext)

    return (
        <div className="card">
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
                <div className="d-flex flex-row bd-highlight justify-content-center">
                    <button onClick={() => { handleLearn(id) }} type="button" className="btn btn-primary btn-block mb-4 me-2">Learn</button>
                    <button onClick={() => { handleEdit(id) }} type="button" className="btn btn-primary btn-block mb-4">Edit</button>
                </div>
                { amountToRevise === 0 &&
                    <div>No cards to revise today!</div>
                }
                { amountToRevise !== 0 &&
                    <div>{amountToRevise} cards need a revision!</div>
                }
            </div>
        </div>
    )
}