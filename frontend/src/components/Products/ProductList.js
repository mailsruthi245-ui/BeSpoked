import React, { useEffect, useState } from 'react';
import { getProducts, updateProduct } from '../../services/api';

const fmt$ = (v) => `$${Number(v).toFixed(2)}`;

export default function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');
  const [editing, setEditing]   = useState(null);

  useEffect(() => {
    getProducts()
      .then(setProducts)
      .catch(() => setError('Failed to load products.'))
      .finally(() => setLoading(false));
  }, []);

  const handleSave = async () => {
    try {
      await updateProduct(editing.id, editing);
      setProducts(prev => prev.map(p => p.id === editing.id ? editing : p));
      setEditing(null);
    } catch (e) {
      setError(e.message);
    }
  };

  if (loading) return <p className="loading">Loading products…</p>;

  return (
    <div>
      <div className="page-header"><h1>Products</h1></div>
      {error && <p className="error">{error}</p>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Name</th><th>Manufacturer</th><th>Style</th>
              <th>Buy Price</th><th>Sale Price</th><th>Qty</th><th>Commission %</th><th></th>
            </tr>
          </thead>
          <tbody>
            {products.map(p => (
              <tr key={p.id}>
                <td><strong>{p.name}</strong></td>
                <td>{p.manufacturer}</td>
                <td>{p.style}</td>
                <td>{fmt$(p.purchasePrice)}</td>
                <td>{fmt$(p.salePrice)}</td>
                <td>
                  <span className={`badge ${p.qtyOnHand > 5 ? 'badge-green' : p.qtyOnHand > 0 ? 'badge-orange' : 'badge-orange'}`}>
                    {p.qtyOnHand}
                  </span>
                </td>
                <td>{p.commissionPercentage}%</td>
                <td>
                  <button className="btn btn-secondary btn-sm" onClick={() => setEditing({ ...p })}>Edit</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {products.length === 0 && <p className="empty">No products found.</p>}
      </div>

      {editing && (
        <div className="modal-overlay" onClick={() => setEditing(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Edit Product</h2>
            <div className="form-grid">
              {[
                { key: 'name', label: 'Name' },
                { key: 'manufacturer', label: 'Manufacturer' },
                { key: 'style', label: 'Style' },
                { key: 'purchasePrice', label: 'Purchase Price', type: 'number' },
                { key: 'salePrice', label: 'Sale Price', type: 'number' },
                { key: 'qtyOnHand', label: 'Qty On Hand', type: 'number' },
                { key: 'commissionPercentage', label: 'Commission %', type: 'number' },
              ].map(({ key, label, type = 'text' }) => (
                <div className="form-group" key={key}>
                  <label>{label}</label>
                  <input
                    type={type}
                    value={editing[key] ?? ''}
                    onChange={e => setEditing({ ...editing, [key]: type === 'number' ? +e.target.value : e.target.value })}
                  />
                </div>
              ))}
            </div>
            <div className="modal-actions">
              <button className="btn btn-secondary" onClick={() => setEditing(null)}>Cancel</button>
              <button className="btn btn-primary" onClick={handleSave}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
