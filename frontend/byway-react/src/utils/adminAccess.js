export function getProtectedRouteDecision({ isAuthReady, user, roles, requiredRole }) {
  if (!isAuthReady) return 'loading';
  if (!user) return 'login';
  if (requiredRole && (!Array.isArray(roles) || !roles.includes(requiredRole))) return 'denied';
  return 'allow';
}
