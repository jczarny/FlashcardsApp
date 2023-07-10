import React, { useEffect, useContext } from 'react';
import { useNavigate, Link } from "react-router-dom";
import axios from 'axios';
import { AuthContext } from '../../contexts/AuthContext';

export default function Login() {
    const { logout } = useContext(AuthContext)

    useEffect(() => {
        logout()
    }, [])

    return (
        <>
        </>
    )
}