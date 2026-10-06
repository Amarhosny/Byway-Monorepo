import { createContext, useContext, useMemo, useState } from 'react';

const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);

  const addItem = (course) => setItems((prev) => [...prev, course]);
  const removeItem = (index) =>
    setItems((prev) => prev.filter((_, i) => i !== index));
  const clearCart = () => setItems([]);

  const totals = useMemo(() => {
    const price = items.reduce((sum, c) => sum + c.price, 0);
    const tax = +(price * 0.15).toFixed(2);
    return { price, tax, total: +(price + tax).toFixed(2) };
  }, [items]);

  const value = { items, addItem, removeItem, clearCart, totals };
  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
