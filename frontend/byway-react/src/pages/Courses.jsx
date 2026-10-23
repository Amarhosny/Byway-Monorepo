import { Link } from 'react-router-dom';
import { courses } from '../data/courses';
import Stars from '../components/Stars';

export default function Courses() {
  return (
    <div className="courses-layout">
      <div>
        <div className="filter-box">
          <h4>Rating</h4>
          <Stars rating={5} />
        </div>
        <div className="filter-box">
          <h4>Number of Lectures</h4>
          <div className="chk"><input type="radio" name="lec" /> 1-15</div>
          <div className="chk"><input type="radio" name="lec" /> 16-30</div>
          <div className="chk"><input type="radio" name="lec" defaultChecked /> 31-45</div>
          <div className="chk"><input type="radio" name="lec" /> More than 45</div>
        </div>
        <div className="filter-box">
          <h4>Category</h4>
          <div className="chk"><input type="checkbox" defaultChecked /> Frontend</div>
          <div className="chk"><input type="checkbox" /> Backend</div>
          <div className="chk"><input type="checkbox" /> Testing</div>
          <div className="chk"><input type="checkbox" defaultChecked /> UI/UX Design</div>
        </div>
      </div>
      <div>
        <h2 style={{ margin: '0 0 4px' }}>Design Courses</h2>
        <p style={{ color: 'var(--ink-soft)', margin: '0 0 20px', fontSize: 14 }}>
          All Development Courses
        </p>
        <div className="course-grid" style={{ gridTemplateColumns: 'repeat(3, 1fr)' }}>
          {courses.map((c) => (
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
        <div className="pager">
          <div className="pg">‹</div>
          <div className="pg active">1</div>
          <div className="pg">2</div>
          <div className="pg">3</div>
          <div className="pg">›</div>
        </div>
      </div>
    </div>
  );
}
