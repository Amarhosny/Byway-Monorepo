const STORAGE_KEY = 'byway.auth';

function getTokenExpiry(token) {
  if (typeof token !== 'string') return null;

  try {
    const payload = token.split('.')[1];
    if (!payload) return null;
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
    const bytes = Uint8Array.from(atob(padded), (character) => character.charCodeAt(0));
    const claims = JSON.parse(new TextDecoder().decode(bytes));
    return typeof claims.exp === 'number' ? claims.exp * 1000 : null;
  } catch {
    return null;
  }
}

function normalizeAuth(data) {
  if (!data || typeof data !== 'object'
    || typeof data.token !== 'string' || !data.token
    || typeof data.userId !== 'string' || !data.userId
    || typeof data.email !== 'string' || !data.email
    || typeof data.firstName !== 'string'
    || typeof data.lastName !== 'string'
    || !Array.isArray(data.roles) || !data.roles.every((role) => typeof role === 'string')) {
    return null;
  }

  const expiresAt = getTokenExpiry(data.token);
  if (!expiresAt || expiresAt <= Date.now()) return null;

  const displayName = data.firstName || data.email;
  return {
    userId: data.userId,
    email: data.email,
    firstName: data.firstName,
    lastName: data.lastName,
    initial: displayName[0].toUpperCase(),
    roles: data.roles,
    token: data.token,
    expiresAt,
  };
}

export function restoreAuth() {
  try {
    const stored = globalThis.localStorage?.getItem(STORAGE_KEY);
    if (!stored) return null;

    const auth = normalizeAuth(JSON.parse(stored));
    if (!auth) globalThis.localStorage.removeItem(STORAGE_KEY);
    return auth;
  } catch {
    try {
      globalThis.localStorage?.removeItem(STORAGE_KEY);
    } catch {
      // Storage can be unavailable in some browser privacy modes.
    }
    return null;
  }
}

export function persistAuth(response) {
  const auth = normalizeAuth(response);
  if (!auth) throw new Error('The API returned invalid or expired authentication data.');

  try {
    globalThis.localStorage?.setItem(STORAGE_KEY, JSON.stringify({
      userId: auth.userId,
      email: auth.email,
      firstName: auth.firstName,
      lastName: auth.lastName,
      roles: auth.roles,
      token: auth.token,
    }));
  } catch {
    // Keep the current session usable if browser storage is unavailable.
  }
  return auth;
}

export function clearPersistedAuth() {
  try {
    globalThis.localStorage?.removeItem(STORAGE_KEY);
  } catch {
    // The in-memory session is still cleared by AuthContext.
  }
}

export function getStoredAuthToken() {
  return restoreAuth()?.token ?? null;
}
