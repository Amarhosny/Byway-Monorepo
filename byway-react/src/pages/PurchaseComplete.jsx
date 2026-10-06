import { useNavigate } from 'react-router-dom';

export default function PurchaseComplete() {
  const navigate = useNavigate();
  return (
    <div className="complete-wrap">
      <div className="checkmark">✓</div>
      <h2>Purchase Complete</h2>
      <p style={{ color: 'var(--ink-soft)' }}>You Will Receive a confirmation email soon!</p>
      <button className="btn btn-dark" style={{ padding: '11px 24px', marginTop: 14 }} onClick={() => navigate('/')}>
        Back to home
      </button>
    </div>
  );
}
