import { useEffect, useRef, useState } from 'react';
import { apiAssetUrl, apiRequest } from '../../api/request';
import { useAuth } from '../../context/AuthContext';
import Modal from '../../components/Modal';
import Stars from '../../components/Stars';

const PAGE_SIZE = 9;
const LEVELS = ['AllLevels', 'Beginner', 'Intermediate', 'Expert'];
const emptyForm = () => ({ name: '', categoryId: '', instructorId: '', level: '', cost: '', totalHours: '', description: '', certification: '', imagePath: '', contents: [{ name: '', lecturesNumber: '1', time: '60', sortOrder: 0 }] });

export default function AdminCourses() {
  const { token } = useAuth();
  const requestSequence = useRef(0);
  const [result, setResult] = useState({ items: [], totalCount: 0, totalPages: 0, pageNumber: 1 });
  const [categories, setCategories] = useState([]);
  const [instructors, setInstructors] = useState([]);
  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [editorLoading, setEditorLoading] = useState(false);
  const [error, setError] = useState('');
  const [modal, setModal] = useState(null);
  const [active, setActive] = useState(null);
  const [form, setForm] = useState(emptyForm);
  const [imageFile, setImageFile] = useState(null);
  const [busy, setBusy] = useState(false);

  const loadCourses = async () => {
    const sequence = ++requestSequence.current;
    setLoading(true);
    setError('');
    const query = new URLSearchParams({ pageNumber: String(page), pageSize: String(PAGE_SIZE) });
    if (search.trim()) query.set('search', search.trim());
    if (categoryId) query.set('categoryId', categoryId);
    try {
      const data = await apiRequest(`/api/admin/courses?${query}`, { token });
      if (sequence === requestSequence.current) setResult(data);
    } catch (requestError) {
      if (sequence === requestSequence.current) setError(requestError.message || 'Unable to load courses.');
    } finally {
      if (sequence === requestSequence.current) setLoading(false);
    }
  };

  useEffect(() => { loadCourses(); }, [page, search, categoryId, token]);
  useEffect(() => {
    let activeRequest = true;
    Promise.all([
      apiRequest('/api/categories', { token }),
      apiRequest('/api/instructors?pageNumber=1&pageSize=50', { token }),
    ]).then(async ([categoryData, firstInstructorPage]) => {
      const remainingPages = await Promise.all(Array.from({ length: Math.max(0, firstInstructorPage.totalPages - 1) }, (_, index) =>
        apiRequest(`/api/instructors?pageNumber=${index + 2}&pageSize=50`, { token })));
      if (activeRequest) { setCategories(categoryData); setInstructors([...firstInstructorPage.items, ...remainingPages.flatMap((pageResult) => pageResult.items)]); }
    }).catch((requestError) => { if (activeRequest) setError(requestError.message || 'Unable to load course options.'); });
    return () => { activeRequest = false; };
  }, [token]);

  const openCreate = () => {
    setActive(null); setForm(emptyForm()); setImageFile(null); setError(''); setModal('form');
  };

  const openEdit = async (course) => {
    setActive(course); setError(''); setEditorLoading(true); setModal('form');
    try {
      const details = await apiRequest(`/api/courses/${course.id}`, { token });
      setForm({
        name: details.name, categoryId: String(details.categoryId), instructorId: String(details.instructorId),
        level: details.level, cost: String(details.cost), totalHours: String(details.totalHours),
        description: details.description, certification: details.certification || '', imagePath: details.imagePath,
        contents: details.contents.map((item, index) => ({ name: item.name, lecturesNumber: String(item.lecturesNumber), time: String(item.time), sortOrder: index })),
      });
    } catch (requestError) { setError(requestError.message || 'Unable to load course details.'); }
    finally { setEditorLoading(false); }
  };

  const updateContent = (index, field, value) => setForm((current) => ({ ...current, contents: current.contents.map((item, itemIndex) => itemIndex === index ? { ...item, [field]: value } : item) }));
  const addContent = () => setForm((current) => ({ ...current, contents: [...current.contents, { name: '', lecturesNumber: '1', time: '60', sortOrder: current.contents.length }] }));
  const removeContent = (index) => setForm((current) => ({ ...current, contents: current.contents.filter((_, itemIndex) => itemIndex !== index).map((item, itemIndex) => ({ ...item, sortOrder: itemIndex })) }));

  const saveCourse = async (event) => {
    event.preventDefault();
    if (busy || editorLoading) return;
    setBusy(true); setError('');
    try {
      let imagePath = form.imagePath;
      if (imageFile) {
        const upload = new FormData(); upload.append('file', imageFile);
        const uploaded = await apiRequest('/api/files/upload?container=courses', { method: 'POST', token, body: upload });
        imagePath = uploaded.path;
      }
      if (!imagePath) throw new Error('Select a course image before saving.');
      const request = {
        name: form.name,
        categoryId: Number(form.categoryId),
        instructorId: Number(form.instructorId),
        level: form.level,
        cost: Number(form.cost),
        totalHours: Number(form.totalHours),
        description: form.description,
        certification: form.certification || null,
        imagePath,
        contents: form.contents.map((item, index) => ({ name: item.name, lecturesNumber: Number(item.lecturesNumber), time: Number(item.time), sortOrder: index })),
      };
      await apiRequest(active ? `/api/courses/${active.id}` : '/api/courses', { method: active ? 'PUT' : 'POST', token, body: JSON.stringify(request) });
      setModal(null);
      await loadCourses();
    } catch (requestError) { setError(requestError.message || 'Unable to save course.'); }
    finally { setBusy(false); }
  };

  const deleteCourse = async () => {
    setBusy(true); setError('');
    try {
      await apiRequest(`/api/courses/${active.id}`, { method: 'DELETE', token });
      setModal(null); await loadCourses();
    } catch (requestError) { setError(requestError.message || 'Unable to delete course.'); }
    finally { setBusy(false); }
  };

  return <>
    <div className="topbar"><h1>Courses</h1><div className="bell" /></div><div className="crumbs">Dashboard / <span className="cur">Courses</span></div>
    <div className="card">
      <div className="toolbar"><h3>Courses <span className="count-pill">{result.totalCount}</span></h3><div className="toolbar-actions">
        <button className="btn btn-dark" onClick={openCreate}>Add Course</button>
        <select aria-label="Filter by category" className="search-in" value={categoryId} onChange={(event) => { setCategoryId(event.target.value); setPage(1); }}><option value="">All Categories</option>{categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}</select>
        <input className="search-in" placeholder="Search for Courses" value={search} onChange={(event) => { setSearch(event.target.value); setPage(1); }} />
      </div></div>
      {error && modal !== 'form' && modal !== 'delete' && <p role="alert" style={{ color: 'var(--red)', padding: 12 }}>{error}</p>}
      {loading ? <p>Loading courses…</p> : result.items.length === 0 ? <p>No courses found.</p> : <div className="grid3">{result.items.map((course) => <div className="gcard" key={course.id}>
        <div className="gthumb" style={course.imagePath ? { backgroundImage: `url("${apiAssetUrl(course.imagePath)}")`, backgroundSize: 'cover' } : undefined}><span className="badge">{course.categoryName}</span></div>
        <div className="gbody"><h4>{course.name}</h4><div className="by">By {course.instructorName}</div><Stars rating={course.rating ?? 0} /><div className="meta">{course.totalHours} hours · {course.level} · ${course.cost.toFixed(2)}</div>
          <div className="gactions"><button onClick={async () => { try { setActive(await apiRequest(`/api/courses/${course.id}`, { token })); setModal('view'); } catch (requestError) { setError(requestError.message); } }}>View</button><button onClick={() => openEdit(course)}>Edit</button><button style={{ color: 'var(--red)' }} onClick={() => { setActive(course); setModal('delete'); setError(''); }}>Delete</button></div>
        </div>
      </div>)}</div>}
      {result.totalPages > 1 && <div className="pager"><button className="pg" disabled={page <= 1} onClick={() => setPage(page - 1)}>‹</button><span className="pg active">{page} / {result.totalPages}</span><button className="pg" disabled={page >= result.totalPages} onClick={() => setPage(page + 1)}>›</button></div>}
    </div>

    <Modal wide open={modal === 'form'} onClose={() => !busy && setModal(null)}>
      <form onSubmit={saveCourse}><h2>{active ? 'Update Course' : 'Add Course'}</h2>
        {editorLoading ? <p>Loading course details…</p> : <>
          {form.imagePath && <img src={apiAssetUrl(form.imagePath)} alt="Course" style={{ maxWidth: 180, maxHeight: 110, objectFit: 'cover' }} />}
          <label className="field-lbl" htmlFor="course-image">Image (.jpg, .jpeg, .png, .gif; max 5 MB)</label><input id="course-image" className="field-in" type="file" accept=".jpg,.jpeg,.png,.gif" onChange={(event) => setImageFile(event.target.files?.[0] || null)} />
          <label className="field-lbl" htmlFor="course-name">Course Name</label><input id="course-name" className="field-in" maxLength={150} required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
          <div className="row2"><div><label className="field-lbl" htmlFor="course-category">Category</label><select id="course-category" className="field-in" required value={form.categoryId} onChange={(event) => setForm({ ...form, categoryId: event.target.value })}><option value="">Choose</option>{categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}</select></div>
            <div><label className="field-lbl" htmlFor="course-instructor">Instructor</label><select id="course-instructor" className="field-in" required value={form.instructorId} onChange={(event) => setForm({ ...form, instructorId: event.target.value })}><option value="">Choose</option>{instructors.map((instructor) => <option key={instructor.id} value={instructor.id}>{instructor.name}</option>)}</select></div></div>
          <div className="row2"><div><label className="field-lbl" htmlFor="course-level">Level</label><select id="course-level" className="field-in" required value={form.level} onChange={(event) => setForm({ ...form, level: event.target.value })}><option value="">Choose</option>{LEVELS.map((level) => <option key={level}>{level}</option>)}</select></div><div><label className="field-lbl" htmlFor="course-cost">Cost</label><input id="course-cost" className="field-in" type="number" min="0" max="100000" step="0.01" required value={form.cost} onChange={(event) => setForm({ ...form, cost: event.target.value })} /></div></div>
          <div className="row2"><div><label className="field-lbl" htmlFor="course-hours">Total hours</label><input id="course-hours" className="field-in" type="number" min="1" max="2000" required value={form.totalHours} onChange={(event) => setForm({ ...form, totalHours: event.target.value })} /></div><div><label className="field-lbl" htmlFor="course-certification">Certification (optional)</label><input id="course-certification" className="field-in" value={form.certification} onChange={(event) => setForm({ ...form, certification: event.target.value })} /></div></div>
          <label className="field-lbl" htmlFor="course-description">Description</label><textarea id="course-description" className="field-in" rows={3} maxLength={4000} required value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
          <h3>Course content</h3>{form.contents.map((item, index) => <div key={index} style={{ background: 'var(--bg-soft)', borderRadius: 10, padding: 12, marginBottom: 10 }}>
            <label className="field-lbl">Section name</label><input className="field-in" maxLength={150} required value={item.name} onChange={(event) => updateContent(index, 'name', event.target.value)} />
            <div className="row2"><div><label className="field-lbl">Lectures</label><input className="field-in" type="number" min="1" max="500" required value={item.lecturesNumber} onChange={(event) => updateContent(index, 'lecturesNumber', event.target.value)} /></div><div><label className="field-lbl">Time (minutes)</label><input className="field-in" type="number" min="1" max="10000" required value={item.time} onChange={(event) => updateContent(index, 'time', event.target.value)} /></div></div>
            {form.contents.length > 1 && <button type="button" className="btn btn-ghost" onClick={() => removeContent(index)}>Remove section</button>}
          </div>)}
          <button type="button" className="btn btn-ghost" style={{ width: '100%', marginBottom: 14 }} onClick={addContent}>+ Add content section</button>
        </>}
        {error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}
        <div className="modal-actions"><button type="button" className="btn btn-ghost" onClick={() => setModal(null)} disabled={busy}>Cancel</button><button className="btn btn-dark" disabled={busy || editorLoading}>{busy ? 'Saving…' : 'Save Course'}</button></div>
      </form>
    </Modal>
    <Modal wide open={modal === 'view'} onClose={() => setModal(null)}><h2>Course Details</h2>{active?.imagePath && <img src={apiAssetUrl(active.imagePath)} alt="" style={{ maxWidth: '100%', maxHeight: 180, objectFit: 'cover' }} />}<p><strong>{active?.name}</strong> · {active?.categoryName}</p><p>By {active?.instructorName} · {active?.level} · {active?.totalHours} hours · ${active?.cost?.toFixed(2)}</p><Stars rating={active?.rating ?? 0} /><p>{active?.description}</p><h4>Content</h4>{active?.contents?.map((item) => <div className="content-row" key={item.id}>{item.name} · {item.lecturesNumber} lectures · {item.time} min</div>)}<button className="btn btn-dark btn-block" onClick={() => setModal(null)}>Close</button></Modal>
    <Modal open={modal === 'delete'} onClose={() => !busy && setModal(null)}><div className="confirm-text">Delete course <strong>{active?.name}</strong>?</div><p>This action may be rejected if the course has paid purchases.</p>{error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}<div className="modal-actions"><button className="btn btn-ghost" onClick={() => setModal(null)} disabled={busy}>Cancel</button><button className="btn btn-red" onClick={deleteCourse} disabled={busy}>{busy ? 'Deleting…' : 'Delete'}</button></div></Modal>
  </>;
}
