using IoTMetrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Interfaces
{
    public interface IRepository<T> where T: IDBEntity
    {
        IEnumerable<T> GetAll();
        IQueryable<T> GetAllEntities();
        Task<ICollection<T>> GetAllAsync();
        T GetById(int id);
        Task<T> GetByIdAsync(int? id);
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(int id);
        
    }
}
