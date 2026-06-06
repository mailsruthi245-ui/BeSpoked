import React, { useEffect, useState } from 'react';
import { getCustomers } from '../../services/api';

export default function CustomerList() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading]     = useState(true);

  useEffect(() => {
    getCustomers().then(setCustomers).finally(() => setLoading(false));
  }, []);

  if (loading) return <p className="loading">Loading customers…</p>;

  return (
    <div>
      <div className="page-header"><h1>Customers</h1></div>
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
    </div>
  );
}
