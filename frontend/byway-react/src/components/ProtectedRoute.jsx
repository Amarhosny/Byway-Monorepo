import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getProtectedRouteDecision } from '../utils/adminAccess';

export default function ProtectedRoute({ requiredRole }) {
  const { user, roles, isAuthReady } = useAuth();
  const decision = getProtectedRouteDecision({ isAuthReady, user, roles, requiredRole });

  if (decision === 'loading') return <p role="status">Checking access…</p>;
  if (decision === 'login') return <Navigate to="/login" replace />;
  if (decision === 'denied') return <Navigate to="/courses" replace />;

  return <Outlet />;
}
