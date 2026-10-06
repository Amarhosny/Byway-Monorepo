import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';

const TABS = ['Description', 'Instructor', 'Content', 'Reviews'];

const CONTENT = [
  { name: 'Introduction to UX Design', lectures: 5, hours: 1 },
  { name: 'Basics of User-Centered Design', lectures: 5, hours: 1 },
  { name: 'Elements of User Experience', lectures: 5, hours: 1 },
  { name: 'Visual Design Principles', lectures: 5, hours: 1 },
];

export default function CourseDetails() {
  const { id } = useParams();
  const [tab, setTab] = useState('Description');
  const { addItem } = useCart();
  const { user, login } = useAuth();
  const navigate = useNavigate();

  const course = {
    id,
    title: 'Introduction to User Experience Design',
    instructor: 'John Doe',
    price: 45,
  };

  const handleAddToCart = () => {
    addItem(course);
    if (!user) login();
    navigate('/cart');
  };

  const handleBuyNow = () => {
    addItem(course);
    if (!user) login();
    navigate('/cart');
  };

  return (
    <>
      <div className="wrap">
        <div className="crumbs">
          <Link to="/">Home</Link> &gt; <Link to="/courses">Courses</Link> &gt;{' '}
          <span className="cur">{course.title}</span>
        </div>
      </div>
      <div className="details-layout">
        <div>
          <h1 style={{ fontSize: 26, margin: '0 0 10px' }}>{course.title}</h1>
          <p style={{ color: 'var(--ink-soft)' }}>
            This course is meticulously crafted to provide you with a
            foundational understanding of the principles, methods, and tools
            that drive exceptional user experiences in the digital landscape.
          </p>
          <div className="stars">
            4.0 ★★★★★{' '}
            <span style={{ color: 'var(--ink-soft)', fontSize: 13 }}>
              | 22 Total Hours · 155 Lectures · All levels
            </span>
          </div>
          <p style={{ fontSize: 13.5 }}>
            Created by <span style={{ color: 'var(--blue)' }}>Ronald Richards</span>
          </p>

          <div className="tabs">
            {TABS.map((t) => (
              <button
                key={t}
                className={`tab ${tab === t ? 'active' : ''}`}
                onClick={() => setTab(t)}
              >
                {t}
              </button>
            ))}
          </div>

          {tab === 'Description' && (
            <>
              <h3>Course Description</h3>
              <p style={{ color: 'var(--ink-soft)', fontSize: 14.5 }}>
                This interactive e-learning course will introduce you to User
                Experience (UX) design, the art of creating products and
                services that are intuitive, enjoyable, and user-friendly.
              </p>
            </>
          )}

          {tab === 'Instructor' && (
            <>
              <h3>Instructor</h3>
              <p><strong>Ronald Richards</strong> — UI/UX Designer</p>
              <p style={{ color: 'var(--ink-soft)', fontSize: 14.5 }}>
                40,445 Reviews · 500 Students · 15 Courses. With over a decade
                of industry experience, Ronald brings a wealth of practical
                knowledge to the classroom.
              </p>
            </>
          )}

          {tab === 'Content' && (
            <>
              <h3>Content</h3>
              {CONTENT.map((c) => (
                <div className="content-row" key={c.name}>
                  <span>{c.name}</span>
                  <span style={{ color: 'var(--ink-soft)' }}>{c.lectures} Lectures · {c.hours} hour</span>
                </div>
              ))}
            </>
          )}

          {tab === 'Reviews' && (
            <>
              <h3>Learner Reviews</h3>
              <div className="review">
                <strong>Mark Doe</strong> <span className="stars">★5</span>
                <p style={{ color: 'var(--ink-soft)', fontSize: 13.5, margin: '6px 0 0' }}>
                  I was initially apprehensive, having no prior design
                  experience. But the instructor did an amazing job of
                  breaking down complex concepts into easily digestible
                  modules.
                </p>
              </div>
            </>
          )}
        </div>

        <div className="buy-card">
          <div className="detail-hero" />
          <div className="price">${course.price.toFixed(2)}</div>
          <button className="btn btn-dark btn-block" style={{ marginBottom: 8 }} onClick={handleAddToCart}>
            Add To Cart
          </button>
          <button className="btn btn-block" onClick={handleBuyNow}>Buy Now</button>
        </div>
      </div>
    </>
  );
}
