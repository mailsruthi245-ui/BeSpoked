import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ProductList from '../../../components/Products/ProductList';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const products = [
  { id: 1, name: 'Apex Racer', manufacturer: 'Trek', style: 'Road', purchasePrice: 800, salePrice: 1499, qtyOnHand: 10, commissionPercentage: 5 },
  { id: 2, name: 'UrbanGlide 7', manufacturer: 'Cannondale', style: 'Hybrid', purchasePrice: 500, salePrice: 899, qtyOnHand: 3, commissionPercentage: 4 },
];

beforeEach(() => {
  mockApi.getProducts.mockResolvedValue(products);
});

test('renders product rows after load', async () => {
  render(<ProductList />);
  await waitFor(() => expect(screen.getByText('Apex Racer')).toBeInTheDocument());
  expect(screen.getByText('UrbanGlide 7')).toBeInTheDocument();
});

test('shows low stock badge for qty <= 5', async () => {
  render(<ProductList />);
  await waitFor(() => screen.getByText('UrbanGlide 7'));
  const badges = screen.getAllByText(/^[0-9]+$/);
  expect(badges[1]).toHaveClass('badge-orange');
});

test('opens modal when New Product is clicked', async () => {
  render(<ProductList />);
  await waitFor(() => screen.getByText('Apex Racer'));
  await userEvent.click(screen.getByText('+ New Product'));
  expect(screen.getByText('New Product')).toBeInTheDocument();
});

test('opens edit modal when Edit is clicked', async () => {
  render(<ProductList />);
  await waitFor(() => screen.getByText('Apex Racer'));
  await userEvent.click(screen.getAllByText('Edit')[0]);
  expect(screen.getByText('Edit Product')).toBeInTheDocument();
});
