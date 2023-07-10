import React, { useState, useContext } from 'react';
import { AuthContext } from '../../contexts/AuthContext';

export default function CardItem({ Id, No, Front, Reverse, HandleDelete  }) {

    return (
        <tr>
            <th scope="row">{No}</th>
            <td>{Front}</td>
            <td>{Reverse}</td>
            <td><button onClick={() => { HandleDelete(Id) }}>Delete</button></td>
        </tr>
    )
}