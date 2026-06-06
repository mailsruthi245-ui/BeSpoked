using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using BeSpoked.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.Infrastructure.Repositories;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(AppDbContext context) : base(context) { }

    // Override GetAllAsync to always eager-load navigation properties
    public new async Task<IEnumerable<Sale>> GetAllAsync() =>
        await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Salesperson)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.SalesDate)
            .ToListAsync();

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Salesperson)
            .Include(s => s.Customer)
            .AsQueryable();

        if (from.HasValue) query = query.Where(s => s.SalesDate >= from.Value);
        if (to.HasValue)   query = query.Where(s => s.SalesDate <= to.Value);

        return await query.OrderByDescending(s => s.SalesDate).ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetBySalespersonAsync(int salespersonId) =>
        await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Customer)
            .Where(s => s.SalespersonId == salespersonId)
            .OrderByDescending(s => s.SalesDate)
            .ToListAsync();

    public async Task<IEnumerable<Sale>> GetByQuarterAsync(int year, int quarter)
    {
        // Quarter 1 = Jan-Mar, Q2 = Apr-Jun, Q3 = Jul-Sep, Q4 = Oct-Dec
        var startMonth = (quarter - 1) * 3 + 1;
        var from = new DateTime(year, startMonth, 1);
        var to = from.AddMonths(3).AddDays(-1);

        return await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Salesperson)
            .Include(s => s.Customer)
            .Where(s => s.SalesDate >= from && s.SalesDate <= to)
            .ToListAsync();
    }
}
