using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using BeSpoked.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.Infrastructure.Repositories;

public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Discount>> GetAllWithProductAsync() =>
        await _context.Discounts
            .Include(d => d.Product)
            .OrderBy(d => d.Product.Name)
            .ThenBy(d => d.BeginDate)
            .ToListAsync();

    public async Task<Discount?> GetByIdWithProductAsync(int id) =>
        await _context.Discounts
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Discount?> GetActiveForProductAsync(int productId, DateTime date) =>
        await _context.Discounts
            .FirstOrDefaultAsync(d => d.ProductId == productId
                                   && d.BeginDate <= date
                                   && d.EndDate >= date);
}
