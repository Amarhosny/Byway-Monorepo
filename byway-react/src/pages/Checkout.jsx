import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';

export default function Checkout() {
  const { items, totals, clearCart } = useCart();
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();
    clearCart();
    navigate('/purchase-complete');
  };

  return (
    <>
      <div className="wrap">
        <h2>Checkout Page</h2>
        <div className="crumbs">
          Details &gt; <Link to="/cart">Shopping Cart</Link> &gt; <span className="cur">Checkout</span>
        </div>
      </div>
      <form className="cart-layout" onSubmit={handleSubmit}>
        <div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 16 }}>
            <div><label>Country</label><input className="field-in" placeholder="Enter Country" /></div>
            <div><label>State/Union Territory</label><input className="field-in" placeholder="Enter State" /></div>
          </div>
          <h4>Payment Method</h4>
          <div className="pay-option">
            <div className="head"><span>⚪ Credit/Debit Card</span><span>VISA</span></div>
            <label>Name of Card</label>
            <input className="field-in" style={{ marginBottom: 10 }} placeholder="Name of card" />
            <label>Card Number</label>
            <input className="field-in" style={{ marginBottom: 10 }} placeholder="Card Number" />
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10 }}>
              <input className="field-in" placeholder="Expiry Date" />
              <input className="field-in" placeholder="CVC/CVV" />
            </div>
          </div>
          <div className="pay-option"><div className="head"><span>⚪ PayPal</span></div></div>
        </div>
        <div className="order-box">
          <h3>Order Details ({items.length})</h3>
          <div style={{ fontSize: 13, color: 'var(--ink-soft)', marginBottom: 12 }}>
            {items.map((item, i) => (
              <div key={i} style={{ padding: '6px 0', borderBottom: '1px solid var(--line)' }}>
                {item.title}
              </div>
            ))}
          </div>
          <div className="order-row"><span>Price</span><span>${totals.price.toFixed(2)}</span></div>
          <div className="order-row"><span>Discount</span><span>$0.00</span></div>
          <div className="order-row"><span>Tax</span><span>${totals.tax.toFixed(2)}</span></div>
          <div className="order-row total"><span>Total</span><span>${totals.total.toFixed(2)}</span></div>
          <button type="submit" className="btn btn-dark btn-block" style={{ marginTop: 10 }} disabled={items.length === 0}>
            Proceed to Checkout
          </button>
        </div>
      </form>
    </>
  );
}
