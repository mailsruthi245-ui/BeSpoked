import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import SalespersonList from '../../../components/Salespersons/SalespersonList';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const salespersons = [
  { id: 1, firstName: 'Alice', lastName: 'Chen', address: '1 St', phone: '555-0001', startDate: '2020-01-01', terminationDate: null, manager: 'Boss' },
  { id: 2, firstName: 'Jake', lastName: 'Novak', address: '2 St', phone: '555-0002', startDate: '2018-01-01', terminationDate: '2023-12-31', manager: 'Boss' },
];

beforeEach(() => {
  mockApi.getSalespersons.mockResolvedValue(salespersons);
});

test('renders salesperson rows after load', async () => {
  render(<SalespersonList />);
  await waitFor(() => expect(screen.getByText('Alice Chen')).toBeInTheDocument());
  expect(screen.getByText('Jake Novak')).toBeInTheDocument();
});

test('shows Active badge for active salesperson', async () => {
  render(<SalespersonList />);
  await waitFor(() => screen.getByText('Alice Chen'));
  expect(screen.getByText('Active')).toBeInTheDocument();
});

test('shows Terminated badge for terminated salesperson', async () => {
  render(<SalespersonList />);
  await waitFor(() => screen.getByText('Jake Novak'));
  expect(screen.getByText(/Terminated/)).toBeInTheDocument();
});

test('opens modal when New Salesperson is clicked', async () => {
  render(<SalespersonList />);
  await waitFor(() => screen.getByText('Alice Chen'));
  await userEvent.click(screen.getByText('+ New Salesperson'));
  expect(screen.getByText('New Salesperson')).toBeInTheDocument();
});
