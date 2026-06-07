import { getSalespersons, createSalesperson, updateSalesperson } from '../../services/api';
import { Salesperson } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import { useEditState } from '../../hooks/useEditState';
import Modal from '../common/Modal';
import FormField from '../common/FormField';

const BLANK: Salesperson = {
  id: 0, firstName: '', lastName: '', address: '', phone: '',
  startDate: new Date().toISOString().split('T')[0],
  terminationDate: null, manager: '',
};

const fmt = (date: string | null) => date ? new Date(date).toLocaleDateString() : '—';

export default function SalespersonList() {
  const { data: salespersons, loading, error, setData: setSalespersons, setError } = useFetch<Salesperson[]>(getSalespersons);
  const { editing, setEditing, set } = useEditState<Salesperson>();

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

  if (loading) return <p className="loading">Loading salespersons…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Salespersons</h1>
        <button className="btn btn-primary" onClick={() => setEditing({ ...BLANK })}>+ New Salesperson</button>
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
                  {sp.terminationDate && new Date(sp.terminationDate) <= new Date()
                    ? <span className="badge badge-orange">Terminated {fmt(sp.terminationDate)}</span>
                    : <span className="badge badge-green">Active</span>}
                </td>
                <td>{sp.manager}</td>
                <td>
                  <button className="btn btn-secondary btn-sm" onClick={() => setEditing({ ...sp })}>Edit</button>
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
          <FormField label="First Name"  value={editing.firstName} onChange={val => set('firstName', val)} />
          <FormField label="Last Name"   value={editing.lastName}  onChange={val => set('lastName', val)} />
          <FormField label="Address"     value={editing.address}   onChange={val => set('address', val)} />
          <FormField label="Phone"       value={editing.phone}     onChange={val => set('phone', val)} />
          <FormField label="Manager"     value={editing.manager}   onChange={val => set('manager', val)} />
          <FormField label="Start Date"  type="date" value={editing.startDate.split('T')[0]} onChange={val => set('startDate', val)} />
          {editing.id !== 0 && (
            <FormField
              label="Termination Date"
              type="date"
              value={editing.terminationDate ? editing.terminationDate.split('T')[0] : ''}
              onChange={val => set('terminationDate', val || null)}
            />
          )}
        </Modal>
      )}
    </div>
  );
}
