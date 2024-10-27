using Investment.Core.Entities;
using Investment.Core.Interfaces;
using Investment.DAL.Data;
using Investment.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Investment.DAL.Repositories;

public class SaleRecordRepository : Repository<SaleRecord>, ISaleRecordRepository
{
    public SaleRecordRepository(InvestmentDbContext context) : base(context) {}

    public async Task<SaleRecord?> GetSaleRecordById(int id)
    {
        return await _context.Set<SaleRecord>()
            .Include(i => i.StockLotSales)
            .ThenInclude(i => i.StockLot)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<SaleRecord>> GetAllSaleRecordsAsync<TKey>(params Expression<Func<SaleRecord, object>>[] includes)
    {
        var query = _context.Set<SaleRecord>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
}