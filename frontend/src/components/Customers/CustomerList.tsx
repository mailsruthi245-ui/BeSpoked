import { useState } from 'react';
import { getCustomers, createCustomer } from '../../services/api';
import { Customer } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import Modal from '../common/Modal';

interface CustomerForm {
  firstName: string;
  lastName: string;
  address: string;
  phone: string;
  startDate: string;
}

const textFields: Array<{ key: keyof Omit<CustomerForm, 'startDate'>; label: string }> = [
  { key: 'firstName', label: 'First Name' },
  { key: 'lastName',  label: 'Last Name' },
  { key: 'address',   label: 'Address' },
  { key: 'phone',     label: 'Phone' },
];

const EMPTY_FORM: CustomerForm = {
  firstName: '', lastName: '', address: '', phone: '',
  startDate: new Date().toISOString().split('T')[0],
};

export default function CustomerList() {
  const { data: customers, loading, error, setData: setCustomers, setError } = useFetch<Customer[]>(getCustomers);
  const [modal, setModal] = useState<CustomerForm | null>(null);

  const set = (key: keyof CustomerForm, val: string) =>
    setModal(prev => prev ? { ...prev, [key]: val } : null);

  const handleSave = async () => {
    if (!modal) return;
    if (!modal.firstName || !modal.lastName || !modal.address || !modal.phone || !modal.startDate) {
      return setError('All fields are required.');
    }
    setError('');
    try {
      const created = await createCustomer(modal);
      setCustomers(prev => [...(prev ?? []), created]);
      setModal(null);
    } catch (e) {
      setError((e as Error).message);
    }
  };

  if (loading) return <p className="loading">Loading customers…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Customers</h1>
        <button className="btn btn-primary" onClick={() => setModal({ ...EMPTY_FORM })}>
          + New Customer
        </button>
      </div>
      {error && <p className="error">{error}</p>}

      <div className="card">
        <table>
          <thead>
            <tr><th>Name</th><th>Phone</th><th>Address</th><th>Customer Since</th></tr>
          </thead>
          <tbody>
            {(customers ?? []).map(c => (
              <tr key={c.id}>
                <td><strong>{c.firstName} {c.lastName}</strong></td>
                <td>{c.phone}</td>
                <td>{c.address}</td>
                <td>{new Date(c.startDate).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {!customers?.length && <p className="empty">No customers found.</p>}
      </div>

      {modal && (
        <Modal title="New Customer" onClose={() => setModal(null)} onSave={handleSave}>
          {textFields.map(({ key, label }) => (
            <div className="form-group" key={key}>
              <label>{label}</label>
              <input value={modal[key]} onChange={e => set(key, e.target.value)} />
            </div>
          ))}
          <div className="form-group">
            <label>Customer Since</label>
            <input type="date" value={modal.startDate} onChange={e => set('startDate', e.target.value)} />
          </div>
        </Modal>
      )}
    </div>
  );
}
