import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import CardItem from './CardItem';

export default function CardList({ title, cards, HandleDelete, deleteMsg }) {

    return (
        <>
            <h3> {title} </h3>
            {deleteMsg
                && <div>{deleteMsg}</div>}
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Front</th>
                        <th scope="col">Reverse</th>
                        <th scope="col"> </th>
                    </tr>
                </thead>
                <tbody>
                    { 
                        cards.map((card) =>
                            <CardItem key={card.No} Id={card.Id} No={card.No} Front={card.Front}
                                Reverse={card.Reverse} HandleDelete={HandleDelete} />
                        )
                    }
                </tbody>
            </table>
        </>
    )
}