using BeSpoked.Core.Entities;

namespace BeSpoked.Core.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    Task<IEnumerable<Discount>> GetAllWithProductAsync();
    Task<Discount?> GetByIdWithProductAsync(int id);
    Task<Discount?> GetActiveForProductAsync(int productId, DateTime date);
}
