using Repair.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Repair.Entitys;
using Repair.Models;

namespace Repair.Services
{
    public class BaseService
    {
    }


    public class BaseService<T> where T : class
    {
        protected readonly IRepository<T, int> _repository;

        public BaseService(IRepository<T, int> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<T>> PageList(PageBase pageBase)
        {
            var list = await _repository.PageListAsync(pageBase);
            return list;
        }

        public async Task Insert(T t)
        {
            await _repository.InsertAsync(t);
        }

        public async Task Update(T t)
        {
            await _repository.UpdateAsync(t);
        }

        public async Task<T> FirstOrDefult(Expression<Func<T,bool>> func)
        {
            return await _repository.FirstOrDefultAsync(func);
        }
    }
}