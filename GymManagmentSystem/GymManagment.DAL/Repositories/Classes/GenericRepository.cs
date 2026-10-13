using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes
{
  public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<int> AddAsync(TEntity entity)
        {
          _dbSet.Add(entity);
           return await _dbContext.SaveChangesAsync();   
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool Tracking=false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = Tracking ? _dbSet : _dbSet.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) => 
            await _dbContext.Set<TEntity>().FindAsync(id);
        

        public async Task<int> UpdateAsync(TEntity entity)
        {
           _dbContext.Set<TEntity>().Update(entity);
            return await _dbContext.SaveChangesAsync();
        }

        
    }
}
