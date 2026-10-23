import { useState } from 'react';
import { courses as seedCourses } from '../../data/courses';
import Modal from '../../components/Modal';
import Stars from '../../components/Stars';

export default function AdminCourses() {
  const [courses] = useState(seedCourses);
  const [modal, setModal] = useState(null); // 'add1' | 'add2' | 'view' | 'delete' | null
  const [active, setActive] = useState(null);

  const openModal = (type, course = null) => {
    setActive(course);
    setModal(type);
  };
  const closeModal = () => setModal(null);

  return (
    <>
      <div className="topbar"><h1>Courses</h1><div className="bell" /></div>
      <div className="crumbs">Dashboard / <span className="cur">Courses</span></div>

      <div className="card">
        <div className="toolbar">
          <h3>Courses <span className="count-pill">200</span></h3>
          <div className="toolbar-actions">
            <button className="btn btn-dark" onClick={() => openModal('add1')}>Add Course</button>
            <select className="search-in"><option>All Categories</option></select>
            <input className="search-in" placeholder="Search for Courses" />
          </div>
        </div>
        <div className="grid3">
          {courses.map((c) => (
            <div className="gcard" key={c.id}>
              <div className="gthumb"><span className="badge">{c.category}</span></div>
              <div className="gbody">
                <h4>{c.title}</h4>
                <div className="by">By {c.instructor}</div>
                <Stars rating={c.rating} />
                <div className="meta">{c.hours} Total Hours · {c.lectures} Lectures · {c.level} · ${c.price.toFixed(2)}</div>
                <div className="gactions">
                  <button onClick={() => openModal('view', c)}>👁</button>
                  <button onClick={() => openModal('add1', c)}>✎</button>
                  <button style={{ color: 'var(--red)' }} onClick={() => openModal('delete', c)}>🗑</button>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="pager">
          <div className="pg">‹</div><div className="pg active">1</div><div className="pg">2</div><div className="pg">3</div><div className="pg">›</div>
        </div>
      </div>

      {/* Add/Edit Course — Step 1 */}
      <Modal wide open={modal === 'add1'} onClose={closeModal}>
        <h2>{active ? 'Update Course' : 'Add Course'}</h2>
        <div className="step-label">Step 1 of 2 · Course details</div>
        <div style={{ border: '1px dashed var(--line)', borderRadius: 10, padding: 16, marginBottom: 14, color: 'var(--ink-soft)', fontSize: 13 }}>
          Upload Image — 700x430 pixels · .jpg, .jpeg, .png, or .gif
        </div>
        <label className="field-lbl">Course Name</label>
        <input className="field-in" placeholder="Write here" defaultValue={active?.title || ''} />
        <div className="row2">
          <div>
            <label className="field-lbl">Category</label>
            <select className="field-in" defaultValue={active?.category || ''}>
              <option value="">Choose</option>
              <option>Frontend</option>
              <option>Backend</option>
              <option>UI/UX Design</option>
            </select>
          </div>
          <div>
            <label className="field-lbl">Level</label>
            <select className="field-in" defaultValue={active?.level || ''}>
              <option value="">Choose</option>
              <option>Beginner</option>
              <option>Intermediate</option>
              <option>Advanced</option>
            </select>
          </div>
        </div>
        <div className="row2">
          <div>
            <label className="field-lbl">Instructor</label>
            <select className="field-in" defaultValue={active?.instructor || ''}>
              <option value="">Choose</option>
              <option>Ronald Richards</option>
              <option>Jason Fahd</option>
            </select>
          </div>
          <div>
            <label className="field-lbl">Cost</label>
            <input className="field-in" placeholder="Write here" defaultValue={active?.price || ''} />
          </div>
        </div>
        <div className="row2">
          <div>
            <label className="field-lbl">Total hours</label>
            <input className="field-in" placeholder="Write here" defaultValue={active?.hours || ''} />
          </div>
          <div>
            <label className="field-lbl">Rate</label>
            <Stars rating={active?.rating || 0} />
          </div>
        </div>
        <label className="field-lbl">Description</label>
        <textarea className="field-in" rows={3} placeholder="Write here" />
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={closeModal}>Cancel</button>
          <button className="btn btn-dark" onClick={() => setModal('add2')}>Next</button>
        </div>
      </Modal>

      {/* Add/Edit Course — Step 2 */}
      <Modal wide open={modal === 'add2'} onClose={closeModal}>
        <h2>{active ? 'Update Course' : 'Add Course'}</h2>
        <div className="step-label">Step 2 of 2 · Add Content</div>
        <div style={{ background: 'var(--bg-soft)', borderRadius: 10, padding: 16, marginBottom: 14 }}>
          <label className="field-lbl">Name</label>
          <input className="field-in" placeholder="Write here" />
          <div className="row2">
            <div><label className="field-lbl">Lectures Number</label><input className="field-in" placeholder="Write here" /></div>
            <div><label className="field-lbl">Time</label><input className="field-in" placeholder="Write here" /></div>
          </div>
        </div>
        <button className="btn btn-ghost" style={{ width: '100%', marginBottom: 14 }}>+ Add Another Content</button>
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={closeModal}>Cancel</button>
          <button className="btn btn-dark" onClick={closeModal}>{active ? 'Save' : 'Add'}</button>
        </div>
      </Modal>

      <Modal wide open={modal === 'view'} onClose={closeModal}>
        <h2>Course Details</h2>
        <div className="gthumb" style={{ borderRadius: 10, marginBottom: 14 }} />
        <p><strong>{active?.title}</strong> — By {active?.instructor}</p>
        <p style={{ color: 'var(--ink-soft)', fontSize: 13.5 }}>
          {active?.hours} Total Hours · {active?.lectures} Lectures · {active?.level} · ${active?.price?.toFixed(2)}
        </p>
        <button className="btn btn-dark btn-block" onClick={closeModal}>Close</button>
      </Modal>

      <Modal open={modal === 'delete'} onClose={closeModal}>
        <div className="confirm-icon">🗑</div>
        <div className="confirm-text">
          Are you sure you want to delete this Course <strong>{active?.title}</strong>?
        </div>
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={closeModal}>Cancel</button>
          <button className="btn btn-red" onClick={closeModal}>Delete</button>
        </div>
      </Modal>
    </>
  );
}
