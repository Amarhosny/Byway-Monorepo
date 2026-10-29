import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { useEffect, useState } from 'react';

export default function Header() {
  const { user, logout } = useAuth();
  const { items } = useCart();
  const navigate = useNavigate();
  const location = useLocation();
  const [search, setSearch] = useState('');

  useEffect(() => {
    setSearch(new URLSearchParams(location.search).get('search') || '');
  }, [location.search]);

  const submitSearch = (event) => {
    event.preventDefault();
    const query = new URLSearchParams();
    if (search.trim()) query.set('search', search.trim());
    const queryString = query.toString();
    navigate(`/courses${queryString ? `?${queryString}` : ''}`);
  };

  return (
    <header className="site-header">
      <nav className="nav">
        <Link className="logo" to="/">
          <span className="dot" />Byway
        </Link>
        <form onSubmit={submitSearch}><input className="search" aria-label="Search courses" placeholder="Search courses" value={search} onChange={(event) => setSearch(event.target.value)} /></form>
        <Link className="navlink" to="/courses">Courses</Link>

        {user ? (
          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
            <button className="icon-btn" onClick={() => navigate('/cart')}>
              🛒
              {items.length > 0 && <span className="badge-count">{items.length}</span>}
            </button>
            <button className="icon-btn" disabled title="Notifications are not available yet" aria-label="Notifications unavailable">🔔</button>
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
