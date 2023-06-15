import React, { useState, useContext } from 'react';

export default function DeckItem({ title, description, id, handleLearn }) {

    return (
        <div className="card">
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
                <button onClick = {() => { handleLearn(id) }} type="button" className="btn btn-primary btn-block mb-4">Learn</button>
            </div>
        </div>
    )
}