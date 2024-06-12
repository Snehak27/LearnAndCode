using System;
using System.Linq.Expressions;

namespace CafeteriaServer.Repositories.Generic
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> Search(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> predicate);

    }
}
