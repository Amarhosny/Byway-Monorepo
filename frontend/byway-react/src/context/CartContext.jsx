import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { apiRequest } from '../api/request';
import { useAuth } from './AuthContext';

const CartContext = createContext(null);
const EMPTY_CART = { items: [], subTotal: 0, tax: 0, grandTotal: 0 };

export function CartProvider({ children }) {
  const { token } = useAuth();
  const tokenRef = useRef(token);
  const requestSequence = useRef(0);
  tokenRef.current = token;
  const [cart, setCart] = useState(EMPTY_CART);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const refresh = useCallback(async () => {
    if (!token) {
      requestSequence.current += 1;
      setCart(EMPTY_CART);
      setError('');
      setLoading(false);
      return EMPTY_CART;
    }

    const sequence = ++requestSequence.current;
    setLoading(true);
    try {
      const currentCart = await apiRequest('/api/cart', { token });
      if (sequence === requestSequence.current && tokenRef.current === token) {
        setCart(currentCart);
        setError('');
      }
      return currentCart;
    } catch (requestError) {
      if (sequence === requestSequence.current && tokenRef.current === token) setError(requestError.message || 'Unable to load your cart.');
      throw requestError;
    } finally {
      if (sequence === requestSequence.current) setLoading(false);
    }
  }, [token]);

  useEffect(() => { refresh().catch(() => {}); }, [refresh]);

  const addItem = async (courseId) => {
    if (!token) throw new Error('Log in to add courses to your cart.');
    const sequence = ++requestSequence.current;
    try {
      const updatedCart = await apiRequest(`/api/cart/items/${courseId}`, { method: 'POST', token });
      if (tokenRef.current !== token || sequence !== requestSequence.current) return updatedCart;
      setCart(updatedCart);
      setError('');
      return updatedCart;
    } finally {
      if (sequence === requestSequence.current) setLoading(false);
    }
  };

  const removeItem = async (cartItemId) => {
    const sequence = ++requestSequence.current;
    try {
      const updatedCart = await apiRequest(`/api/cart/items/${cartItemId}`, { method: 'DELETE', token });
      if (tokenRef.current !== token || sequence !== requestSequence.current) return updatedCart;
      setCart(updatedCart);
      setError('');
      return updatedCart;
    } finally {
      if (sequence === requestSequence.current) setLoading(false);
    }
  };

  const totals = useMemo(() => ({
    price: cart.subTotal,
    tax: cart.tax,
    total: cart.grandTotal,
  }), [cart]);

  return <CartContext.Provider value={{ items: cart.items, totals, loading, error, setError, refresh, addItem, removeItem }}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
