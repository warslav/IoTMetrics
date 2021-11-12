using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class Repository<T> : IRepository<T> where T : class, IDBEntity
    {
        private readonly SensorContext _context;
        public Repository(SensorContext context)
        {
            _context = context;
        }
        public virtual void Add(T entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Add(entity);
            }
        }

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                if (entity != null)
                {
                    await _context.Set<T>().AddAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}", ex);
            }
        }

        public virtual void Delete(int id)
        {
            T entity = _context.Set<T>().Find(id);

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            _context.Set<T>().Remove(entity);

        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}", ex);
            }
        }

        public IQueryable<T> GetAllEntities()
        {
            return _context.Set<T>();
        }

        public virtual T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} entity must not be null");
            }

            try
            {
                if (entity != null)
                {
                    _context.Set<T>().Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} state could not be updated: {ex.Message}", ex);
            }
        }

    }
}
