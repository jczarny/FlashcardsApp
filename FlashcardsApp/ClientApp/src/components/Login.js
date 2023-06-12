import React, { useState } from 'react';

export default function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [isPending, setIsPending] = useState(false);

    const url = "api/user/login";
    const handleSubmit = (e) => {
        e.preventDefault();
        setIsPending(true);

        const accData = { username, password };
        fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: username,
                password: password,
                })
        }).then(response => response.json())
            .then(
                data => console.log(data),
                setIsPending(false)
            );
     
    }

    return (
        <>
            <h2> Log in </h2>
            <form onSubmit = { handleSubmit }>
                <label>username: </label>
                <input
                    type="text"
                    required
                    value={username}
                    onChange={(e) => setUsername(e.target.value) }
                />
                <label>password: </label>
                <input
                    type="password"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
                { !isPending && <button>Confirm</button> }
                { isPending && <button disabled> Logging in </button> }
            </form>
        </>
    )
}