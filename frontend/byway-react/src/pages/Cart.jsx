import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiAssetUrl } from '../api/request';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import Stars from '../components/Stars';

export default function Cart() {
  const { items, removeItem, totals, loading, error, refresh } = useCart();
  const { token } = useAuth();
  const [removing, setRemoving] = useState(null);
  const [actionError, setActionError] = useState('');
  const navigate = useNavigate();

  const handleRemove = async (cartItemId) => {
    setRemoving(cartItemId);
    setActionError('');
    try { await removeItem(cartItemId); }
    catch (requestError) { setActionError(requestError.message || 'Unable to remove this course.'); }
    finally { setRemoving(null); }
  };

  return <>
    <div className="wrap"><h2>Shopping Cart</h2><div className="crumbs"><Link to="/courses">Courses</Link> &gt; <span className="cur">Shopping Cart</span></div></div>
    <div className="cart-layout">
      <div>
        {loading && <p>Loading cart…</p>}
        {!token && <p style={{ color: 'var(--ink-soft)' }}>Your cart is linked to your account. <Link to="/login">Log in</Link> to view it.</p>}
        {(error || actionError) && <div><p role="alert" style={{ color: 'var(--red)' }}>{actionError || error}</p>{error && <button type="button" className="btn btn-outline" onClick={() => refresh().catch(() => {})}>Retry loading cart</button>}</div>}
        {token && !loading && !error && items.length === 0 && <p style={{ color: 'var(--ink-soft)' }}>Your cart is empty. <Link to="/courses">Browse courses</Link>.</p>}
        {token && items.map((item) => <div className="cart-item" key={item.cartItemId}>
          <div className="cart-thumb" style={item.imagePath ? { backgroundImage: `url("${apiAssetUrl(item.imagePath)}")`, backgroundSize: 'cover' } : undefined} />
          <div style={{ flex: 1 }}><h4 style={{ margin: '0 0 4px' }}>{item.courseName}</h4><div className="by">By {item.instructorName}</div>
            <div className="stars"><Stars rating={item.rating ?? 0} /><span style={{ color: 'var(--ink-soft)', fontSize: 12 }}>{item.totalHours} Total Hours</span></div>
            <button className="remove" disabled={removing === item.cartItemId} onClick={() => handleRemove(item.cartItemId)}>{removing === item.cartItemId ? 'Removing…' : 'Remove'}</button>
          </div><div className="price">${item.price.toFixed(2)}</div>
        </div>)}
      </div>
      <div className="order-box"><h3>Order Details</h3>
        <div className="order-row"><span>Price</span><span>${totals.price.toFixed(2)}</span></div>
        <div className="order-row"><span>Tax</span><span>${totals.tax.toFixed(2)}</span></div>
        <div className="order-row total"><span>Total</span><span>${totals.total.toFixed(2)}</span></div>
        <button className="btn btn-dark btn-block" style={{ marginTop: 10 }} disabled={!token || loading || items.length === 0} onClick={() => navigate('/checkout')}>Proceed to Checkout</button>
      </div>
    </div>
  </>;
}
