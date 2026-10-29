import assert from 'node:assert/strict';
import test from 'node:test';
import { getProtectedRouteDecision } from './adminAccess.js';

test('admin routes wait until persisted authentication has initialized', () => {
  assert.equal(getProtectedRouteDecision({ isAuthReady: false, user: null, roles: [], requiredRole: 'Admin' }), 'loading');
});

test('logged-out visitors are sent to login', () => {
  assert.equal(getProtectedRouteDecision({ isAuthReady: true, user: null, roles: [], requiredRole: 'Admin' }), 'login');
});

test('authenticated students are denied admin routes', () => {
  assert.equal(getProtectedRouteDecision({ isAuthReady: true, user: { userId: 'student' }, roles: ['Student'], requiredRole: 'Admin' }), 'denied');
});

test('admins can render admin routes', () => {
  assert.equal(getProtectedRouteDecision({ isAuthReady: true, user: { userId: 'admin' }, roles: ['Admin'], requiredRole: 'Admin' }), 'allow');
});
