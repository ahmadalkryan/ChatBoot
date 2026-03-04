using Application.IRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class Repository<T> : IAppRepository<T> where T : class
    {
        private  DBContext _context;
        private  DbSet<T> _entities;

        public Repository(DBContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            var result = await _entities.AddAsync(entity);  
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            //var result = await _entities.ToListAsync();
            //return result;
            IQueryable<T> set = _entities;
            var navs = _context.Model.FindEntityType(typeof(T)).GetNavigations();

            if(navs is not null)
            {
                foreach (var nav in navs)
                {
                    set = set.Include(nav.Name);
                }

            }
            return set;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            //var entity = await _entities.FindAsync(id);
            //return entity;
            var navs = _context.Model.FindEntityType(typeof(T)).GetNavigations();
            var set = _entities.AsQueryable();

            if (navs is not null)
            {
                foreach (var nav in navs)
                {
                    set = set.Include(nav.Name);
                }
            }
            return await set.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public Task UpdateAsync(T entity)
        {
            var result =  _entities.Update(entity);
            return _context.SaveChangesAsync();
        }
    }
}
