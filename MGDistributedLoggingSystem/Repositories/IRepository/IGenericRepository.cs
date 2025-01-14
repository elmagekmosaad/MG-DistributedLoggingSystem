using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MGDistributedLoggingSystem.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
         IQueryable<T> Table { get; }
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        Task AddAsync(T entity);


        /// <summary>
        /// Asynchronously retrieves all entities from the repository.
        /// </summary>
        /// <returns>A collection of all entities in the repository.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The retrieved entity, or null if not found.</returns>
        Task<T> GetById(int id);
    }

}
