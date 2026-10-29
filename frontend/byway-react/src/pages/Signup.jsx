import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiRequest } from '../api/request';
import { useAuth } from '../context/AuthContext';
import { getAuthenticatedLandingPath } from '../utils/authNavigation';

export default function Signup() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (loading) return;
    const form = new FormData(event.currentTarget);
    if (form.get('password') !== form.get('confirmPassword')) {
      setError('Passwords do not match.');
      return;
    }
    setLoading(true);
    setError('');
    try {
      const response = await apiRequest('/api/auth/register', {
        method: 'POST', token: null,
        body: JSON.stringify({
          firstName: form.get('firstName'),
          lastName: form.get('lastName'),
          email: form.get('email'),
          password: form.get('password'),
          confirmPassword: form.get('confirmPassword'),
        }),
      });
      login(response);
      navigate(getAuthenticatedLandingPath(response.roles));
    } catch (requestError) {
      setError(requestError.message || 'Unable to create your account.');
    } finally {
      setLoading(false);
    }
  };

  return <div className="auth-grid">
    <div className="auth-art" />
    <form className="auth-form" onSubmit={handleSubmit}>
      <h2>Create Your Account</h2>
      <div className="field row2">
        <div><label htmlFor="signup-first-name">First Name</label><input id="signup-first-name" name="firstName" maxLength={50} autoComplete="given-name" placeholder="First Name" required /></div>
        <div><label htmlFor="signup-last-name">Last Name</label><input id="signup-last-name" name="lastName" maxLength={50} autoComplete="family-name" placeholder="Last Name" required /></div>
      </div>
      <div className="field"><label htmlFor="signup-email">Email</label><input id="signup-email" name="email" type="email" autoComplete="email" placeholder="Email ID" required /></div>
      <div className="field row2">
        <div><label htmlFor="signup-password">Password</label><input id="signup-password" name="password" type="password" minLength={6} autoComplete="new-password" placeholder="Enter Password" required /></div>
        <div><label htmlFor="signup-confirm-password">Confirm Password</label><input id="signup-confirm-password" name="confirmPassword" type="password" minLength={6} autoComplete="new-password" placeholder="Confirm Password" required /></div>
      </div>
      {error && <p role="alert" style={{ color: 'var(--red)', fontSize: 14 }}>{error}</p>}
      <button type="submit" className="btn btn-dark btn-block" disabled={loading}>{loading ? 'Creating Account…' : 'Create Account →'}</button>
      <div className="switch-link">Already have an account? <Link to="/login">Log in</Link></div>
    </form>
  </div>;
}
