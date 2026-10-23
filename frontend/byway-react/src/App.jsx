import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { CartProvider } from './context/CartContext';
import { AuthProvider } from './context/AuthContext';

import PublicLayout from './components/PublicLayout';
import AdminLayout from './components/AdminLayout';

import Landing from './pages/Landing';
import Login from './pages/Login';
import Signup from './pages/Signup';
import Courses from './pages/Courses';
import CourseDetails from './pages/CourseDetails';
import Cart from './pages/Cart';
import Checkout from './pages/Checkout';
import PurchaseComplete from './pages/PurchaseComplete';

import Dashboard from './pages/admin/Dashboard';
import Instructors from './pages/admin/Instructors';
import AdminCourses from './pages/admin/AdminCourses';

export default function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <BrowserRouter>
          <Routes>
            {/* Public / student-facing site */}
            <Route element={<PublicLayout />}>
              <Route path="/" element={<Landing />} />
              <Route path="/login" element={<Login />} />
              <Route path="/signup" element={<Signup />} />
              <Route path="/courses" element={<Courses />} />
              <Route path="/courses/:id" element={<CourseDetails />} />
              <Route path="/cart" element={<Cart />} />
              <Route path="/checkout" element={<Checkout />} />
              <Route path="/purchase-complete" element={<PurchaseComplete />} />
            </Route>

            {/* Admin dashboard */}
            <Route path="/admin" element={<AdminLayout />}>
              <Route index element={<Dashboard />} />
              <Route path="instructors" element={<Instructors />} />
              <Route path="courses" element={<AdminCourses />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </CartProvider>
    </AuthProvider>
  );
}
