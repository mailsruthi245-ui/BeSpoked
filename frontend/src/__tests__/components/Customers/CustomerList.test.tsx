import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import CustomerList from '../../../components/Customers/CustomerList';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const customers = [
  { id: 1, firstName: 'Tom', lastName: 'Walker', address: '1 River St', phone: '512-200-0001', startDate: '2022-01-05' },
  { id: 2, firstName: 'Lisa', lastName: 'Park', address: '9 Summit Dr', phone: '303-200-0002', startDate: '2022-03-14' },
];

beforeEach(() => {
  mockApi.getCustomers.mockResolvedValue(customers);
});

test('renders customer rows after load', async () => {
  render(<CustomerList />);
  await waitFor(() => expect(screen.getByText('Tom Walker')).toBeInTheDocument());
  expect(screen.getByText('Lisa Park')).toBeInTheDocument();
});

test('opens modal when New Customer is clicked', async () => {
  render(<CustomerList />);
  await waitFor(() => screen.getByText('Tom Walker'));
  await userEvent.click(screen.getByText('+ New Customer'));
  expect(screen.getByText('New Customer')).toBeInTheDocument();
});

test('shows empty message when no customers', async () => {
  mockApi.getCustomers.mockResolvedValue([]);
  render(<CustomerList />);
  await waitFor(() => expect(screen.getByText('No customers found.')).toBeInTheDocument());
});
