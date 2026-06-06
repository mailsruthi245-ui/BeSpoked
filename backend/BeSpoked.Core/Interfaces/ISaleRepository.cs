using BeSpoked.Core.Entities;

namespace BeSpoked.Core.Interfaces;

public interface ISaleRepository : IRepository<Sale>
{
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime? from, DateTime? to);
    Task<IEnumerable<Sale>> GetBySalespersonAsync(int salespersonId);

    // For quarterly commission report: returns all sales in a given quarter
    Task<IEnumerable<Sale>> GetByQuarterAsync(int year, int quarter);
}
