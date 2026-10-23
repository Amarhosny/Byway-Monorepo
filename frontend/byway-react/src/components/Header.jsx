import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Header() {
  const { user, logout } = useAuth();
  const { items } = useCart();
  const navigate = useNavigate();

  return (
    <header className="site-header">
      <nav className="nav">
        <Link className="logo" to="/">
          <span className="dot" />Byway
        </Link>
        <input className="search" placeholder="Search courses" />
        <Link className="navlink" to="/courses">Courses</Link>

        {user ? (
          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
            <button className="icon-btn" onClick={() => navigate('/cart')}>
              🛒
              {items.length > 0 && <span className="badge-count">{items.length}</span>}
            </button>
            <button className="icon-btn">🔔</button>
            <button className="avatar-chip" onClick={logout} title="Log out">
              {user.initial}
            </button>
          </div>
        ) : (
          <div style={{ display: 'flex', gap: 10 }}>
            <button className="btn" onClick={() => navigate('/login')}>Log In</button>
            <button className="btn btn-dark" onClick={() => navigate('/signup')}>Sign Up</button>
          </div>
        )}
      </nav>
    </header>
  );
}
