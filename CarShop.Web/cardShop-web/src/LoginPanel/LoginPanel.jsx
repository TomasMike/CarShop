import { useState } from 'react';

export function LoginPanel()
{
    const [registering, setRegistering] = useState(false);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');
    const [signedIn, setSignedIn] = useState(() => Boolean(sessionStorage.getItem('carshop.accessToken')));
    const [busy, setBusy] = useState(false);

    async function loginRegisterSubmit(event)
    {
        event.preventDefault();
        setBusy(true);
        setMessage('');

        try
        {
            if (registering)
            {
                const response = await fetch('/api/register', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password }),
                });
                if (!response.ok) throw new Error(await responseError(response));
            }

            const response = await fetch('/api/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });
            if (!response.ok) throw new Error(await responseError(response));

            const result = await response.json();
            sessionStorage.setItem('carshop.accessToken', result.accessToken);
            sessionStorage.setItem('carshop.email', email);
            setSignedIn(true);
        }
        catch (error)
        {
            setMessage(error.message || 'Unable to connect to the server.');
        }
        finally
        {
            setBusy(false);
        }
    }

    function signOut()
    {
        sessionStorage.removeItem('carshop.accessToken');
        sessionStorage.removeItem('carshop.email');
        setSignedIn(false);
        setPassword('');
    }

    return (
        <>
            <div className="login-page">
                {signedIn ? (
                    <div>
                        <div>Signed in as {sessionStorage.getItem('carshop.email')}<button onClick={signOut}>Sign out</button></div>
                    </div>
                ) : (
                    <>
                        <span>Not logged in</span>
                        <form onSubmit={loginRegisterSubmit}>
                            <label htmlFor="email">Email</label>
                            <input
                                id="email"
                                type="email"
                                value={email}
                                onChange={(event) => setEmail(event.target.value)}
                                required
                            />

                            <label htmlFor="password">Password</label>
                            <input
                                id="password"
                                type="password"
                                value={password}
                                onChange={(event) => setPassword(event.target.value)}
                                required
                            />

                            {message && <p className="error" role="alert">{message}</p>}

                            <button className="primary-button" type="submit" disabled={busy}>
                                {busy ? 'Please wait…' : registering ? 'Create account' : 'Sign in'}
                            </button>

                            <button
                                type="button"
                                className="secondary-button"
                                onClick={() => setRegistering((current) => !current)}
                            >
                                {registering ? 'Switch to sign in' : 'Create account'}
                            </button>
                        </form>
                    </>
                )}
            </div>
            <p></p>
        </>
    );
}

export async function responseError(response)
{
    try
    {
        const body = await response.json();
        return body.detail || body.title || Object.values(body.errors || {}).flat().join(' ') || 'Sign-in failed.';
    }
    catch
    {
        return 'Sign-in failed.';
    }
}

export default LoginPanel;
