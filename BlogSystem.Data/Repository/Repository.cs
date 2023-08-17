using BlogSystem.Data.Context;
using BlogSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value of the entity to retrieve.</param>
        /// <returns>The retrieved entity or null if not found.</returns>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
    }

}
