using Investment.Core.Entities;
using System.Linq.Expressions;

namespace Investment.Core.Interfaces;

public interface ISaleRecordRepository : IRepository<SaleRecord>
{
    Task<SaleRecord?> GetSaleRecordById(int id);
    Task<IEnumerable<SaleRecord>> GetAllSaleRecordsAsync<TKey>(params Expression<Func<SaleRecord, object>>[] includes);
}

