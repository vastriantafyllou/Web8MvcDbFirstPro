using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly Mvc8DbProContext context;
    protected readonly DbSet<T> dbSet;

    public BaseRepository(Mvc8DbProContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();  // dynamically retrieves DbSet
    }

    public virtual async Task AddAsync(T entity) => 
        await dbSet.AddAsync(entity);
    
    public virtual async Task AddRangeAsync(IEnumerable<T> entities) => 
        await dbSet.AddRangeAsync(entities);
    
    public virtual Task UpdateAsync(T entity) 
    {
        dbSet.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
    
    public virtual async Task<bool> DeleteAsync(int id)
    {
        T? existingEntity = await dbSet.FindAsync(id);
        if (existingEntity is null) return false;
        
        dbSet.Remove(existingEntity);
        return true;
    }

    public virtual async Task<T?> GetAsync(int id) =>
        await dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => 
        await dbSet.ToListAsync();
    
    public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();
    
}