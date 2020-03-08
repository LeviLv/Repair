using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Repair.AutoMapper;
using Repair.Entitys;
using Repair.Models;

namespace Repair.EntityFramework
{
    public class Repository<TEntity> : Repository<TEntity, int> where TEntity : class
    {
        public Repository(DBContext dbContext)
            : base(dbContext)
        {
        }
    }

    public class Repository<TEntity, Tkey> : IRepository<TEntity, Tkey> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbset;
        public DBContext _dbContext { get; } = null;


        public Repository(DBContext dbContext)
        {
            _dbContext = dbContext;
            _dbset = _dbContext.Set<TEntity>();
        }

        public Task<int> Count(Expression<Func<TEntity, bool>> expression)
        {
            return _dbset.CountAsync(expression);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _dbset.CountAsync(expression);
        }

        public Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            return null;
        }

        public Task<bool> DeleteAsync(Tkey key)
        {
            return null;
        }

        public Task<TEntity> FirstOrDefultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _dbset.FirstOrDefaultAsync(expression);
        }

        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _dbset.Where(expression).ToListAsync();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }

        public Task<QueryResult<TEntity>> PageListAsync(PageBase pageBase)
        {
            var list = _dbset.Skip((pageBase.PageIndex - 1) * pageBase.PageSize)
                .Take(pageBase.PageSize)
                .ToList();
            var count = _dbset.Count();
            
            return Task.FromResult(new QueryResult<TEntity>() { List = list, Total = count });
        }

        public Task<QueryResult<TEntity>> PageListAsync(Func<TEntity, bool> funcWhere, Func<TEntity, int> funcOrderBy,
            PageBase pageBase)
        {
            var list = _dbset.WhereIfNotNull(funcWhere)
                .Skip((pageBase.PageIndex - 1) * pageBase.PageSize)
                .Take(pageBase.PageSize)
                .OrderByDescending(funcOrderBy)
                .ToList();
            var count = _dbset.WhereIfNotNull(funcWhere)
                .Count();
            return Task.FromResult(new QueryResult<TEntity>() { List = list, Total = count });
        }

        public Task<QueryResult<T>> PageListAsync<T>(Func<TEntity, bool> funcWhere, Func<TEntity, int> funcOrderBy,
            PageBase pageBase) where T : class
        {
            var list = _dbset.WhereIfNotNull(funcWhere)
                .Skip((pageBase.PageIndex - 1) * pageBase.PageSize)
                .Take(pageBase.PageSize)
                .OrderByDescending(funcOrderBy)
                .ToList()
                .MapTo<List<T>>();
            var count = _dbset.WhereIfNotNull(funcWhere)
                .Count();
            return Task.FromResult(new QueryResult<T>() { List = list, Total = count });
        }
        
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var en = await _dbset.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return en.Entity;
        }

        public Task<IQueryable<TEntity>> LoadListAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(_dbset.WhereIfNotNull(expression));
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            _dbset.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TEntity> UpdateAndGetModelAsync(TEntity entity)
        {
            foreach (var p in entity.GetType().GetProperties())
            {
                if (p.Name == "ModifyTime")
                {
                    p.SetValue(entity, DateTime.Now);
                }
            }

            var en = _dbset.Update(entity);
            await _dbContext.SaveChangesAsync();
            return en.Entity;
        }
    }
}