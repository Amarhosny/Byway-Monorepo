import { useState } from 'react';
import { instructors as seedInstructors } from '../../data/instructors';
import Modal from '../../components/Modal';
import Stars from '../../components/Stars';

export default function Instructors() {
  const [instructors] = useState(seedInstructors);
  const [modal, setModal] = useState(null); // 'add' | 'view' | 'delete' | null
  const [active, setActive] = useState(null);

  const openModal = (type, instructor = null) => {
    setActive(instructor);
    setModal(type);
  };
  const closeModal = () => setModal(null);

  return (
    <>
      <div className="topbar"><h1>Instructors</h1><div className="bell" /></div>
      <div className="crumbs">Dashboard / <span className="cur">Instructors</span></div>

      <div className="card">
        <div className="toolbar">
          <h3>Instructors <span className="count-pill">200</span></h3>
          <div className="toolbar-actions">
            <button className="btn btn-dark" onClick={() => openModal('add')}>Add Instructor</button>
            <input className="search-in" placeholder="Search for Instructors" />
          </div>
        </div>
        <table>
          <thead>
            <tr><th>Name</th><th>Job Title</th><th>Rate</th><th>Action</th></tr>
          </thead>
          <tbody>
            {instructors.map((i) => (
              <tr key={i.id}>
                <td>{i.name}</td>
                <td>{i.title}</td>
                <td><Stars rating={i.rating} /></td>
                <td className="acts">
                  <button onClick={() => openModal('view', i)}>👁</button>
                  <button onClick={() => openModal('add', i)}>✎</button>
                  <button className="del" onClick={() => openModal('delete', i)}>🗑</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pager">
          <div className="pg">‹</div><div className="pg active">1</div><div className="pg">2</div><div className="pg">3</div><div className="pg">›</div>
        </div>
      </div>

      <Modal open={modal === 'add'} onClose={closeModal}>
        <h2>{active ? 'Update Instructor' : 'Add Instructor'}</h2>
        <div className="up-avatar" />
        <label className="field-lbl">Name</label>
        <input className="field-in" placeholder="Write here" defaultValue={active?.name || ''} />
        <div className="row2">
          <div>
            <label className="field-lbl">Job Title</label>
            <select className="field-in" defaultValue={active?.title || ''}>
              <option value="">Choose</option>
              <option>Frontend</option>
              <option>Backend</option>
              <option>UI/UX Design</option>
            </select>
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
          <button className="btn btn-dark" onClick={closeModal}>{active ? 'Save' : 'Add'}</button>
        </div>
      </Modal>

      <Modal open={modal === 'view'} onClose={closeModal}>
        <h2>Instructor Details</h2>
        <div className="up-avatar" />
        <p><strong>Name:</strong> {active?.name}</p>
        <p><strong>Job Title:</strong> {active?.title}</p>
        <p><strong>Rate:</strong> <Stars rating={active?.rating || 0} /></p>
        <p style={{ color: 'var(--ink-soft)', fontSize: 13.5 }}>
          With over a decade of industry experience, bringing a wealth of
          practical knowledge to the classroom.
        </p>
        <button className="btn btn-dark btn-block" onClick={closeModal}>Close</button>
      </Modal>

      <Modal open={modal === 'delete'} onClose={closeModal}>
        <div className="confirm-icon">🗑</div>
        <div className="confirm-text">
          Are you sure you want to delete this Instructor <strong>{active?.name}</strong>?
        </div>
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={closeModal}>Cancel</button>
          <button className="btn btn-red" onClick={closeModal}>Delete</button>
        </div>
      </Modal>
    </>
  );
}
