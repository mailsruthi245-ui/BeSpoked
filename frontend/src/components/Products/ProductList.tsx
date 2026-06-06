import React, { useState } from 'react';
import { getProducts, updateProduct } from '../../services/api';
import { Product } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import Modal from '../common/Modal';

const fmt$ = (v: number) => `$${Number(v).toFixed(2)}`;

interface FieldDef { key: keyof Product; label: string; type?: string; }

const fields: FieldDef[] = [
  { key: 'name',                 label: 'Name' },
  { key: 'manufacturer',         label: 'Manufacturer' },
  { key: 'style',                label: 'Style' },
  { key: 'purchasePrice',        label: 'Purchase Price',  type: 'number' },
  { key: 'salePrice',            label: 'Sale Price',      type: 'number' },
  { key: 'qtyOnHand',            label: 'Qty On Hand',     type: 'number' },
  { key: 'commissionPercentage', label: 'Commission %',    type: 'number' },
];

export default function ProductList() {
  const { data: products, loading, error, setData: setProducts, setError } = useFetch<Product[]>(getProducts);
  const [editing, setEditing] = useState<Product | null>(null);

  const handleSave = async () => {
    if (!editing) return;
    try {
      await updateProduct(editing.id, editing);
      setProducts(prev => (prev ?? []).map(p => p.id === editing.id ? editing : p));
      setEditing(null);
    } catch (e) {
      setError((e as Error).message);
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
            {(products ?? []).map(p => (
              <tr key={p.id}>
                <td><strong>{p.name}</strong></td>
                <td>{p.manufacturer}</td>
                <td>{p.style}</td>
                <td>{fmt$(p.purchasePrice)}</td>
                <td>{fmt$(p.salePrice)}</td>
                <td>
                  <span className={`badge ${p.qtyOnHand > 5 ? 'badge-green' : 'badge-orange'}`}>
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
        {!products?.length && <p className="empty">No products found.</p>}
      </div>

      {editing && (
        <Modal title="Edit Product" onClose={() => setEditing(null)} onSave={handleSave}>
          {fields.map(({ key, label, type = 'text' }) => (
            <div className="form-group" key={key}>
              <label>{label}</label>
              <input
                type={type}
                value={editing[key] as string | number}
                onChange={e => setEditing({
                  ...editing,
                  [key]: type === 'number' ? +e.target.value : e.target.value,
                })}
              />
            </div>
          ))}
        </Modal>
      )}
    </div>
  );
}
