import React, { useEffect, useState } from 'react';
import { getCustomers, createCustomer } from '../../services/api';
import { Customer } from '../../types';

interface CustomerForm {
  firstName: string;
  lastName: string;
  address: string;
  phone: string;
  startDate: string;
}

const EMPTY_FORM: CustomerForm = {
  firstName: '', lastName: '', address: '', phone: '',
  startDate: new Date().toISOString().split('T')[0],
};

export default function CustomerList() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState('');
  const [modal, setModal]         = useState<CustomerForm | null>(null);

  useEffect(() => {
    getCustomers()
      .then(setCustomers)
      .catch(() => setError('Failed to load customers.'))
      .finally(() => setLoading(false));
  }, []);

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
      setCustomers(prev => [...prev, created]);
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
            {customers.map(c => (
              <tr key={c.id}>
                <td><strong>{c.firstName} {c.lastName}</strong></td>
                <td>{c.phone}</td>
                <td>{c.address}</td>
                <td>{new Date(c.startDate).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {customers.length === 0 && <p className="empty">No customers found.</p>}
      </div>

      {modal && (
        <div className="modal-overlay" onClick={() => setModal(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>New Customer</h2>
            <div className="form-grid">
              {([
                { key: 'firstName' as const, label: 'First Name' },
                { key: 'lastName'  as const, label: 'Last Name' },
                { key: 'address'   as const, label: 'Address' },
                { key: 'phone'     as const, label: 'Phone' },
              ]).map(({ key, label }) => (
                <div className="form-group" key={key}>
                  <label>{label}</label>
                  <input
                    value={modal[key]}
                    onChange={e => set(key, e.target.value)}
                  />
                </div>
              ))}
              <div className="form-group">
                <label>Customer Since</label>
                <input
                  type="date"
                  value={modal.startDate}
                  onChange={e => set('startDate', e.target.value)}
                />
              </div>
            </div>
            <div className="modal-actions">
              <button className="btn btn-secondary" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary" onClick={handleSave}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
