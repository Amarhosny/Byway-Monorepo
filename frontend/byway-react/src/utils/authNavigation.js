export function getAuthenticatedLandingPath(roles) {
  return Array.isArray(roles) && roles.includes('Admin')
    ? '/admin/dashboard'
    : '/courses';
}
