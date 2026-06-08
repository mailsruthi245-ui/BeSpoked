import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import QuarterlyReport from '../../../components/Reports/QuarterlyReport';
import * as api from '../../../services/api';

jest.mock('../../../services/api');
const mockApi = api as jest.Mocked<typeof api>;

const reportWithData = {
  year: 2024,
  quarter: 1,
  salespersons: [
    {
      salespersonId: 1, name: 'Alice Chen', totalSales: 3,
      totalRevenue: 4500, totalCommission: 225, quarterlyBonus: 22.5,
      isTopSalesperson: true, sales: [],
    },
  ],
};

const emptyReport = { year: 2024, quarter: 1, salespersons: [] };

test('renders year and quarter controls', () => {
  render(<QuarterlyReport />);
  expect(screen.getByText('Generate Report')).toBeInTheDocument();
  expect(screen.getByRole('combobox')).toBeInTheDocument();
});

test('shows salesperson results after generating report', async () => {
  mockApi.getQuarterlyReport.mockResolvedValue(reportWithData);
  render(<QuarterlyReport />);
  await userEvent.click(screen.getByText('Generate Report'));
  await waitFor(() => expect(screen.getByText('Alice Chen')).toBeInTheDocument());
  expect(screen.getByText('🏆 Top Salesperson')).toBeInTheDocument();
});

test('shows empty message when no sales in quarter', async () => {
  mockApi.getQuarterlyReport.mockResolvedValue(emptyReport);
  render(<QuarterlyReport />);
  await userEvent.click(screen.getByText('Generate Report'));
  await waitFor(() =>
    expect(screen.getByText('No sales recorded for this quarter.')).toBeInTheDocument()
  );
});
