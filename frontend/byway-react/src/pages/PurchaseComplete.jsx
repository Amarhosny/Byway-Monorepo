import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';

export default function PurchaseComplete() {
  const navigate = useNavigate();
  const location = useLocation();
  const [order, setOrder] = useState(location.state?.order ?? null);

  useEffect(() => {
    if (order) return;
    try {
      const stored = sessionStorage.getItem('byway.lastOrder');
      const recentOrder = stored ? JSON.parse(stored) : null;
      if (recentOrder?.orderStatus === 'Paid') setOrder(recentOrder);
    } catch {
      // No recent successful order is available in this browser session.
    }
  }, [order]);

  const backHome = () => {
    try { sessionStorage.removeItem('byway.lastOrder'); } catch { /* Storage may be unavailable. */ }
    navigate('/');
  };

  if (!order || order.orderStatus !== 'Paid') return <div className="complete-wrap">
    <h2>No completed purchase to show</h2><p style={{ color: 'var(--ink-soft)' }}>Complete checkout to see an order confirmation here.</p>
    <Link className="btn btn-dark" to="/courses">Browse courses</Link>
  </div>;

  return <div className="complete-wrap">
    <div className="checkmark">✓</div><h2>Purchase Complete</h2>
    <p style={{ color: 'var(--ink-soft)' }}>Order #{order.id} · Total ${order.grandTotal.toFixed(2)}</p>
    <p style={{ color: 'var(--ink-soft)' }}>Your payment was marked as paid by the checkout simulator.</p>
    <button className="btn btn-dark" style={{ padding: '11px 24px', marginTop: 14 }} onClick={backHome}>Back to home</button>
  </div>;
}
