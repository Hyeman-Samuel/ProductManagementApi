using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Persistence;
using ProductManagementApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        public virtual async void Create(T entity)
        {
          await _context.Set<T>().AddAsync(entity);
          _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
             _context.SaveChanges();
        }

        public async void Edit(T entity)
        {
             _context.Set<T>().Update(entity);
             _context.SaveChanges();
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).AsNoTracking();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T GetOne(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().First(predicate);
        }
    }
}
