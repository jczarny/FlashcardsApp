import React, { useState, useContext } from 'react';
import { AuthContext } from '../../contexts/AuthContext';

export default function CardItem({ Id, No, Front, Reverse, HandleDelete }) {

    return (
        <tr>
            <th scope="row">{No}</th>
            <td>{Front}</td>
            <td>{Reverse}</td>
            <td>
                <div className="d-flex justify-content-end">
                    <button onClick={() => { HandleDelete(Id) }} className="d-flex btn btn-sm btn-primary me-3"> Delete </button>
                </div>
            </td>
        </tr>
    )
}