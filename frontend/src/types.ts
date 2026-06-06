export interface Salesperson {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  phone: string;
  startDate: string;
  terminationDate: string | null;
  manager: string;
}

export interface Product {
  id: number;
  name: string;
  manufacturer: string;
  style: string;
  purchasePrice: number;
  salePrice: number;
  qtyOnHand: number;
  commissionPercentage: number;
}

export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  phone: string;
  startDate: string;
}

export interface Sale {
  id: number;
  salesDate: string;
  product: string;
  customer: string;
  salesperson: string;
  price: number;
  discountApplied: number;
  commission: number;
  commissionPercentage: number;
}

export interface SalespersonReport {
  salespersonId: number;
  name: string;
  totalSales: number;
  totalRevenue: number;
  totalCommission: number;
  quarterlyBonus: number;
  isTopSalesperson: boolean;
  sales: Sale[];
}

export interface QuarterlyReportData {
  year: number;
  quarter: number;
  salespersons: SalespersonReport[];
}

export interface Discount {
  id: number;
  productId: number;
  productName: string;
  beginDate: string;
  endDate: string;
  discountPercentage: number;
}

export type DiscountPayload = Omit<Discount, 'productName'>;

export interface CreateSaleRequest {
  productId: number;
  salespersonId: number;
  customerId: number;
  salesDate: string;
}
