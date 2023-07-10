import React, { useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';

export default function DeckItem({ No, Author, Title, Description}) {

    return (
        <tr>
            <th scope="row">{No}</th>
            <td>{Author}</td>
            <td>{Title}</td>
            <td>{Description}</td>
            <td><button>Learn</button></td>
        </tr>
    )
}