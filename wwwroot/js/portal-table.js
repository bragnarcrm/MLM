// Shared client-side data table: real pagination (Previous/Next), search and entries-per-page.
function initPortalTable(options) {
  const {
    url,
    tbody = document.querySelector('tbody'),
    emptyEl = document.querySelector('#empty'),
    countEl = document.querySelector('#count'),
    searchEl = document.querySelector('#search'),
    entriesEl = document.querySelector('#entries'),
    prevBtn,
    nextBtn,
    columnCount = 1,
    mapItems = (data) => data.items || [],
    renderRow,
    matches,
    emptyMessage = 'No data available in table',
    searchMessage = 'No matching records',
    errorMessage = 'Unable to load data.',
    authorize = true
  } = options;

  let allItems = [];
  let page = 1;

  const render = () => {
    const term = (searchEl?.value || '').trim().toLowerCase();
    const filtered = term
      ? allItems.filter((item) => (matches ? matches(item, term) : JSON.stringify(item).toLowerCase().includes(term)))
      : allItems;
    const pageSize = Number(entriesEl?.value || 10);
    const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
    page = Math.min(Math.max(page, 1), totalPages);
    const start = (page - 1) * pageSize;
    const visible = filtered.slice(start, start + pageSize);

    if (tbody) {
      tbody.innerHTML = visible.length
        ? visible.map((item, index) => renderRow(item, start + index)).join('')
        : `<tr><td class="empty text-center text-secondary py-4" colspan="${columnCount}">${term ? searchMessage : emptyMessage}</td></tr>`;
    }
    if (emptyEl && !tbody) emptyEl.textContent = visible.length ? '' : (term ? searchMessage : emptyMessage);
    if (countEl) countEl.textContent = `Showing ${visible.length ? start + 1 : 0} to ${start + visible.length} of ${filtered.length} entries`;
    if (prevBtn) prevBtn.disabled = page <= 1;
    if (nextBtn) nextBtn.disabled = page >= totalPages;
  };

  prevBtn?.addEventListener('click', () => { if (page > 1) { page -= 1; render(); } });
  nextBtn?.addEventListener('click', () => { page += 1; render(); });
  searchEl?.addEventListener('input', () => { page = 1; render(); });
  entriesEl?.addEventListener('change', () => { page = 1; render(); });

  const headers = authorize ? { Authorization: `Bearer ${localStorage.getItem('vitality.accessToken') || ''}` } : {};
  fetch(url, { headers })
    .then((response) => (response.ok ? response.json() : Promise.reject()))
    .then((data) => { allItems = mapItems(data); render(); })
    .catch(() => {
      if (tbody) tbody.innerHTML = `<tr><td class="empty text-center text-danger py-4" colspan="${columnCount}">${errorMessage}</td></tr>`;
      if (emptyEl && !tbody) emptyEl.textContent = errorMessage;
      if (countEl) countEl.textContent = 'Showing 0 to 0 of 0 entries';
    });

  return { render, refresh: (items) => { allItems = items; render(); }, getItems: () => allItems };
}
