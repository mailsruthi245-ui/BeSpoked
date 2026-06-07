import { getCustomers, createCustomer } from '../../services/api';
import { Customer } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import { useEditState } from '../../hooks/useEditState';
import Modal from '../common/Modal';
import FormField from '../common/FormField';

interface CustomerForm {
  firstName: string;
  lastName: string;
  address: string;
  phone: string;
  startDate: string;
}

const BLANK: CustomerForm = {
  firstName: '', lastName: '', address: '', phone: '',
  startDate: new Date().toISOString().split('T')[0],
};

export default function CustomerList() {
  const { data: customers, loading, error, setData: setCustomers, setError } = useFetch<Customer[]>(getCustomers);
  const { editing: modal, setEditing: setModal, set } = useEditState<CustomerForm>();

  const handleSave = async () => {
    if (!modal) return;
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
        <button className="btn btn-primary" onClick={() => setModal({ ...BLANK })}>+ New Customer</button>
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
          <FormField label="First Name"     value={modal.firstName} onChange={val => set('firstName', val)} />
          <FormField label="Last Name"      value={modal.lastName}  onChange={val => set('lastName', val)} />
          <FormField label="Address"        value={modal.address}   onChange={val => set('address', val)} />
          <FormField label="Phone"          value={modal.phone}     onChange={val => set('phone', val)} />
          <FormField label="Customer Since" type="date" value={modal.startDate} onChange={val => set('startDate', val)} />
        </Modal>
      )}
    </div>
  );
}
