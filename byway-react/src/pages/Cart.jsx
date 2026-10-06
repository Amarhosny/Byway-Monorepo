import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';

export default function Cart() {
  const { items, removeItem, totals } = useCart();
  const navigate = useNavigate();

  return (
    <>
      <div className="wrap">
        <h2>Shopping Cart</h2>
        <div className="crumbs">
          <Link to="/courses">Courses</Link> &gt; Details &gt; <span className="cur">Shopping Cart</span>
        </div>
      </div>
      <div className="cart-layout">
        <div>
          {items.length === 0 ? (
            <p style={{ color: 'var(--ink-soft)' }}>Your cart is empty.</p>
          ) : (
            items.map((item, i) => (
              <div className="cart-item" key={i}>
                <div className="cart-thumb" />
                <div style={{ flex: 1 }}>
                  <h4 style={{ margin: '0 0 4px' }}>{item.title}</h4>
                  <div className="by">By {item.instructor}</div>
                  <div className="stars">
                    4.6 ★★★★★{' '}
                    <span style={{ color: 'var(--ink-soft)', fontSize: 12 }}>
                      22 Total Hours · 155 Lectures · All levels
                    </span>
                  </div>
                  <button className="remove" onClick={() => removeItem(i)}>Remove</button>
                </div>
                <div className="price">${item.price.toFixed(2)}</div>
              </div>
            ))
          )}
        </div>
        <div className="order-box">
          <h3>Order Details</h3>
          <div className="order-row"><span>Price</span><span>${totals.price.toFixed(2)}</span></div>
          <div className="order-row"><span>Discount</span><span>$0.00</span></div>
          <div className="order-row"><span>Tax</span><span>${totals.tax.toFixed(2)}</span></div>
          <div className="order-row total"><span>Total</span><span>${totals.total.toFixed(2)}</span></div>
          <button
            className="btn btn-dark btn-block"
            style={{ marginTop: 10 }}
            disabled={items.length === 0}
            onClick={() => navigate('/checkout')}
          >
            Proceed to Checkout
          </button>
        </div>
      </div>
    </>
  );
}
