import { getProducts, createProduct, updateProduct } from '../../services/api';
import { Product } from '../../types';
import { useFetch } from '../../hooks/useFetch';
import { useEditState } from '../../hooks/useEditState';
import Modal from '../common/Modal';
import FormField from '../common/FormField';

const fmt$ = (v: number) => `$${Number(v).toFixed(2)}`;

const BLANK: Product = {
  id: 0, name: '', manufacturer: '', style: '',
  purchasePrice: 0, salePrice: 0, qtyOnHand: 0, commissionPercentage: 0,
};

export default function ProductList() {
  const { data: products, loading, error, setData: setProducts, setError } = useFetch<Product[]>(getProducts);
  const { editing, setEditing, set } = useEditState<Product>();

  const handleSave = async () => {
    if (!editing) return;
    try {
      if (editing.id === 0) {
        const { id: _id, ...payload } = editing;
        const created = await createProduct(payload);
        setProducts(prev => [...(prev ?? []), created]);
      } else {
        await updateProduct(editing.id, editing);
        setProducts(prev => (prev ?? []).map(p => p.id === editing.id ? editing : p));
      }
      setEditing(null);
    } catch (e) {
      setError((e as Error).message);
    }
  };

  if (loading) return <p className="loading">Loading products…</p>;

  return (
    <div>
      <div className="page-header">
        <h1>Products</h1>
        <button className="btn btn-primary" onClick={() => setEditing({ ...BLANK })}>+ New Product</button>
      </div>
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
        <Modal
          title={editing.id === 0 ? 'New Product' : 'Edit Product'}
          onClose={() => setEditing(null)}
          onSave={handleSave}
        >
          <FormField label="Name"         value={editing.name}                 onChange={val => set('name', val)} />
          <FormField label="Manufacturer" value={editing.manufacturer}         onChange={val => set('manufacturer', val)} />
          <FormField label="Style"        value={editing.style}                onChange={val => set('style', val)} />
          <FormField label="Purchase Price"      type="number" value={editing.purchasePrice}        onChange={val => set('purchasePrice', +val)} />
          <FormField label="Sale Price"          type="number" value={editing.salePrice}             onChange={val => set('salePrice', +val)} />
          <FormField label="Qty On Hand"         type="number" value={editing.qtyOnHand}             onChange={val => set('qtyOnHand', +val)} />
          <FormField label="Commission %"        type="number" value={editing.commissionPercentage}  onChange={val => set('commissionPercentage', +val)} />
        </Modal>
      )}
    </div>
  );
}
