import React, { useState } from 'react';
import { getQuarterlyReport } from '../../services/api';
import { QuarterlyReportData } from '../../types';

const fmt$ = (v: number) => `$${Number(v).toFixed(2)}`;
const currentYear    = new Date().getFullYear();
const currentQuarter = Math.ceil((new Date().getMonth() + 1) / 3);

export default function QuarterlyReport() {
  const [year, setYear]       = useState(currentYear);
  const [quarter, setQuarter] = useState(currentQuarter);
  const [report, setReport]   = useState<QuarterlyReportData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState('');

  const load = async () => {
    setLoading(true); setError('');
    try {
      setReport(await getQuarterlyReport(year, quarter));
    } catch {
      setError('Failed to load report.');
    } finally {
      setLoading(false);
    }
  };

  const quarterLabel = (q: number) =>
    ['Q1 (Jan–Mar)', 'Q2 (Apr–Jun)', 'Q3 (Jul–Sep)', 'Q4 (Oct–Dec)'][q - 1];

  return (
    <div>
      <div className="page-header"><h1>Quarterly Commission Report</h1></div>
      {error && <p className="error">{error}</p>}

      <div className="filters" style={{ marginBottom: '1.5rem' }}>
        <div className="form-group">
          <label>Year</label>
          <input
            type="number" value={year} min={2020} max={2030}
            onChange={e => setYear(+e.target.value)} style={{ width: 100 }}
          />
        </div>
        <div className="form-group">
          <label>Quarter</label>
          <select value={quarter} onChange={e => setQuarter(+e.target.value)}>
            {[1, 2, 3, 4].map(q => <option key={q} value={q}>{quarterLabel(q)}</option>)}
          </select>
        </div>
        <button className="btn btn-primary" onClick={load}>Generate Report</button>
      </div>

      {loading && <p className="loading">Generating report…</p>}

      {report && !loading && (
        <div>
          <h2 style={{ marginBottom: '1rem', fontSize: '1.1rem', color: '#374151' }}>
            {quarterLabel(report.quarter)} {report.year} — Commission Summary
          </h2>

          {report.salespersons.length === 0
            ? <p className="empty">No sales recorded for this quarter.</p>
            : report.salespersons.map(sp => (
              <div key={sp.salespersonId} className="card" style={{ marginBottom: '1.25rem' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                  <div>
                    <span style={{ fontWeight: 700, fontSize: '1rem' }}>{sp.name}</span>
                    {sp.isTopSalesperson && (
                      <span className="badge badge-gold" style={{ marginLeft: '0.75rem' }}>🏆 Top Salesperson</span>
                    )}
                  </div>
                  <div style={{ textAlign: 'right', fontSize: '0.875rem' }}>
                    <div><strong>Sales:</strong> {sp.totalSales}</div>
                    <div><strong>Revenue:</strong> {fmt$(sp.totalRevenue)}</div>
                    <div><strong>Commission:</strong> {fmt$(sp.totalCommission)}</div>
                    {sp.quarterlyBonus > 0 && (
                      <div style={{ color: '#854d0e', fontWeight: 700 }}>
                        🎁 Quarterly Bonus: {fmt$(sp.quarterlyBonus)}
                      </div>
                    )}
                  </div>
                </div>

                <table>
                  <thead>
                    <tr><th>Date</th><th>Product</th><th>Price</th><th>Commission</th></tr>
                  </thead>
                  <tbody>
                    {sp.sales.map(s => (
                      <tr key={s.id}>
                        <td>{new Date(s.salesDate).toLocaleDateString()}</td>
                        <td>{s.product}</td>
                        <td>{fmt$(s.price)}</td>
                        <td>{fmt$(s.commission)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ))
          }
        </div>
      )}
    </div>
  );
}
