import { Link } from 'react-router-dom';
import { courses, categories, testimonials } from '../data/courses';
import Stars from '../components/Stars';
import { instructors } from '../data/instructors';

export default function Landing() {
  return (
    <>
      <section className="hero">
        <div className="hero-grid">
          <div>
            <h1>Unlock Your Potential with Byway</h1>
            <p>
              Welcome to Byway, where learning knows no bounds. We believe that
              education is the key to personal and professional growth, and
              we're here to guide you on your journey to success.
            </p>
            <Link to="/courses">
              <button className="btn btn-dark" style={{ background: 'var(--blue)', borderColor: 'var(--blue)' }}>
                Start your journey
              </button>
            </Link>
          </div>
          <div className="hero-art">
            <div className="bubble b1">Instructor</div>
            <div className="bubble b2">Mentor</div>
            <div className="bubble b3">Join our community of 1200+ Students</div>
          </div>
        </div>
      </section>

      <section className="stats">
        <div className="stats-grid">
          {['250+', '1000+', '15+', '2400+'].map((n) => (
            <div key={n}>
              <div className="stat-num">{n}</div>
              <div className="stat-label">Courses by our best mentors</div>
            </div>
          ))}
        </div>
      </section>

      <section className="section">
        <div className="section-head">
          <h2>Top Categories</h2>
          <div className="arrows"><div className="arrow">‹</div><div className="arrow">›</div></div>
        </div>
        <div className="cat-grid">
          {categories.map((c) => (
            <div className="cat-card" key={c.name}>
              <div className="cat-icon">{c.icon}</div>
              <h3>{c.name}</h3>
              <span>{c.count} Courses</span>
            </div>
          ))}
        </div>
      </section>

      <section className="section">
        <div className="section-head">
          <h2>Top Courses</h2>
          <Link className="seeall" to="/courses">See All</Link>
        </div>
        <div className="course-grid">
          {courses.slice(0, 4).map((c) => (
            <Link className="course-card" to={`/courses/${c.id}`} key={c.id}>
              <div className="course-thumb"><span className="badge">{c.category}</span></div>
              <div className="course-body">
                <h4>{c.title}</h4>
                <div className="by">By {c.instructor}</div>
                <Stars rating={c.rating} />
                <div className="meta">{c.hours} Total Hours · {c.lectures} Lectures · {c.level}</div>
                <div className="price">${c.price.toFixed(2)}</div>
              </div>
            </Link>
          ))}
        </div>
      </section>

      <section className="section">
        <div className="section-head">
          <h2>Top Instructors</h2>
          <div className="arrows"><div className="arrow">‹</div><div className="arrow">›</div></div>
        </div>
        <div className="instr-grid">
          {instructors.slice(0, 5).map((i) => (
            <div className="instr-card" key={i.id}>
              <div className="avatar" />
              <h4>{i.name}</h4>
              <div style={{ fontSize: 12, color: 'var(--ink-soft)', marginBottom: 6 }}>UI/UX Designer</div>
              <div className="stars" style={{ fontSize: 12 }}>★ 4.9 · 2400 Students</div>
            </div>
          ))}
        </div>
      </section>

      <section className="section">
        <div className="section-head">
          <h2>What Our Customers Say About Us</h2>
          <div className="arrows"><div className="arrow">‹</div><div className="arrow">›</div></div>
        </div>
        <div className="test-grid">
          {testimonials.map((t, i) => (
            <div className="test-card" key={i}>
              <div className="quote-mark">"</div>
              <p>{t.quote}</p>
              <div className="test-who">
                <div className="av" />
                <div><strong>{t.name}</strong><span>{t.role}</span></div>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="cta-row">
        <div className="art-box" />
        <div>
          <h3>Become an Instructor</h3>
          <p>
            Instructors from around the world teach millions of students on
            Byway. We provide the tools and skills to teach what you love.
          </p>
          <button className="btn btn-dark">Start Your Instructor Journey →</button>
        </div>
      </section>

      <section className="cta-row">
        <div>
          <h3>Transform your life through education</h3>
          <p>
            Learners around the world are launching new careers, advancing in
            their fields, and enriching their lives.
          </p>
          <Link to="/courses">
            <button className="btn btn-dark">Checkout Courses →</button>
          </Link>
        </div>
        <div className="art-box sky" />
      </section>
    </>
  );
}
