import {
  Salesperson, Product, Customer, Sale,
  QuarterlyReportData, Discount, DiscountPayload, CreateSaleRequest,
} from '../types';

const BASE = '/api';

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) {
    const raw = await res.text();
    let msg = `HTTP ${res.status}`;
    try {
      const json = JSON.parse(raw);
      msg = json.detail ?? json.error ?? json.title ?? msg;
    } catch {
      msg = raw || msg;
    }
    throw new Error(msg);
  }
  if (res.status === 204) return null as unknown as T;
  return res.json() as Promise<T>;
}

// ── Salespersons ──────────────────────────────────────────────────────────────
export const getSalespersons   = () => request<Salesperson[]>('/salespersons');
export const createSalesperson = (data: Omit<Salesperson, 'id'>) =>
  request<Salesperson>('/salespersons', { method: 'POST', body: JSON.stringify({ id: 0, ...data }) });
export const updateSalesperson = (id: number, data: Salesperson) =>
  request<null>(`/salespersons/${id}`, { method: 'PUT', body: JSON.stringify(data) });

// ── Products ──────────────────────────────────────────────────────────────────
export const getProducts    = () => request<Product[]>('/products');
export const createProduct  = (data: Omit<Product, 'id'>) =>
  request<Product>('/products', { method: 'POST', body: JSON.stringify({ id: 0, ...data }) });
export const updateProduct  = (id: number, data: Product) =>
  request<null>(`/products/${id}`, { method: 'PUT', body: JSON.stringify(data) });

// ── Customers ─────────────────────────────────────────────────────────────────
export const getCustomers   = () => request<Customer[]>('/customers');
export const createCustomer = (data: Omit<Customer, 'id'>) =>
  request<Customer>('/customers', { method: 'POST', body: JSON.stringify({ id: 0, ...data }) });

// ── Sales ─────────────────────────────────────────────────────────────────────
export const getSales = (from?: string, to?: string) => {
  const params = new URLSearchParams();
  if (from) params.append('from', from);
  if (to)   params.append('to', to);
  const qs = params.toString();
  return request<Sale[]>(`/sales${qs ? '?' + qs : ''}`);
};
export const createSale = (data: CreateSaleRequest) =>
  request<number>('/sales', { method: 'POST', body: JSON.stringify(data) });

// ── Discounts ─────────────────────────────────────────────────────────────────
export const getDiscounts   = () => request<Discount[]>('/discounts');
export const createDiscount = (data: DiscountPayload) =>
  request<Discount>('/discounts', { method: 'POST', body: JSON.stringify(data) });
export const updateDiscount = (id: number, data: DiscountPayload) =>
  request<null>(`/discounts/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const deleteDiscount = (id: number) =>
  request<null>(`/discounts/${id}`, { method: 'DELETE' });

// ── Commission Report ─────────────────────────────────────────────────────────
export const getQuarterlyReport = (year: number, quarter: number) =>
  request<QuarterlyReportData>(`/sales/report?year=${year}&quarter=${quarter}`);
