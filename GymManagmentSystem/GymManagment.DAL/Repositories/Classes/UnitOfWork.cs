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
    public class UnitOfWork : IUnitofWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories =[];

        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
           var typename= typeof(TEntity).Name;
            if (_repositories.TryGetValue(typename, out object? value))
                return (GenericRepository<TEntity>) value;
            else
            {
              var repo=new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(typename, repo);
                return repo;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);
       
    }
}
