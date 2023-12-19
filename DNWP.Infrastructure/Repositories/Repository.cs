using System.Linq.Expressions;
using DNWP.Application.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DNWP.Infrastructure.Repositories;

public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    readonly DbSet<TEntity> _dbSet;
    public readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
        _dbContext = dbContext;
    }


    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task InsertRangeAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<TEntity> FirstOrDefaultAsync(object id)
       => await _dbSet.FindAsync(id);

    public async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes
        )
    {
        return await includes
        .Aggregate(
            _dbContext.Set<TEntity>().AsQueryable(),
            (current, include) => current.Include(include),
            c => c.AsNoTracking().FirstOrDefaultAsync(predicate)
        ).ConfigureAwait(false);
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        params Expression<Func<TEntity, object>>[] includes
        )
    {
        return await orderBy(
            includes.Aggregate(
            _dbContext.Set<TEntity>().AsQueryable(),
            (current, include) => current.Include(include),
            c => c.Where(predicate)
        )).ToListAsync()
        .ConfigureAwait(false);
    }
}
