import { getStoredAuthToken } from '../context/authStorage';

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:61550').replace(/\/$/, '');

export async function apiRequest(path, { token, headers, ...options } = {}) {
  const requestHeaders = new Headers(headers);
  if (options.body && !(options.body instanceof FormData) && !requestHeaders.has('Content-Type')) {
    requestHeaders.set('Content-Type', 'application/json');
  }
  const authToken = token === undefined ? getStoredAuthToken() : token;
  if (authToken) requestHeaders.set('Authorization', `Bearer ${authToken}`);

  let response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, { ...options, headers: requestHeaders });
  } catch {
    throw new Error(`Could not reach Byway API at ${API_BASE_URL}. Check that the backend is running and VITE_API_BASE_URL is correct.`);
  }

  const contentType = response.headers.get('content-type') ?? '';
  const payload = contentType.includes('json') ? await response.json() : null;
  if (response.status === 401 && authToken && typeof window !== 'undefined') {
    window.dispatchEvent(new CustomEvent('byway:unauthorized'));
  }
  if (!response.ok) {
    const validationErrors = payload?.errors ? Object.values(payload.errors).flat().join(' ') : '';
    const fallback = response.status === 401
      ? 'Your session is not authorized. Please log in again.'
      : response.status === 403
        ? 'This action requires permission from the backend. Check that your account has the required role or access.'
        : `Request failed (${response.status}).`;
    const error = new Error(payload?.error || payload?.detail || validationErrors || payload?.title || fallback);
    error.status = response.status;
    throw error;
  }
  return payload;
}

export function apiAssetUrl(path) {
  if (!path) return '';
  if (/^https?:\/\//i.test(path)) return path;
  return `${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`;
}
