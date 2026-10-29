import { useEffect, useRef, useState } from 'react';
import { apiAssetUrl, apiRequest } from '../../api/request';
import { useAuth } from '../../context/AuthContext';
import Modal from '../../components/Modal';
import Stars from '../../components/Stars';

const PAGE_SIZE = 10;
const JOB_TITLES = [
  ['FullstackDeveloper', 'Fullstack Developer'],
  ['BackendDeveloper', 'Backend Developer'],
  ['FrontendDeveloper', 'Frontend Developer'],
  ['UxUiDesigner', 'UX/UI Designer'],
];

export default function Instructors() {
  const { token } = useAuth();
  const requestSequence = useRef(0);
  const [result, setResult] = useState({ items: [], totalCount: 0, totalPages: 0, pageNumber: 1 });
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [modal, setModal] = useState(null);
  const [active, setActive] = useState(null);
  const [form, setForm] = useState({ name: '', jobTitle: '', rating: '0', description: '', imagePath: '' });
  const [imageFile, setImageFile] = useState(null);
  const [busy, setBusy] = useState(false);

  const loadInstructors = async () => {
    const sequence = ++requestSequence.current;
    setLoading(true);
    setError('');
    const query = new URLSearchParams({ pageNumber: String(page), pageSize: String(PAGE_SIZE) });
    if (search.trim()) query.set('search', search.trim());
    try {
      const data = await apiRequest(`/api/instructors?${query}`, { token });
      if (sequence === requestSequence.current) setResult(data);
    } catch (requestError) {
      if (sequence === requestSequence.current) setError(requestError.message || 'Unable to load instructors.');
    } finally {
      if (sequence === requestSequence.current) setLoading(false);
    }
  };

  useEffect(() => { loadInstructors(); }, [page, search, token]);

  const openForm = (instructor = null) => {
    setActive(instructor);
    setForm(instructor ? {
      name: instructor.name, jobTitle: instructor.jobTitle, rating: String(instructor.rating),
      description: instructor.description, imagePath: instructor.imagePath,
    } : { name: '', jobTitle: '', rating: '0', description: '', imagePath: '' });
    setImageFile(null);
    setError('');
    setModal('form');
  };

  const saveInstructor = async (event) => {
    event.preventDefault();
    if (busy) return;
    setBusy(true);
    setError('');
    try {
      let imagePath = form.imagePath;
      if (imageFile) {
        const upload = new FormData();
        upload.append('file', imageFile);
        const uploaded = await apiRequest('/api/files/upload?container=instructors', { method: 'POST', token, body: upload });
        imagePath = uploaded.path;
      }
      if (!imagePath) throw new Error('Select an instructor image before saving.');
      const body = JSON.stringify({ ...form, rating: Number(form.rating), imagePath });
      await apiRequest(active ? `/api/instructors/${active.id}` : '/api/instructors', { method: active ? 'PUT' : 'POST', token, body });
      setModal(null);
      await loadInstructors();
    } catch (requestError) { setError(requestError.message || 'Unable to save instructor.'); }
    finally { setBusy(false); }
  };

  const deleteInstructor = async () => {
    setBusy(true);
    setError('');
    try {
      await apiRequest(`/api/instructors/${active.id}`, { method: 'DELETE', token });
      setModal(null);
      await loadInstructors();
    } catch (requestError) { setError(requestError.message || 'Unable to delete instructor.'); }
    finally { setBusy(false); }
  };

  return <>
    <div className="topbar"><h1>Instructors</h1><div className="bell" /></div><div className="crumbs">Dashboard / <span className="cur">Instructors</span></div>
    <div className="card">
      <div className="toolbar"><h3>Instructors <span className="count-pill">{result.totalCount}</span></h3><div className="toolbar-actions"><button className="btn btn-dark" onClick={() => openForm()}>Add Instructor</button><input className="search-in" placeholder="Search for Instructors" value={search} onChange={(event) => { setSearch(event.target.value); setPage(1); }} /></div></div>
      {error && <p role="alert" style={{ color: 'var(--red)', padding: 12 }}>{error}</p>}
      <table><thead><tr><th>Name</th><th>Job Title</th><th>Rate</th><th>Action</th></tr></thead>
        <tbody>{loading ? <tr><td colSpan="4">Loading instructors…</td></tr> : result.items.length === 0 ? <tr><td colSpan="4">No instructors found.</td></tr> : result.items.map((instructor) => <tr key={instructor.id}>
          <td>{instructor.name}</td><td>{instructor.jobTitle}</td><td><Stars rating={instructor.rating} /></td><td className="acts"><button title="View" onClick={() => { setActive(instructor); setModal('view'); }}>View</button><button title="Edit" onClick={() => openForm(instructor)}>Edit</button><button className="del" title="Delete" onClick={() => { setActive(instructor); setModal('delete'); }}>Delete</button></td>
        </tr>)}</tbody>
      </table>
      {result.totalPages > 1 && <div className="pager"><button className="pg" disabled={page <= 1} onClick={() => setPage(page - 1)}>‹</button><span className="pg active">{page} / {result.totalPages}</span><button className="pg" disabled={page >= result.totalPages} onClick={() => setPage(page + 1)}>›</button></div>}
    </div>

    <Modal open={modal === 'form'} onClose={() => !busy && setModal(null)}>
      <form onSubmit={saveInstructor}><h2>{active ? 'Update Instructor' : 'Add Instructor'}</h2>
        {form.imagePath && <img src={apiAssetUrl(form.imagePath)} alt="Instructor" style={{ width: 96, height: 96, objectFit: 'cover', borderRadius: '50%' }} />}
        <label className="field-lbl" htmlFor="instructor-image">Image (.jpg, .jpeg, .png, .gif; max 5 MB)</label><input id="instructor-image" className="field-in" type="file" accept=".jpg,.jpeg,.png,.gif" onChange={(event) => setImageFile(event.target.files?.[0] || null)} />
        <label className="field-lbl" htmlFor="instructor-name">Name</label><input id="instructor-name" className="field-in" maxLength={100} required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
        <label className="field-lbl" htmlFor="instructor-title">Job Title</label><select id="instructor-title" className="field-in" required value={form.jobTitle} onChange={(event) => setForm({ ...form, jobTitle: event.target.value })}><option value="">Choose</option>{JOB_TITLES.map(([value, label]) => <option value={value} key={value}>{label}</option>)}</select>
        <label className="field-lbl" htmlFor="instructor-rating">Rating</label><input id="instructor-rating" className="field-in" type="number" min="0" max="5" step="0.1" required value={form.rating} onChange={(event) => setForm({ ...form, rating: event.target.value })} />
        <label className="field-lbl" htmlFor="instructor-description">Description</label><textarea id="instructor-description" className="field-in" rows={3} maxLength={2000} required value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
        {error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}
        <div className="modal-actions"><button type="button" className="btn btn-ghost" onClick={() => setModal(null)} disabled={busy}>Cancel</button><button className="btn btn-dark" disabled={busy}>{busy ? 'Saving…' : 'Save'}</button></div>
      </form>
    </Modal>
    <Modal open={modal === 'view'} onClose={() => setModal(null)}><h2>Instructor Details</h2>{active?.imagePath && <img src={apiAssetUrl(active.imagePath)} alt="" style={{ width: 96, height: 96, objectFit: 'cover', borderRadius: '50%' }} />}<p><strong>{active?.name}</strong></p><p>{active?.jobTitle} · <Stars rating={active?.rating ?? 0} /></p><p>{active?.description}</p><button className="btn btn-dark btn-block" onClick={() => setModal(null)}>Close</button></Modal>
    <Modal open={modal === 'delete'} onClose={() => !busy && setModal(null)}><div className="confirm-text">Delete instructor <strong>{active?.name}</strong>?</div>{error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}<div className="modal-actions"><button className="btn btn-ghost" onClick={() => setModal(null)} disabled={busy}>Cancel</button><button className="btn btn-red" onClick={deleteInstructor} disabled={busy}>{busy ? 'Deleting…' : 'Delete'}</button></div></Modal>
  </>;
}
