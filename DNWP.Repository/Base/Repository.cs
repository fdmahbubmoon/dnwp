using DNWP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DNWP.Repository.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    readonly DbSet<TEntity> _dbSet;
    public readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
        _dbContext = dbContext;
    }

    public virtual IQueryable<TEntity> Query() => _dbSet;

    public virtual async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate = null
        )
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate != null)
            return await query.AnyAsync(predicate);
        else
            return await query.AnyAsync();
    }

    public async Task<List<T>> RawSqlListAsync<T>(FormattableString sql) where T : class
        => await _dbContext.Set<T>().FromSqlInterpolated(sql).ToListAsync();

    public async Task<T> RawSqlFirstOrDefaultAsync<T>(FormattableString sql) where T : class
        => (await _dbContext.Set<T>().FromSqlInterpolated(sql).ToListAsync()).FirstOrDefault();

    public async Task RawSqlAsync(FormattableString sql)
        => await _dbContext.Database.ExecuteSqlInterpolatedAsync(sql);

    public async Task<List<T>> RawSqlListAsync<T>(string sql, params object[] parameters) where T : class
        => await _dbContext.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();

    public async Task<T> RawSqlFirstOrDefaultAsync<T>(string sql, params object[] parameters) where T : class
        => (await _dbContext.Set<T>().FromSqlRaw(sql, parameters).ToListAsync()).FirstOrDefault();

    public async Task RawSqlAsync(string sql, params object[] parameters)
        => await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

    public virtual async Task<TEntity> InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task InsertRangeAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }


    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateRangeAsync(List<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public virtual async Task DeleteRangeAsync(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }


    public virtual async Task<TEntity> FirstOrDefaultAsync(object id)
       => await _dbSet.FindAsync(id);

    public async Task<TEntity> GetEntityAsNoTrackingAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);
        _dbContext.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    
    public async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy
        )
    {
        return await orderBy(_dbContext.Set<TEntity>())
            .FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(
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

    public async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params Expression<Func<TEntity, object>>[] includes
        )
    {
        return await includes
        .Aggregate(
            orderBy(_dbContext.Set<TEntity>()).AsQueryable(),
            (current, include) => current.Include(include),
            c => c.FirstOrDefaultAsync(predicate)
        ).ConfigureAwait(false);
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate
        )
    {
        return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy
        )
    {
        return await orderBy(
            _dbContext.Set<TEntity>().Where(predicate)
        ).ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes
        )
    {
        return await includes.Aggregate(
            _dbContext.Set<TEntity>().AsQueryable(),
            (current, include) => current.Include(include),
            c => c.Where(predicate)
        ).ToListAsync()
        .ConfigureAwait(false);
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

    public virtual void Attach(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Added;
    }

    public virtual void Detach(TEntity entity)
    {
        if (_dbContext.Entry(entity).State != EntityState.Detached)
            _dbContext.Entry(entity).State = EntityState.Detached;
    }
}
