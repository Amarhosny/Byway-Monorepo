import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { apiAssetUrl, apiRequest } from '../api/request';
import { testimonials } from '../data/courses';
import Stars from '../components/Stars';

export default function Landing() {
  const [content, setContent] = useState({ courses: [], categories: [], instructors: [] });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;
    Promise.all([
      apiRequest('/api/courses/top?count=4', { token: null }),
      apiRequest('/api/categories/top?count=4', { token: null }),
      apiRequest('/api/instructors/top?count=5', { token: null }),
    ]).then(([courses, categories, instructors]) => {
      if (active) setContent({ courses, categories, instructors });
    }).catch((requestError) => {
      if (active) setError(requestError.message || 'Unable to load featured content.');
    }).finally(() => {
      if (active) setLoading(false);
    });
    return () => { active = false; };
  }, []);

  return (
    <>
      <section className="hero">
        <div className="hero-grid">
          <div>
            <h1>Unlock Your Potential with Byway</h1>
            <p>Welcome to Byway, where learning knows no bounds. We believe that education is the key to personal and professional growth, and we're here to guide you on your journey to success.</p>
            <Link to="/courses"><button className="btn btn-dark" style={{ background: 'var(--blue)', borderColor: 'var(--blue)' }}>Start your journey</button></Link>
          </div>
          <div className="hero-art"><div className="bubble b1">Instructor</div><div className="bubble b2">Mentor</div><div className="bubble b3">Learn at your own pace</div></div>
        </div>
      </section>

      {error && <p className="wrap" role="alert" style={{ color: 'var(--red)' }}>{error}</p>}

      <section className="section">
        <div className="section-head"><h2>Top Categories</h2></div>
        {loading ? <p>Loading categories…</p> : content.categories.length === 0 ? <p>No categories are available yet.</p> : (
          <div className="cat-grid">
            {content.categories.map((category) => (
              <Link className="cat-card" key={category.id} to={`/courses?categoryId=${category.id}`}>
                <div className="cat-icon" style={category.imagePath ? { backgroundImage: `url("${apiAssetUrl(category.imagePath)}")`, backgroundSize: 'cover' } : undefined} />
                <h3>{category.name}</h3><span>{category.courseCount} Courses</span>
              </Link>
            ))}
          </div>
        )}
      </section>

      <section className="section">
        <div className="section-head"><h2>Top Courses</h2><Link className="seeall" to="/courses">See All</Link></div>
        {loading ? <p>Loading courses…</p> : content.courses.length === 0 ? <p>No courses are available yet.</p> : (
          <div className="course-grid">
            {content.courses.map((course) => (
              <Link className="course-card" to={`/courses/${course.id}`} key={course.id}>
                <div className="course-thumb" style={course.imagePath ? { backgroundImage: `url("${apiAssetUrl(course.imagePath)}")`, backgroundSize: 'cover' } : undefined}><span className="badge">{course.categoryName}</span></div>
                <div className="course-body"><h4>{course.name}</h4><div className="by">By {course.instructorName}</div>
                  <Stars rating={course.rating ?? 0} /><div className="meta">{course.totalHours} Total Hours · {course.level}</div><div className="price">${course.cost.toFixed(2)}</div>
                </div>
              </Link>
            ))}
          </div>
        )}
      </section>

      <section className="section">
        <div className="section-head"><h2>Top Instructors</h2></div>
        {loading ? <p>Loading instructors…</p> : content.instructors.length === 0 ? <p>No instructors are available yet.</p> : (
          <div className="instr-grid">
            {content.instructors.map((instructor) => (
              <div className="instr-card" key={instructor.id}>
                <div className="avatar" style={instructor.imagePath ? { backgroundImage: `url("${apiAssetUrl(instructor.imagePath)}")`, backgroundSize: 'cover' } : undefined} />
                <h4>{instructor.name}</h4><div style={{ fontSize: 12, color: 'var(--ink-soft)', marginBottom: 6 }}>{instructor.jobTitle}</div>
                <Stars rating={instructor.rating} />
              </div>
            ))}
          </div>
        )}
      </section>

      <section className="section">
        <div className="section-head"><h2>What Our Customers Say About Us</h2></div>
        <div className="test-grid">{testimonials.map((testimonial, index) => (
          <div className="test-card" key={index}><div className="quote-mark">"</div><p>{testimonial.quote}</p><div className="test-who"><div className="av" /><div><strong>{testimonial.name}</strong><span>{testimonial.role}</span></div></div></div>
        ))}</div>
        <p style={{ color: 'var(--ink-soft)', fontSize: 12 }}>Testimonials are sample content; the API does not currently provide testimonials.</p>
      </section>

      <section className="cta-row">
        <div className="art-box" /><div><h3>Become an Instructor</h3><p>Instructor applications are not available yet. Contact the Byway team to discuss teaching.</p><button className="btn btn-dark" disabled title="Instructor applications are not supported yet">Instructor applications unavailable</button></div>
      </section>
      <section className="cta-row">
        <div><h3>Transform your life through education</h3><p>Learners around the world are launching new careers, advancing in their fields, and enriching their lives.</p><Link to="/courses"><button className="btn btn-dark">Checkout Courses →</button></Link></div><div className="art-box sky" />
      </section>
    </>
  );
}
