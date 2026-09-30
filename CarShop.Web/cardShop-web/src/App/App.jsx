import { useState } from 'react';
import './App.css'
import { CarsDashboard } from '../CarsDashboard';

function App() {
  const [registering, setRegistering] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');
  const [signedIn, setSignedIn] = useState(() => Boolean(sessionStorage.getItem('carshop.accessToken')));
  const [busy, setBusy] = useState(false);

  async function submit(event) {
    event.preventDefault();
    setBusy(true);
    setMessage('');

    try {
      if (registering) {
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
    } catch (error) {
      setMessage(error.message || 'Unable to connect to the server.');
    } finally {
      setBusy(false);
    }
  }

  function signOut() {
    sessionStorage.removeItem('carshop.accessToken');
    sessionStorage.removeItem('carshop.email');
    setSignedIn(false);
    setPassword('');
  }

  return (
    <main className="login-page">
      <p className={`login-status ${signedIn ? 'is-signed-in' : ''}`} role="status">
        <span aria-hidden="true" />
        {signedIn ? 'Logged in' : 'Not logged in'}
      </p>
      <section className="login-panel">
        <p className="brand">CARSHOP</p>
        {signedIn ? (
          <>
            <header className="garage-header">
              <div>
                <h1>Cars</h1>
                <p className="subtext">Signed in as {sessionStorage.getItem('carshop.email')}</p>
              </div>
              <button className="secondary-button" onClick={signOut}>Sign out</button>
            </header>
            <CarsDashboard />
          </>
        ) : (
          <>
            <h1>{registering ? 'Create account' : 'Sign in'}</h1>
            <p className="subtext">{registering ? 'Create an account to get started.' : 'Sign in to your CarShop account.'}</p>
            <form onSubmit={submit}>
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                required
              />
              <label htmlFor="password">Password</label>
              <input
                id="password"
                type="password"
                autoComplete={registering ? 'new-password' : 'current-password'}
                minLength={6}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
              />
              {message && <p className="error" role="alert">{message}</p>}
              <button className="primary-button" type="submit" disabled={busy}>
                {busy ? 'Please wait…' : registering ? 'Create account' : 'Sign in'}
              </button>
            </form>
            <p className="switch-mode">
              {registering ? 'Already have an account?' : 'New to CarShop?'}{' '}
              <button type="button" onClick={() => { setRegistering(!registering); setMessage(''); }}>
                {registering ? 'Sign in' : 'Create account'}
              </button>
            </p>
          </>
        )}
      </section>
    </main>
  );
}

export async function responseError(response) {
  try {
    const body = await response.json();
    return body.detail || body.title || Object.values(body.errors || {}).flat().join(' ') || 'Sign-in failed.';
  } catch {
    return 'Sign-in failed.';
  }
}

export default App;
