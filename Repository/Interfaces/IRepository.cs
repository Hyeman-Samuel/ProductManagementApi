using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        T GetOne(Expression<Func<T, bool>> predicate);
        void Create(T entity);
        void Edit(T entity);
        void Delete(T entity);
        Task<T> GetById(Guid id);
    }
}
