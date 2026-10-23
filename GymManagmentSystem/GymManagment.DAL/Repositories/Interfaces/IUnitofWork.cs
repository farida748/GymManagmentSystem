using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IUnitofWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity> () where TEntity : BaseEntity, new();

        Task<int> SaveChangesAsync(CancellationToken ct=default);
    }
}
