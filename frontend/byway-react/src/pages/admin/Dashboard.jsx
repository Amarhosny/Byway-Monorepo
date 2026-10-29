import { useCallback, useEffect, useState } from 'react';
import { apiRequest } from '../../api/request';
import { useAuth } from '../../context/AuthContext';

function sanitizeDiagnosticMessage(message) {
  return String(message || 'Unknown dashboard request error.')
    .replace(/Bearer\s+\S+/gi, 'Bearer [redacted]')
    .replace(/\beyJ[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*\b/g, '[redacted JWT]')
    .replace(/\b(password|secret|signingkey|connectionstring|token)\b\s*[:=]\s*[^\s,;]+/gi, '$1=[redacted]')
    .slice(0, 240);
}

export default function Dashboard() {
  const { token } = useAuth();
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadDashboard = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await apiRequest('/api/admin/dashboard', { token });
      const fields = ['instructorCount', 'categoryCount', 'courseCount', 'enrollmentsThisMonth'];
      if (!data || fields.some((field) => !Number.isFinite(data[field]))) {
        throw new Error('The dashboard API returned an unexpected response.');
      }
      setDashboard(data);
    } catch (requestError) {
      if (import.meta.env.DEV) {
        console.warn('[Byway dashboard diagnostic]', {
          tokenPresent: Boolean(token),
          status: Number.isInteger(requestError.status) ? requestError.status : null,
          message: sanitizeDiagnosticMessage(requestError.message),
        });
      }
      setDashboard(null);
      setError(requestError.message || 'Unable to load the dashboard.');
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  return <>
    <div className="topbar"><h1>Dashboard</h1><div className="bell" /></div><hr className="rule" />
    {error && <div><p role="alert" style={{ color: 'var(--red)' }}>{error}</p><button type="button" className="btn btn-dark" onClick={loadDashboard} disabled={loading}>{loading ? 'Retrying…' : 'Retry'}</button></div>}
    {loading ? <p>Loading dashboard…</p> : dashboard && <>
      <div className="stat-row">
        {[
          ['Instructors', dashboard.instructorCount],
          ['Categories', dashboard.categoryCount],
          ['Courses', dashboard.courseCount],
          ['Enrollments this month', dashboard.enrollmentsThisMonth],
        ].map(([label, value]) => <div className="stat-card" key={label}><div><div className="num">{value}</div><div className="lbl">{label}</div></div></div>)}
      </div>
      <div className="panel"><h3>Monthly enrollments</h3><p>{dashboard.enrollmentsThisMonth} course enrollments on paid orders this calendar month.</p><p style={{ color: 'var(--ink-soft)', fontSize: 13 }}>Revenue and wallet balances are not provided by the current API.</p></div>
    </>}
  </>;
}
