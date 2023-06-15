import React, { useState, useContext } from 'react';
import { useParams } from 'react-router-dom';

export default function Login() {
    const { id } = useParams()
    return (
        <div>xD? id: {id}</div>
        )
}