import { useState } from 'react';
import { Link, Navigate, useNavigate } from 'react-router-dom';
import { apiRequest } from '../api/request';
import { useAuth } from '../context/AuthContext';
import { getAuthenticatedLandingPath } from '../utils/authNavigation';

export default function Login() {
  const { login, user } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (loading) return;

    const form = new FormData(event.currentTarget);
    setLoading(true);
    setError('');

    try {
      const response = await apiRequest('/api/auth/login', {
        method: 'POST',
        token: null,
        body: JSON.stringify({
          email: form.get('email'),
          password: form.get('password'),
        }),
      });
      login(response);
      navigate(getAuthenticatedLandingPath(response.roles));
    } catch (requestError) {
      setError(requestError.status === 401
        ? 'Invalid email or password.'
        : requestError.message || 'Unable to sign in. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (user) return <Navigate to={getAuthenticatedLandingPath(user.roles)} replace />;

  return (
    <div className="auth-grid">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Sign in to your account</h2>
        <div className="field">
          <label htmlFor="login-email">Email</label>
          <input id="login-email" name="email" type="email" autoComplete="username" placeholder="Email ID" required />
        </div>
        <div className="field">
          <label htmlFor="login-password">Password</label>
          <input id="login-password" name="password" type="password" autoComplete="current-password" placeholder="Enter Password" required />
        </div>
        {error && <p role="alert" style={{ color: 'var(--red)', fontSize: 14 }}>{error}</p>}
        <button type="submit" className="btn btn-dark btn-block" disabled={loading}>
          {loading ? 'Signing In…' : 'Sign In →'}
        </button>
        <div className="divider">Sign in with</div>
        <div className="oauth-row">
          <div className="oauth-btn">Facebook</div>
          <div className="oauth-btn">Google</div>
          <div className="oauth-btn">Microsoft</div>
        </div>
        <div className="switch-link">
          Don't have an account? <Link to="/signup">Sign up</Link>
        </div>
      </form>
      <div className="auth-art" />
    </div>
  );
}
