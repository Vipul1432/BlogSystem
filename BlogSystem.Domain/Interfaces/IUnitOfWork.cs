using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Asynchronously saves changes made to the underlying database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation and the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Asynchronously commits the changes made during the current transaction to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitAsync();

        /// <summary>
        /// Rolls back the changes made during the current transaction.
        /// </summary>
        void Rollback();
    }

}
