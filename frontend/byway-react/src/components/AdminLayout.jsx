import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const navItems = [
  { to: '/admin', label: 'Dashboard', icon: '🏠', end: true },
  { to: '/admin/instructors', label: 'Instructors', icon: '👤' },
  { to: '/admin/courses', label: 'Courses', icon: '📁' },
];

export default function AdminLayout() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="shell">
      <div className="sidebar">
        <div className="logo"><span className="dot" />Byway</div>
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.end}
            className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
          >
            {item.icon} {item.label}
          </NavLink>
        ))}
        <div className="nav-sep" />
        <div className="nav-item" onClick={handleLogout}>↪ logout</div>
      </div>
      <div className="main">
        <Outlet />
      </div>
    </div>
  );
}
