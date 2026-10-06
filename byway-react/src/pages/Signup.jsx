import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Signup() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();
    login();
    navigate('/courses');
  };

  return (
    <div className="auth-grid">
      <div className="auth-art" />
      <form className="auth-form" onSubmit={handleSubmit}>
        <h2>Create Your Account</h2>
        <div className="field row2">
          <div><label>Full Name</label><input placeholder="First Name" /></div>
          <div><label>&nbsp;</label><input placeholder="Last Name" /></div>
        </div>
        <div className="field"><label>Username</label><input placeholder="Username" /></div>
        <div className="field"><label>Email</label><input placeholder="Email ID" /></div>
        <div className="field row2">
          <div><label>Password</label><input type="password" placeholder="Enter Password" /></div>
          <div><label>Confirm Password</label><input type="password" placeholder="Confirm Password" /></div>
        </div>
        <button type="submit" className="btn btn-dark btn-block">Create Account →</button>
        <div className="divider">Sign up with</div>
        <div className="oauth-row">
          <div className="oauth-btn">Facebook</div>
          <div className="oauth-btn">Google</div>
          <div className="oauth-btn">Microsoft</div>
        </div>
        <div className="switch-link">
          Already have an account? <Link to="/login">Log in</Link>
        </div>
      </form>
    </div>
  );
}
