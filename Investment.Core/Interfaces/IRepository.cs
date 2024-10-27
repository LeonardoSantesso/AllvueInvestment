using System.Linq.Expressions;

namespace Investment.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}

