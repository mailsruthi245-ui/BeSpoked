import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getProducts, getSalespersons, getCustomers, createSale } from '../../services/api';

export default function CreateSale() {
  const navigate = useNavigate();
  const [products, setProducts]         = useState([]);
  const [salespersons, setSalespersons] = useState([]);
  const [customers, setCustomers]       = useState([]);
  const [error, setError]               = useState('');
  const [form, setForm] = useState({
    productId: '', salespersonId: '', customerId: '',
    salesDate: new Date().toISOString().split('T')[0]
  });

  useEffect(() => {
    Promise.all([getProducts(), getSalespersons(), getCustomers()])
      .then(([p, s, c]) => { setProducts(p); setSalespersons(s); setCustomers(c); });
  }, []);

  const set = (key, val) => setForm(f => ({ ...f, [key]: val }));

  const selectedProduct = products.find(p => p.id === +form.productId);

  const handleSubmit = async () => {
    if (!form.productId || !form.salespersonId || !form.customerId || !form.salesDate) {
      return setError('All fields are required.');
    }
    try {
      await createSale({ ...form, productId: +form.productId, salespersonId: +form.salespersonId, customerId: +form.customerId });
      navigate('/sales');
    } catch (e) {
      setError(e.message);
    }
  };

  // Filter out terminated salespersons
  const activeSalespersons = salespersons.filter(s => !s.terminationDate);

  return (
    <div>
      <div className="page-header"><h1>New Sale</h1></div>
      {error && <p className="error">{error}</p>}

      <div className="card" style={{ maxWidth: 600 }}>
        <div className="form-grid">
          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label>Product</label>
            <select value={form.productId} onChange={e => set('productId', e.target.value)}>
              <option value="">— Select a product —</option>
              {products.map(p => (
                <option key={p.id} value={p.id} disabled={p.qtyOnHand === 0}>
                  {p.name} — ${p.salePrice} ({p.qtyOnHand} in stock)
                </option>
              ))}
            </select>
          </div>

          {selectedProduct && (
            <div className="form-group" style={{ gridColumn: '1 / -1' }}>
              <label>Selected Product Details</label>
              <div style={{ background: '#f5f5f4', padding: '0.75rem', borderRadius: 6, fontSize: '0.875rem' }}>
                <strong>{selectedProduct.name}</strong> by {selectedProduct.manufacturer} &nbsp;|&nbsp;
                Style: {selectedProduct.style} &nbsp;|&nbsp;
                Commission: {selectedProduct.commissionPercentage}%
              </div>
            </div>
          )}

          <div className="form-group">
            <label>Salesperson</label>
            <select value={form.salespersonId} onChange={e => set('salespersonId', e.target.value)}>
              <option value="">— Select —</option>
              {activeSalespersons.map(s => (
                <option key={s.id} value={s.id}>{s.firstName} {s.lastName}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Customer</label>
            <select value={form.customerId} onChange={e => set('customerId', e.target.value)}>
              <option value="">— Select —</option>
              {customers.map(c => (
                <option key={c.id} value={c.id}>{c.firstName} {c.lastName}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Sale Date</label>
            <input type="date" value={form.salesDate} onChange={e => set('salesDate', e.target.value)} />
          </div>
        </div>

        <p style={{ marginTop: '1rem', fontSize: '0.8rem', color: '#6b7280' }}>
          💡 Any active discounts for the selected product and date will be applied automatically.
        </p>

        <div className="modal-actions" style={{ marginTop: '1.5rem' }}>
          <button className="btn btn-secondary" onClick={() => navigate('/sales')}>Cancel</button>
          <button className="btn btn-primary" onClick={handleSubmit}>Create Sale</button>
        </div>
      </div>
    </div>
  );
}
