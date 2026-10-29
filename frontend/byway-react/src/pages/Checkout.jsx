import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiRequest } from '../api/request';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export default function Checkout() {
  const { items, totals, refresh, loading: cartLoading, error: cartError } = useCart();
  const { token } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [paymentFailed, setPaymentFailed] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!token || loading) return;
    const form = new FormData(event.currentTarget);
    setLoading(true);
    setError('');
    setPaymentFailed(false);
    try {
      const order = await apiRequest('/api/checkout', {
        method: 'POST', token,
        body: JSON.stringify({ country: form.get('country'), state: form.get('state') }),
      });
      if (order.orderStatus === 'Paid') {
        try { sessionStorage.setItem('byway.lastOrder', JSON.stringify(order)); } catch { /* Navigation state still carries the result. */ }
        await refresh().catch(() => {});
        navigate('/purchase-complete', { state: { order } });
      } else if (order.orderStatus === 'Failed') {
        setPaymentFailed(true);
        setError('Payment was not completed. Your cart is unchanged; you can try again.');
      } else {
        setError(`Checkout returned an unexpected status: ${order.orderStatus || 'unknown'}.`);
      }
    } catch (requestError) {
      setError(requestError.message || 'Unable to complete checkout.');
    } finally {
      setLoading(false);
    }
  };

  return <>
    <div className="wrap"><h2>Checkout Page</h2><div className="crumbs"><Link to="/cart">Shopping Cart</Link> &gt; <span className="cur">Checkout</span></div></div>
    {!token ? <div className="wrap"><p role="alert">Log in to check out. <Link to="/login">Log in</Link></p></div> : cartLoading ? <div className="wrap"><p>Loading cart…</p></div> : cartError ? <div className="wrap"><p role="alert" style={{ color: 'var(--red)' }}>{cartError}</p><button type="button" className="btn btn-outline" onClick={() => refresh().catch(() => {})}>Retry loading cart</button></div> : items.length === 0 ? <div className="wrap"><p>Your cart is empty. <Link to="/courses">Browse courses</Link>.</p></div> : (
      <form className="cart-layout" onSubmit={handleSubmit}>
        <div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 16 }}>
            <div><label htmlFor="country">Country</label><input id="country" name="country" className="field-in" placeholder="Enter Country" maxLength={100} required /></div>
            <div><label htmlFor="state">State/Union Territory</label><input id="state" name="state" className="field-in" placeholder="Enter State" maxLength={100} required /></div>
          </div>
          <h4>Payment</h4>
          <div className="pay-option"><p>The backend uses a payment simulator for checkout. Card and PayPal payment are not supported or collected by this form.</p></div>
          {error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}
        </div>
        <div className="order-box"><h3>Order Details ({items.length})</h3>
          <div style={{ fontSize: 13, color: 'var(--ink-soft)', marginBottom: 12 }}>{items.map((item) => <div key={item.cartItemId} style={{ padding: '6px 0', borderBottom: '1px solid var(--line)' }}>{item.courseName}</div>)}</div>
          <div className="order-row"><span>Price</span><span>${totals.price.toFixed(2)}</span></div>
          <div className="order-row"><span>Tax</span><span>${totals.tax.toFixed(2)}</span></div>
          <div className="order-row total"><span>Total</span><span>${totals.total.toFixed(2)}</span></div>
          <button type="submit" className="btn btn-dark btn-block" style={{ marginTop: 10 }} disabled={loading}>{loading ? 'Processing…' : paymentFailed ? 'Try Again' : 'Complete Checkout'}</button>
        </div>
      </form>
    )}
  </>;
}
