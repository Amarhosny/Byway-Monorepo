import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { apiAssetUrl, apiRequest } from '../api/request';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import Stars from '../components/Stars';

const TABS = ['Description', 'Instructor', 'Content', 'Reviews'];

export default function CourseDetails() {
  const { id } = useParams();
  const [tab, setTab] = useState('Description');
  const [course, setCourse] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [actionError, setActionError] = useState('');
  const [actionSuccess, setActionSuccess] = useState('');
  const [busy, setBusy] = useState(false);
  const [rating, setRating] = useState('5');
  const [comment, setComment] = useState('');
  const { addItem } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();

  const loadCourse = async () => {
    const data = await apiRequest(`/api/courses/${id}`, { token: null });
    setCourse(data);
  };

  useEffect(() => {
    let active = true;
    setLoading(true);
    setError('');
    apiRequest(`/api/courses/${id}`, { token: null }).then((data) => { if (active) setCourse(data); })
      .catch((requestError) => { if (active) setError(requestError.message || 'Unable to load this course.'); })
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, [id]);

  const handleCartAction = async (destination) => {
    if (!user?.token) {
      setActionError('Please log in with your Byway account before adding a course to your cart.');
      return;
    }
    setBusy(true);
    setActionError('');
    setActionSuccess('');
    try {
      await addItem(course.id);
      if (destination) navigate(destination);
      else setActionSuccess('Course added to your cart.');
    } catch (requestError) {
      setActionError(requestError.message || 'Unable to update your cart.');
    } finally {
      setBusy(false);
    }
  };

  const handleReview = async (event) => {
    event.preventDefault();
    setBusy(true);
    setActionError('');
    try {
      await apiRequest(`/api/courses/${id}/reviews`, {
        method: 'POST', token: user?.token,
        body: JSON.stringify({ rating: Number(rating), comment: comment.trim() || null }),
      });
      setComment('');
      await loadCourse();
    } catch (requestError) {
      setActionError(requestError.message || 'Unable to submit this review.');
    } finally {
      setBusy(false);
    }
  };

  if (loading) return <div className="wrap"><p>Loading course…</p></div>;
  if (error || !course) return <div className="wrap"><p role="alert" style={{ color: 'var(--red)' }}>{error || 'Course not found.'}</p><Link to="/courses">Back to courses</Link></div>;

  const totalLectures = course.contents.reduce((sum, item) => sum + item.lecturesNumber, 0);
  return (
    <>
      <div className="wrap"><div className="crumbs"><Link to="/">Home</Link> &gt; <Link to="/courses">Courses</Link> &gt; <span className="cur">{course.name}</span></div></div>
      <div className="details-layout">
        <div>
          <h1 style={{ fontSize: 26, margin: '0 0 10px' }}>{course.name}</h1>
          <p style={{ color: 'var(--ink-soft)' }}>{course.description}</p>
          <div className="stars"><Stars rating={course.rating ?? 0} /><span style={{ color: 'var(--ink-soft)', fontSize: 13 }}> {course.rating ?? 'No'} ({course.reviewCount} reviews) · {course.totalHours} hours · {totalLectures} lectures · {course.level}</span></div>
          <p style={{ fontSize: 13.5 }}>Created by <span style={{ color: 'var(--blue)' }}>{course.instructorName}</span> · {course.categoryName}</p>
          {actionError && <p role="alert" style={{ color: 'var(--red)' }}>{actionError} {!user?.token && <Link to="/login">Log in</Link>}</p>}
          {actionSuccess && <p role="status" style={{ color: 'var(--green)' }}>{actionSuccess} <Link to="/cart">View cart</Link></p>}

          <div className="tabs">{TABS.map((name) => <button key={name} className={`tab ${tab === name ? 'active' : ''}`} onClick={() => setTab(name)}>{name}</button>)}</div>
          {tab === 'Description' && <><h3>Course Description</h3><p style={{ color: 'var(--ink-soft)', fontSize: 14.5 }}>{course.description}</p>{course.certification && <p><strong>Certification:</strong> {course.certification}</p>}</>}
          {tab === 'Instructor' && <><h3>Instructor</h3><div style={{ display: 'flex', gap: 16, alignItems: 'center' }}><div className="avatar" style={course.instructorImagePath ? { backgroundImage: `url("${apiAssetUrl(course.instructorImagePath)}")`, backgroundSize: 'cover' } : undefined} /><div><strong>{course.instructorName}</strong><p>{course.instructorJobTitle}</p><Stars rating={course.instructorRating} /></div></div><p style={{ color: 'var(--ink-soft)', fontSize: 14.5 }}>{course.instructorDescription}</p></>}
          {tab === 'Content' && <><h3>Course Content</h3>{course.contents.length ? course.contents.map((item) => <div className="content-row" key={item.id}><span>{item.name}</span><span style={{ color: 'var(--ink-soft)' }}>{item.lecturesNumber} Lectures · {item.time} min</span></div>) : <p>No course content has been added.</p>}</>}
          {tab === 'Reviews' && <><h3>Learner Reviews</h3>{course.reviews.length ? course.reviews.map((review) => <div className="review" key={review.id}><strong>{review.userDisplayName}</strong> <Stars rating={review.rating} /><p style={{ color: 'var(--ink-soft)', fontSize: 13.5 }}>{review.comment || 'No written comment.'}</p></div>) : <p>No reviews yet.</p>}
            {user?.token ? <form onSubmit={handleReview} style={{ marginTop: 20 }}><h4>Write a review</h4><label htmlFor="review-rating">Rating</label><select id="review-rating" className="field-in" value={rating} onChange={(event) => setRating(event.target.value)}>{[5, 4, 3, 2, 1].map((value) => <option key={value} value={value}>{value} stars</option>)}</select><label htmlFor="review-comment">Comment</label><textarea id="review-comment" className="field-in" maxLength={2000} value={comment} onChange={(event) => setComment(event.target.value)} /><button className="btn btn-dark" disabled={busy}>{busy ? 'Submitting…' : 'Submit review'}</button></form> : <p><Link to="/login">Log in</Link> to review a course you have purchased.</p>}
          </>}
        </div>
        <div className="buy-card">
          <div className="detail-hero" style={course.imagePath ? { backgroundImage: `url("${apiAssetUrl(course.imagePath)}")`, backgroundSize: 'cover' } : undefined} />
          <div className="price">${course.cost.toFixed(2)}</div>
          <button className="btn btn-dark btn-block" style={{ marginBottom: 8 }} disabled={busy} onClick={() => handleCartAction(null)}>{busy ? 'Please wait…' : 'Add To Cart'}</button>
          <button className="btn btn-block" disabled={busy} onClick={() => handleCartAction('/checkout')}>Buy Now</button>
        </div>
      </div>
      {course.relatedCourses.length > 0 && <section className="section"><h2>Related Courses</h2><div className="course-grid">{course.relatedCourses.map((related) => <Link className="course-card" to={`/courses/${related.id}`} key={related.id}><div className="course-thumb" /><div className="course-body"><h4>{related.name}</h4><div className="by">By {related.instructorName}</div><div className="price">${related.cost.toFixed(2)}</div></div></Link>)}</div></section>}
    </>
  );
}
