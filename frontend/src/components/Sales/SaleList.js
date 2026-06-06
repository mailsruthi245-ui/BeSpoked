import React, { useEffect, useState } from 'react';
import { getSales } from '../../services/api';

const fmt$ = (v) => `$${Number(v).toFixed(2)}`;

export default function SaleList() {
  const [sales, setSales]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [from, setFrom]     = useState('');
  const [to, setTo]         = useState('');
  const [error, setError]   = useState('');

  const load = (f, t) => {
    setLoading(true);
    getSales(f || undefined, t || undefined)
      .then(setSales)
      .catch(() => setError('Failed to load sales.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  return (
    <div>
      <div className="page-header"><h1>Sales</h1></div>
      {error && <p className="error">{error}</p>}

      <div className="filters">
        <div className="form-group">
          <label>From</label>
          <input type="date" value={from} onChange={e => setFrom(e.target.value)} />
        </div>
        <div className="form-group">
          <label>To</label>
          <input type="date" value={to} onChange={e => setTo(e.target.value)} />
        </div>
        <button className="btn btn-primary" onClick={() => load(from, to)}>Filter</button>
        <button className="btn btn-secondary" onClick={() => { setFrom(''); setTo(''); load(); }}>Clear</button>
      </div>

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Date</th><th>Product</th><th>Customer</th>
              <th>Salesperson</th><th>Price</th><th>Discount</th><th>Commission</th>
            </tr>
          </thead>
          <tbody>
            {!loading && sales.map(s => (
              <tr key={s.id}>
                <td>{new Date(s.salesDate).toLocaleDateString()}</td>
                <td><strong>{s.product}</strong></td>
                <td>{s.customer}</td>
                <td>{s.salesperson}</td>
                <td>{fmt$(s.price)}</td>
                <td>{s.discountApplied > 0 ? <span className="badge badge-orange">{s.discountApplied}% off</span> : '—'}</td>
                <td className="badge badge-green">{fmt$(s.commission)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {loading && <p className="loading">Loading…</p>}
        {!loading && sales.length === 0 && <p className="empty">No sales found for this range.</p>}
      </div>
    </div>
  );
}
