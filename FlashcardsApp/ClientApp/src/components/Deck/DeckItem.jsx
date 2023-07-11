import React, { useState, useContext } from 'react';

export default function DeckItem({ Id, No, Author, Title, Description, ownedDecks, handleAcquire }) {
   
    return (
        <tr classname="d-flex align-self-stretch">
            <th scope="row">{No}</th>
            <td>{Author}</td>
            <td>{Title}</td>
            <td>{Description}</td>
            { ownedDecks.includes(Id) &&
                <td><button disabled className="btn btn-primary btn-sm mb-4">Owned</button></td>}
            {!ownedDecks.includes(Id) &&
                <td><button onClick={() => handleAcquire(Id)} className="btn btn-primary btn-sm">Acquire</button></td>
            }
        </tr>
    )
}