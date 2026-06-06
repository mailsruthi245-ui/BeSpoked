import { useState } from 'react';
import { getSalespersons, createSalesperson, updateSalesperson } from '../../services/api';
import { Salesperson } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import Modal from '../common/Modal';

type StringField = 'firstName' | 'lastName' | 'address' | 'phone' | 'manager';

const textFields: Array<{ key: StringField; label: string }> = [
  { key: 'firstName', label: 'First Name' },
  { key: 'lastName',  label: 'Last Name' },
  { key: 'address',   label: 'Address' },
  { key: 'phone',     label: 'Phone' },
  { key: 'manager',   label: 'Manager' },
];

const BLANK: Salesperson = {
  id: 0, firstName: '', lastName: '', address: '', phone: '',
  startDate: new Date().toISOString().split('T')[0],
  terminationDate: null, manager: '',
};

export default function SalespersonList() {
  const { data: salespersons, loading, error, setData: setSalespersons, setError } = useFetch<Salesperson[]>(getSalespersons);
  const [editing, setEditing] = useState<Salesperson | null>(null);

  const set = (key: keyof Salesperson, val: string | null) =>
    setEditing(prev => prev ? { ...prev, [key]: val } : null);

  const handleSave = async () => {
    if (!editing) return;
    setError('');
    try {
      if (editing.id === 0) {
        const { id: _id, ...payload } = editing;
        const created = await createSalesperson(payload);
        setSalespersons(prev => [...(prev ?? []), created]);
      } else {
        await updateSalesperson(editing.id, editing);
        setSalespersons(prev => (prev ?? []).map(s => s.id === editing.id ? editing : s));
      }
      setEditing(null);
    } catch (e) {
      setError((e as Error).message);
    }
  };

  const fmt = (date: string | null) => date ? new Date(date).toLocaleDateString() : '—';

  if (loading) return <p className="loading">Loading salespersons…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Salespersons</h1>
        <button className="btn btn-primary" onClick={() => setEditing({ ...BLANK })}>
          + New Salesperson
        </button>
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
            {(salespersons ?? []).map(sp => (
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
        {!salespersons?.length && <p className="empty">No salespersons found.</p>}
      </div>

      {editing && (
        <Modal
          title={editing.id === 0 ? 'New Salesperson' : 'Edit Salesperson'}
          onClose={() => setEditing(null)}
          onSave={handleSave}
        >
          {textFields.map(({ key, label }) => (
            <div className="form-group" key={key}>
              <label>{label}</label>
              <input value={editing[key]} onChange={e => set(key, e.target.value)} />
            </div>
          ))}
          <div className="form-group">
            <label>Start Date</label>
            <input
              type="date"
              value={editing.startDate.split('T')[0]}
              onChange={e => set('startDate', e.target.value)}
            />
          </div>
          {editing.id !== 0 && (
            <div className="form-group">
              <label>Termination Date</label>
              <input
                type="date"
                value={editing.terminationDate ? editing.terminationDate.split('T')[0] : ''}
                onChange={e => set('terminationDate', e.target.value || null)}
              />
            </div>
          )}
        </Modal>
      )}
    </div>
  );
}
