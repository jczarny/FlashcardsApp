import React, { useState, useContext } from 'react';

export default function DeckItem({ Id, No, Author, Title, Description, ownedDecks, handleAcquire }) {
   
    return (
        <tr>
            <th scope="row">{No}</th>
            <td>{Author}</td>
            <td>{Title}</td>
            <td>{Description}</td>
            { ownedDecks.includes(Id) &&
                <td><button disabled>Owned</button></td>}
            {!ownedDecks.includes(Id) &&
                <td><button onClick={() => handleAcquire(Id) }>Acquire</button></td>
            }
        </tr>
    )
}