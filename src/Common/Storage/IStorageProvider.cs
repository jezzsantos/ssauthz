using System.Collections.Generic;
using Common.Storage.DataEntities;

namespace Common.Storage
{
    /// <summary>
    ///     Defines a provider of storage
    /// </summary>
    public interface IStorageProvider<TDto> where TDto : class
    {
        /// <summary>
        ///     Returns a formatted string for a query with the specified parameters.
        /// </summary>
        /// <param name="propertyName">The name of the property to query</param>
        /// <param name="operation">One of the query operations <see cref="QueryOperator" /></param>
        /// <param name="givenValue">The value to compare to</param>
        string BuildQuery(string propertyName, QueryOperator operation, string givenValue);

        /// <summary>
        ///     Gets the <see cref="TDto" /> with the specified identifer
        /// </summary>
        /// <param name="identifier">The identifier of the contract</param>
        TDto Get(string identifier);

        /// <summary>
        ///     Finds the <see cref="TDto" /> for the specified query.
        /// </summary>
        /// <param name="query">The query to filter</param>
        IEnumerable<TDto> Find(string query);

        /// <summary>
        ///     Adds a new <see cref="TDto" /> to the store.
        /// </summary>
        /// <param name="dto">The  <see cref="TDto" /> to store</param>
        /// <returns>The identifier of the newly created entity</returns>
        string Add(TDto dto);

        /// <summary>
        ///     Updates the <see cref="TDto" /> in the store.
        /// </summary>
        /// <param name="identifier">The identifier of the <see cref="TDto" /></param>
        /// <param name="dto">The  <see cref="TDto" /> to update</param>
        TDto Update(string identifier, TDto dto);

        /// <summary>
        ///     Deletes the <see cref="TDto" /> from the store.
        /// </summary>
        /// <param name="identifier"></param>
        void Delete(string identifier);
    }
}