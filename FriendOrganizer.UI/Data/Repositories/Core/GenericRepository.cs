using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories.Core
{
    /**
     * The implementation of IGenericRepository with Generic initialize parameters
     *  1- TContext as DbContext from entity framework to represent the DbContext inside {DataAccess} project [MVVM]
     *      Because of : all database contexts inherited from this DbContext class
     *  2- TEntity : as class to represent the Model :
     *      Because of all our models of type class
     */
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        /**
         * protected constructor mean this constructor/class only can use as a base class for it'schild
         * not with new keyword
         */
        protected GenericRepository(TContext context)
        {
            this.Context = context;
        }

        /**
         * Protected : to allow All childs to using it
         * readonly  : to prevent any child from replacing it
         */
        protected readonly TContext Context;

        /**
         * All Next methods is very simple and straightforward
         */

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public void Add(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public void Remove(TEntity model)
        {
            Context.Set<TEntity>().Remove(model);
        }
    }
}
