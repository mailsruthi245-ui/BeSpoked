import React from 'react';
import { BrowserRouter, Routes, Route, NavLink } from 'react-router-dom';
import SalespersonList from './components/Salespersons/SalespersonList';
import ProductList from './components/Products/ProductList';
import CustomerList from './components/Customers/CustomerList';
import SaleList from './components/Sales/SaleList';
import CreateSale from './components/Sales/CreateSale';
import QuarterlyReport from './components/Reports/QuarterlyReport';
import './App.css';

export default function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <header className="header">
          <div className="brand">🚲 BeSpoked Bikes</div>
          <nav className="nav">
            <NavLink to="/salespersons">Salespersons</NavLink>
            <NavLink to="/products">Products</NavLink>
            <NavLink to="/customers">Customers</NavLink>
            <NavLink to="/sales">Sales</NavLink>
            <NavLink to="/sales/new" className="nav-cta">+ New Sale</NavLink>
            <NavLink to="/report">Q Report</NavLink>
          </nav>
        </header>

        <main className="main">
          <Routes>
            <Route path="/"               element={<SalespersonList />} />
            <Route path="/salespersons"   element={<SalespersonList />} />
            <Route path="/products"       element={<ProductList />} />
            <Route path="/customers"      element={<CustomerList />} />
            <Route path="/sales"          element={<SaleList />} />
            <Route path="/sales/new"      element={<CreateSale />} />
            <Route path="/report"         element={<QuarterlyReport />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
