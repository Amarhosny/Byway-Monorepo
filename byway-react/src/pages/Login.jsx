import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();
    login();
    navigate('/courses');
  };

  return (
    <div className="auth-grid">
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Sign in to your account</h2>
        <div className="field">
          <label>Email</label>
          <input placeholder="Username or Email ID" />
        </div>
        <div className="field">
          <label>Password</label>
          <input type="password" placeholder="Enter Password" />
        </div>
        <button type="submit" className="btn btn-dark btn-block">Sign In →</button>
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
