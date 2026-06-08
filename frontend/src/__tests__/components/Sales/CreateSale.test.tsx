import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import CreateSale from '../../../components/Sales/CreateSale';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const products = [
  { id: 1, name: 'Apex Racer', manufacturer: 'Trek', style: 'Road', purchasePrice: 800, salePrice: 1499, qtyOnHand: 5, commissionPercentage: 5 },
  { id: 2, name: 'Out Of Stock Bike', manufacturer: 'Giant', style: 'Road', purchasePrice: 500, salePrice: 999, qtyOnHand: 0, commissionPercentage: 4 },
];
const salespersons = [
  { id: 1, firstName: 'Alice', lastName: 'Chen', address: '1 St', phone: '555-0001', startDate: '2020-01-01', terminationDate: null, manager: 'Boss' },
  { id: 2, firstName: 'Jake', lastName: 'Novak', address: '2 St', phone: '555-0002', startDate: '2018-01-01', terminationDate: '2023-12-31', manager: 'Boss' },
];
const customers = [
  { id: 1, firstName: 'Tom', lastName: 'Walker', address: '1 St', phone: '555-0001', startDate: '2022-01-01' },
];

beforeEach(() => {
  mockApi.getProducts.mockResolvedValue(products);
  mockApi.getSalespersons.mockResolvedValue(salespersons);
  mockApi.getCustomers.mockResolvedValue(customers);
});

const renderComponent = () =>
  render(<MemoryRouter><CreateSale /></MemoryRouter>);

test('populates product dropdown after load', async () => {
  renderComponent();
  await waitFor(() => expect(screen.getByText(/Apex Racer/)).toBeInTheDocument());
});

test('excludes terminated salespersons from dropdown', async () => {
  renderComponent();
  await waitFor(() => expect(screen.getByText('Alice Chen')).toBeInTheDocument());
  expect(screen.queryByText('Jake Novak')).not.toBeInTheDocument();
});

test('shows validation error when submitting with empty fields', async () => {
  renderComponent();
  await userEvent.click(screen.getByText('Create Sale'));
  expect(screen.getByText('All fields are required.')).toBeInTheDocument();
});

test('shows API error on failed submission', async () => {
  mockApi.createSale.mockRejectedValue(new Error('Product is out of stock.'));
  renderComponent();

  await waitFor(() => screen.getByText(/Apex Racer/));
  const selects = screen.getAllByRole('combobox');
  await userEvent.selectOptions(selects[0], '1');
  await userEvent.selectOptions(selects[1], '1');
  await userEvent.selectOptions(selects[2], '1');
  await userEvent.click(screen.getByText('Create Sale'));

  await waitFor(() =>
    expect(screen.getByText('Product is out of stock.')).toBeInTheDocument()
  );
});
