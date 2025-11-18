using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.EntityFrameworkCore.Repositories
{
    public class CorarlRepository<TEntity, TPrimaryKey> :
        CorarlERPRepositoryBase<TEntity, TPrimaryKey>, ICorarlRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public CorarlRepository(IDbContextProvider<CorarlERPDbContext> dbContextProvider) :
            base(dbContextProvider)
        {
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> list)
        {
            await this.Context.BulkInsertAsync(list);
        }

        public async Task BulkInsertAsync(TEntity entity)
        {
            await this.Context.BulkInsertAsync( new List<TEntity> { entity });
        }

        public async Task BulkUpdateAsync(IEnumerable<TEntity> list)
        {
            await this.Context.BulkUpdateAsync(list);
        }

        public async Task BulkUpdateAsync(TEntity entity)
        {
            await this.Context.BulkUpdateAsync(new List<TEntity> { entity });
        }

        public async Task BulkDeleteAsync(IEnumerable<TEntity> list)
        {
            await this.Context.BulkDeleteAsync(list);
        }

        public async Task BulkDeleteAsync(TEntity entity)
        {
            await this.Context.BulkDeleteAsync(new List<TEntity> { entity });
        }

        public async Task BulkMergeAsync(IEnumerable<TEntity> list)
        {
            await this.Context.BulkMergeAsync(list);
        }
    }


    public class CorarlRepository<TEntity> : CorarlRepository<TEntity, int>, ICorarlRepository<TEntity>
       where TEntity : class, IEntity<int>
    {
        public CorarlRepository(IDbContextProvider<CorarlERPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)!!!
    }
}
