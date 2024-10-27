using System.Linq.Expressions;
using Investment.Core.Entities;
using Investment.Core.Interfaces;
using Investment.DAL.Data;
using Investment.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Investment.DAL.Repositories;

public class StockLotRepository : Repository<StockLot>, IStockLotRepository
{
    public StockLotRepository(InvestmentDbContext context) : base(context) { }

    public async Task<IEnumerable<StockLot>> GetAllStockLotsAsync<TKey>(Expression<Func<StockLot, TKey>> orderBy, 
        bool ascending = true, bool excludeEmptyLots = false, params Expression<Func<StockLot, object>>[] includes)
    {
        var query = _context.Set<StockLot>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        
        if (excludeEmptyLots)
            query = query.Where(lot => lot.Shares > 0);

        return await query.ToListAsync();
    }
}

