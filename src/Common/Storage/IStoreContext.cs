using System.Collections.Generic;
using Common.Storage.DataEntities;

namespace Common.Storage
{
    public interface IStoreContext<TEntity> where TEntity : KeyedEntity
    {
        /// <summary>
        /// Returns a formatted string for a query with the specified parameters.
        /// </summary>
        /// <param name="propertyName">The name of the property to query</param>
        /// <param name="operation">One of the query operations <see cref="QueryOperator"/></param>
        /// <param name="givenValue">The value to compare to</param>
        string BuildQuery(string propertyName, QueryOperator operation, string givenValue);

        /// <summary>
        /// Gets the specific item from the table by row identifier
        /// </summary>
        /// <param name="identifier">The identifier of the item</param>
        TEntity Get(string identifier);

        /// <summary>
        /// Gets a search of <see cref="TEntity" />.
        /// </summary>
        IEnumerable<TEntity> Find(string queryString);

        /// <summary>
        /// Adds a new <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The entity</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds a range of new <see cref="TEntity" />
        /// </summary>
        /// <param name="entities">The entities</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an existing <see cref="TEntity" />
        /// </summary>
        /// <param name="identifier">The identifier of the item</param>
        void Delete(string identifier);

        /// <summary>
        /// Updates an existing <see cref="TEntity" />
        /// </summary>
        /// <param name="identifier">The identifier of the item</param>
        /// <param name="entity">The new version of the entity</param>
        TEntity Update(string identifier, TEntity entity);
    }
}
