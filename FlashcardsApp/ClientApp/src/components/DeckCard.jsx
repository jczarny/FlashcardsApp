import React, { useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';

export default function DeckCard({ title, description, id, handleLearn, handleEdit }) {
    const { userId } = useContext(AuthContext)

    return (
        <div className="card">
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
                <button onClick={() => { handleLearn(id) }} type="button" className="btn btn-primary btn-block mb-4">Learn</button>
                <button onClick={() => { handleEdit(id) }} type="button" className="btn btn-primary btn-block mb-4">Edit</button>
            </div>
        </div>
    )
}