using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP
{
    public interface ICorarlRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> 
        where TEntity : class, IEntity<TPrimaryKey>
    {
        Task BulkInsertAsync(IEnumerable<TEntity> list);
        Task BulkInsertAsync(TEntity entity);

        Task BulkUpdateAsync(IEnumerable<TEntity> list);
        Task BulkUpdateAsync(TEntity entity);

        Task BulkDeleteAsync(IEnumerable<TEntity> list);
        Task BulkDeleteAsync(TEntity entity);

        Task BulkMergeAsync(IEnumerable<TEntity> list);

    }

    public interface ICorarlRepository<TEntity> : ICorarlRepository<TEntity, int>
        where TEntity : class, IEntity<int>
    {
    }
}
