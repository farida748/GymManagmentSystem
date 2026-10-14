using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IGenericRepository <TEntity> where TEntity : BaseEntity ,new()
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(bool Tracking=false,CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken ct = default, params Expression<Func<TEntity, object>>[] includes);
        Task<int> AddAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
    }
}
