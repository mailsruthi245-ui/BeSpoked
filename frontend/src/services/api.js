const BASE = '/api';

async function request(path, options = {}) {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) {
    const err = await res.text();
    throw new Error(err || `HTTP ${res.status}`);
  }
  // 204 No Content has no body
  if (res.status === 204) return null;
  return res.json();
}

// ── Salespersons ──────────────────────────────────────────────────────────────
export const getSalespersons   = () => request('/salespersons');
export const updateSalesperson = (id, data) => request(`/salespersons/${id}`, { method: 'PUT', body: JSON.stringify(data) });

// ── Products ──────────────────────────────────────────────────────────────────
export const getProducts   = () => request('/products');
export const updateProduct = (id, data) => request(`/products/${id}`, { method: 'PUT', body: JSON.stringify(data) });

// ── Customers ─────────────────────────────────────────────────────────────────
export const getCustomers = () => request('/customers');

// ── Sales ─────────────────────────────────────────────────────────────────────
export const getSales     = (from, to) => {
  const params = new URLSearchParams();
  if (from) params.append('from', from);
  if (to)   params.append('to', to);
  const qs = params.toString();
  return request(`/sales${qs ? '?' + qs : ''}`);
};
export const createSale = (data) => request('/sales', { method: 'POST', body: JSON.stringify(data) });

// ── Commission Report ─────────────────────────────────────────────────────────
export const getQuarterlyReport = (year, quarter) =>
  request(`/sales/report?year=${year}&quarter=${quarter}`);
