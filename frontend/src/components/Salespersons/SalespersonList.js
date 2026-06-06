import React, { useEffect, useState } from 'react';
import { getSalespersons, updateSalesperson } from '../../services/api';

export default function SalespersonList() {
  const [salespersons, setSalespersons] = useState([]);
  const [loading, setLoading]           = useState(true);
  const [error, setError]               = useState('');
  const [editing, setEditing]           = useState(null);   // the row being edited

  useEffect(() => {
    getSalespersons()
      .then(setSalespersons)
      .catch(() => setError('Failed to load salespersons.'))
      .finally(() => setLoading(false));
  }, []);

  const handleSave = async () => {
    try {
      await updateSalesperson(editing.id, editing);
      setSalespersons(prev => prev.map(s => s.id === editing.id ? editing : s));
      setEditing(null);
    } catch (e) {
      setError(e.message);
    }
  };

  const fmt = (date) => date ? new Date(date).toLocaleDateString() : '—';

  if (loading) return <p className="loading">Loading salespersons…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Salespersons</h1>
      </div>

      {error && <p className="error">{error}</p>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Name</th><th>Phone</th><th>Address</th>
              <th>Start Date</th><th>Status</th><th>Manager</th><th></th>
            </tr>
          </thead>
          <tbody>
            {salespersons.map(sp => (
              <tr key={sp.id}>
                <td><strong>{sp.firstName} {sp.lastName}</strong></td>
                <td>{sp.phone}</td>
                <td>{sp.address}</td>
                <td>{fmt(sp.startDate)}</td>
                <td>
                  {sp.terminationDate
                    ? <span className="badge badge-orange">Terminated {fmt(sp.terminationDate)}</span>
                    : <span className="badge badge-green">Active</span>}
                </td>
                <td>{sp.manager}</td>
                <td>
                  <button className="btn btn-secondary btn-sm" onClick={() => setEditing({ ...sp })}>
                    Edit
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {salespersons.length === 0 && <p className="empty">No salespersons found.</p>}
      </div>

      {/* Edit Modal */}
      {editing && (
        <div className="modal-overlay" onClick={() => setEditing(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Edit Salesperson</h2>
            <div className="form-grid">
              {['firstName','lastName','address','phone','manager'].map(field => (
                <div className="form-group" key={field}>
                  <label>{field.replace(/([A-Z])/g, ' $1')}</label>
                  <input
                    value={editing[field] || ''}
                    onChange={e => setEditing({ ...editing, [field]: e.target.value })}
                  />
                </div>
              ))}
              <div className="form-group">
                <label>Termination Date</label>
                <input
                  type="date"
                  value={editing.terminationDate ? editing.terminationDate.split('T')[0] : ''}
                  onChange={e => setEditing({ ...editing, terminationDate: e.target.value || null })}
                />
              </div>
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
