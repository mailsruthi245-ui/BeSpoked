import React, { useState } from 'react';
import { getDiscounts, createDiscount, updateDiscount, deleteDiscount, getProducts } from '../../services/api';
import { Discount, Product } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import Modal from '../common/Modal';

const fmtDate = (d: string) => new Date(d).toLocaleDateString();
const toInput  = (d: string) => new Date(d).toISOString().split('T')[0];

interface DiscountForm {
  id: number;
  productId: string;
  beginDate: string;
  endDate: string;
  discountPercentage: string;
}

const EMPTY_FORM: DiscountForm = {
  id: 0, productId: '', beginDate: '', endDate: '', discountPercentage: '',
};

export default function DiscountList() {
  const { data: discounts, loading: dLoading, error: dError, setData: setDiscounts } = useFetch<Discount[]>(getDiscounts);
  const { data: products,  loading: pLoading, error: pError } = useFetch<Product[]>(getProducts);
  const [actionError, setActionError] = useState('');
  const [modal, setModal] = useState<DiscountForm | null>(null);

  const loading = dLoading || pLoading;
  const error   = dError || pError || actionError;

  const openCreate = () => setModal({ ...EMPTY_FORM });
  const openEdit   = (d: Discount) => setModal({
    id: d.id, productId: String(d.productId),
    beginDate: toInput(d.beginDate), endDate: toInput(d.endDate),
    discountPercentage: String(d.discountPercentage),
  });

  const handleSave = async () => {
    if (!modal) return;
    setActionError('');
    const payload = {
      id: modal.id, productId: Number(modal.productId),
      beginDate: modal.beginDate, endDate: modal.endDate,
      discountPercentage: Number(modal.discountPercentage),
    };
    try {
      if (modal.id === 0) {
        const created = await createDiscount(payload);
        setDiscounts(prev => [...(prev ?? []), created]);
      } else {
        await updateDiscount(modal.id, payload);
        setDiscounts(prev => (prev ?? []).map(d => d.id === modal.id ? { ...d, ...payload, productName: products?.find(p => p.id === payload.productId)?.name ?? d.productName } : d));
      }
      setModal(null);
    } catch (e) {
      setActionError((e as Error).message);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this discount?')) return;
    try {
      await deleteDiscount(id);
      setDiscounts(prev => (prev ?? []).filter(d => d.id !== id));
    } catch (e) {
      setActionError((e as Error).message);
    }
  };

  const set = (key: keyof DiscountForm, val: string) =>
    setModal(prev => prev ? { ...prev, [key]: val } : null);

  if (loading) return <p className="loading">Loading discounts…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Discounts</h1>
        <button className="btn btn-primary" onClick={openCreate}>+ New Discount</button>
      </div>
      {error && <p className="error">{error}</p>}

      <div className="card">
        <table>
          <thead>
            <tr><th>Product</th><th>Begin Date</th><th>End Date</th><th>Discount %</th><th></th></tr>
          </thead>
          <tbody>
            {(discounts ?? []).map(d => (
              <tr key={d.id}>
                <td><strong>{d.productName}</strong></td>
                <td>{fmtDate(d.beginDate)}</td>
                <td>{fmtDate(d.endDate)}</td>
                <td><span className="badge badge-green">{d.discountPercentage}%</span></td>
                <td>
                  <button className="btn btn-secondary btn-sm" onClick={() => openEdit(d)}>Edit</button>
                  {' '}
                  <button className="btn btn-danger btn-sm" onClick={() => handleDelete(d.id)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {!discounts?.length && <p className="empty">No discounts found.</p>}
      </div>

      {modal && (
        <Modal
          title={modal.id === 0 ? 'New Discount' : 'Edit Discount'}
          onClose={() => setModal(null)}
          onSave={handleSave}
        >
          <div className="form-group">
            <label>Product</label>
            <select value={modal.productId} onChange={e => set('productId', e.target.value)}>
              <option value="">Select a product…</option>
              {(products ?? []).map(p => (
                <option key={p.id} value={p.id}>{p.name} — {p.manufacturer}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Begin Date</label>
            <input type="date" value={modal.beginDate} onChange={e => set('beginDate', e.target.value)} />
          </div>
          <div className="form-group">
            <label>End Date</label>
            <input type="date" value={modal.endDate} onChange={e => set('endDate', e.target.value)} />
          </div>
          <div className="form-group">
            <label>Discount %</label>
            <input type="number" min="0" max="100" step="0.01" value={modal.discountPercentage} onChange={e => set('discountPercentage', e.target.value)} />
          </div>
        </Modal>
      )}
    </div>
  );
}
