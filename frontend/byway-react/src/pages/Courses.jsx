import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { apiAssetUrl, apiRequest } from '../api/request';
import Stars from '../components/Stars';

export default function Courses() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [result, setResult] = useState({ items: [], totalPages: 0, totalCount: 0, pageNumber: 1 });
  const [categories, setCategories] = useState([]);
  const [filters, setFilters] = useState(() => Object.fromEntries(searchParams.entries()));
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => setFilters(Object.fromEntries(searchParams.entries())), [searchParams]);

  useEffect(() => {
    let active = true;
    apiRequest('/api/categories', { token: null }).then((data) => { if (active) setCategories(data); }).catch(() => {});
    return () => { active = false; };
  }, []);

  useEffect(() => {
    let active = true;
    setLoading(true);
    setError('');
    const query = new URLSearchParams(searchParams);
    if (!query.has('pageSize')) query.set('pageSize', '9');
    apiRequest(`/api/courses?${query.toString()}`, { token: null }).then((data) => {
      if (active) setResult(data);
    }).catch((requestError) => {
      if (active) setError(requestError.message || 'Unable to load courses.');
    }).finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, [searchParams]);

  const updateFilter = (name, value) => {
    const next = new URLSearchParams(searchParams);
    if (value) next.set(name, value);
    else next.delete(name);
    next.delete('pageNumber');
    setSearchParams(next);
  };

  const goToPage = (page) => {
    const next = new URLSearchParams(searchParams);
    if (page <= 1) next.delete('pageNumber');
    else next.set('pageNumber', String(page));
    setSearchParams(next);
  };

  const pageNumber = result.pageNumber || Number(searchParams.get('pageNumber') || 1);

  return (
    <div className="courses-layout">
      <div>
        <div className="filter-box"><h4>Minimum Rating</h4>
          <select className="field-in" value={filters.minRating || ''} onChange={(event) => updateFilter('minRating', event.target.value)}>
            <option value="">Any rating</option><option value="4">4 stars & up</option><option value="3">3 stars & up</option>
          </select>
        </div>
        <div className="filter-box"><h4>Category</h4>
          <select className="field-in" value={filters.categoryId || ''} onChange={(event) => updateFilter('categoryId', event.target.value)}>
            <option value="">All categories</option>{categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
          </select>
        </div>
        <div className="filter-box"><h4>Level</h4>
          <select className="field-in" value={filters.level || ''} onChange={(event) => updateFilter('level', event.target.value)}>
            <option value="">All levels</option><option>AllLevels</option><option>Beginner</option><option>Intermediate</option><option>Expert</option>
          </select>
        </div>
        <div className="filter-box"><h4>Price range</h4>
          <input aria-label="Minimum price" className="field-in" type="number" min="0" placeholder="Minimum" value={filters.minPrice || ''} onChange={(event) => updateFilter('minPrice', event.target.value)} />
          <input aria-label="Maximum price" className="field-in" type="number" min="0" placeholder="Maximum" value={filters.maxPrice || ''} onChange={(event) => updateFilter('maxPrice', event.target.value)} />
        </div>
      </div>
      <div>
        <h2 style={{ margin: '0 0 4px' }}>Courses</h2>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
          <p style={{ color: 'var(--ink-soft)', margin: 0, fontSize: 14 }}>{result.totalCount} courses</p>
          <label>Sort by <select className="search-in" value={filters.sortBy || 'latest'} onChange={(event) => updateFilter('sortBy', event.target.value)}>
            <option value="latest">Latest</option><option value="price">Price</option><option value="rating">Rating</option>
          </select></label>
        </div>
        {error && <p role="alert" style={{ color: 'var(--red)' }}>{error}</p>}
        {loading ? <p>Loading courses…</p> : !error && result.items.length === 0 ? <p>No courses match these filters.</p> : (
          <div className="course-grid" style={{ gridTemplateColumns: 'repeat(3, 1fr)' }}>
            {result.items.map((course) => (
              <Link className="course-card" to={`/courses/${course.id}`} key={course.id}>
                <div className="course-thumb" style={course.imagePath ? { backgroundImage: `url("${apiAssetUrl(course.imagePath)}")`, backgroundSize: 'cover' } : undefined}><span className="badge">{course.categoryName}</span></div>
                <div className="course-body"><h4>{course.name}</h4><div className="by">By {course.instructorName}</div>
                  <Stars rating={course.rating ?? 0} /><div className="meta">{course.totalHours} Total Hours · {course.level}</div><div className="price">${course.cost.toFixed(2)}</div>
                </div>
              </Link>
            ))}
          </div>
        )}
        {result.totalPages > 1 && <div className="pager">
          <button className="pg" disabled={pageNumber <= 1} onClick={() => goToPage(pageNumber - 1)}>‹</button>
          {Array.from({ length: result.totalPages }, (_, index) => index + 1).map((page) => <button key={page} className={`pg${page === pageNumber ? ' active' : ''}`} onClick={() => goToPage(page)}>{page}</button>)}
          <button className="pg" disabled={pageNumber >= result.totalPages} onClick={() => goToPage(pageNumber + 1)}>›</button>
        </div>}
      </div>
    </div>
  );
}
