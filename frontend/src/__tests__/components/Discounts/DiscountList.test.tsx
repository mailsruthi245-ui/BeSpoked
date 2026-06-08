import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import DiscountList from '../../../components/Discounts/DiscountList';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const discounts = [
  { id: 1, productId: 1, productName: 'Apex Racer', beginDate: '2024-01-01', endDate: '2024-01-31', discountPercentage: 10 },
];
const products = [
  { id: 1, name: 'Apex Racer', manufacturer: 'Trek', style: 'Road', purchasePrice: 800, salePrice: 1499, qtyOnHand: 10, commissionPercentage: 5 },
];

beforeEach(() => {
  mockApi.getDiscounts.mockResolvedValue(discounts);
  mockApi.getProducts.mockResolvedValue(products);
});

test('renders discount rows after load', async () => {
  render(<DiscountList />);
  await waitFor(() => expect(screen.getByText('Apex Racer')).toBeInTheDocument());
  expect(screen.getByText('10%')).toBeInTheDocument();
});

test('opens modal when New Discount is clicked', async () => {
  render(<DiscountList />);
  await waitFor(() => screen.getByText('Apex Racer'));
  await userEvent.click(screen.getByText('+ New Discount'));
  expect(screen.getByText('New Discount')).toBeInTheDocument();
});

test('opens edit modal when Edit is clicked', async () => {
  render(<DiscountList />);
  await waitFor(() => screen.getByText('Apex Racer'));
  await userEvent.click(screen.getByText('Edit'));
  expect(screen.getByText('Edit Discount')).toBeInTheDocument();
});

test('removes row after delete is confirmed', async () => {
  mockApi.deleteDiscount.mockResolvedValue(null);
  jest.spyOn(window, 'confirm').mockReturnValue(true);

  render(<DiscountList />);
  await waitFor(() => screen.getByText('Apex Racer'));
  await userEvent.click(screen.getByText('Delete'));

  await waitFor(() => expect(screen.queryByText('Apex Racer')).not.toBeInTheDocument());
});
