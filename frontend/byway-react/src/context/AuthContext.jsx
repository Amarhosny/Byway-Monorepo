import { createContext, useContext, useEffect, useState } from 'react';
import { clearPersistedAuth, persistAuth, restoreAuth } from './authStorage';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(restoreAuth);
  const [isAuthReady, setIsAuthReady] = useState(false);

  const login = (response) => setAuth(persistAuth(response));
  const logout = () => {
    clearPersistedAuth();
    setAuth(null);
  };

  useEffect(() => {
    const handleUnauthorized = () => logout();
    window.addEventListener('byway:unauthorized', handleUnauthorized);
    return () => window.removeEventListener('byway:unauthorized', handleUnauthorized);
  }, []);

  useEffect(() => {
    setIsAuthReady(true);
  }, []);

  useEffect(() => {
    if (!auth?.expiresAt) return undefined;

    const remaining = auth.expiresAt - Date.now();
    if (remaining <= 0) {
      logout();
      return undefined;
    }

    const timeout = window.setTimeout(logout, remaining);
    return () => window.clearTimeout(timeout);
  }, [auth]);

  return (
    <AuthContext.Provider value={{ user: auth, token: auth?.token ?? null, roles: auth?.roles ?? [], isAuthReady, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
