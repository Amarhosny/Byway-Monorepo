export default function Modal({ open, onClose, wide = false, children }) {
  if (!open) return null;
  return (
    <div className="overlay" onClick={onClose}>
      <div
        className={`modal ${wide ? 'wide' : ''}`}
        onClick={(e) => e.stopPropagation()}
      >
        <button className="modal-close" onClick={onClose}>✕</button>
        {children}
      </div>
    </div>
  );
}
