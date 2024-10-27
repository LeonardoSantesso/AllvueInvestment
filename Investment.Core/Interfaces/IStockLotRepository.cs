using Investment.Core.Entities;
using System.Linq.Expressions;

namespace Investment.Core.Interfaces;

public interface IStockLotRepository : IRepository<StockLot>
{
    Task<IEnumerable<StockLot>> GetAllStockLotsAsync<TKey>(Expression<Func<StockLot, TKey>> orderBy, 
        bool ascending = true, bool excludeEmptyLots = false, params Expression<Func<StockLot, object>>[] includes);
}

