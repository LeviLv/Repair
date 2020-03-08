using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Repair.Entitys;
using Repair.Models;

namespace Repair.EntityFramework
{
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class
    {
    }

    public interface IRepository<TEntity, Tkey> where TEntity : class
    {
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> FirstOrDefultAsync(Expression<Func<TEntity, bool>> expression);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression);

        Task<List<TEntity>> GetAllAsync();

        Task<IQueryable<TEntity>> LoadListAsync(Expression<Func<TEntity, bool>> expression);

        Task<QueryResult<TEntity>> PageListAsync(PageBase pageBase);


        Task<QueryResult<TEntity>> PageListAsync(Func<TEntity, bool> where, Func<TEntity, int> orderBy,
            PageBase pageBase);


        Task<QueryResult<T>> PageListAsync<T>(Func<TEntity, bool> where, Func<TEntity, int> orderBy,
            PageBase pageBase) where T : class;

        Task<TEntity> InsertAsync(TEntity entity);

        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);

        Task<bool> DeleteAsync(Tkey key);

        Task<bool> UpdateAsync(TEntity entity);

        Task<TEntity> UpdateAndGetModelAsync(TEntity entity);
    }
}