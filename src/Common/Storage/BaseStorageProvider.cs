using System.Collections.Generic;
using System.Linq;
using Common.Storage.DataEntities;

namespace Common.Storage
{
    /// <summary>
    /// An Azure table storage provider.
    /// </summary>
    public abstract class BaseStorageProvider<TDto, TEntity> : IStorageProvider<TDto>
        where TDto : class
        where TEntity : KeyedEntity
    {
        /// <summary>
        /// Gets the <see cref="IStoreContext{TEntity}" />.
        /// </summary>
        public IStoreContext<TEntity> StoreContext { get; set; }

        /// <summary>
        /// Returns a formatted string for a query with the specified parameters.
        /// </summary>
        /// <param name="propertyName">The name of the property to query</param>
        /// <param name="operation">One of the query operations <see cref="QueryOperator"/></param>
        /// <param name="givenValue">The value to compare to</param>
        /// <returns></returns>
        public string BuildQuery(string propertyName, QueryOperator operation, string givenValue)
        {
            return this.StoreContext.BuildQuery(propertyName, operation, givenValue);
        }

        /// <summary>
        /// Converts the <see cref="TEntity"/> to a DTO of <see cref="TDto"/>
        /// </summary>
        protected abstract TDto ToDto(TEntity entity);

        /// <summary>
        /// Converts the DTO <see cref="TDto"/> to a entity of <see cref="TEntity"/>
        /// </summary>
        protected abstract TEntity FromDto(TDto dto);

        /// <summary>
        /// Creates a new identity for the new <see cref="TEntity"/>
        /// </summary>
        protected abstract string CreateIdentity(TEntity entity);

        /// <summary>
        /// Gets the <see cref="TDto" /> with the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the <see cref="TDto" /></param>
        public TDto Get(string identifier)
        {
            Guard.NotNullOrEmpty(() => identifier, identifier);

            var entity = this.StoreContext.Get(identifier);
            return entity != null ? this.ToDto(entity) : null;
        }

        /// <summary>
        /// Lists the <see cref="TDto" /> for the specified query.
        /// </summary>
        /// <param name="query">The query to filter</param>
        public IEnumerable<TDto> Find(string query)
        {
            Guard.NotNull(() => query, query);

            return this.StoreContext.Find(query)
                .Select(x => this.ToDto(x));
        }

        /// <summary>
        /// Adds a new <see cref="TDto"/> to the store.
        /// </summary>
        /// <param name="dto">The <see cref="TDto"/> to store</param>
        public string Add(TDto dto)
        {
            Guard.NotNull(() => dto, dto);

            var entity = this.FromDto(dto);
            entity.Id = this.CreateIdentity(entity);

            this.StoreContext.Add(entity);

            return entity.Id;
        }

        /// <summary>
        /// Updates the <see cref="TDto"/> in the store.
        /// </summary>
        /// <param name="identifier">The identifier of the <see cref="TDto" /></param>
        /// <param name="dto">The  <see cref="TDto" /> to update</param>
        public TDto Update(string identifier, TDto dto)
        {
            Guard.NotNullOrEmpty(() => identifier, identifier);
            Guard.NotNull(() => dto, dto);

            var entity = this.StoreContext.Update(identifier, this.FromDto(dto));
            return entity != null ? this.ToDto(entity) : null;
        }

        /// <summary>
        /// Deletes the <see cref="TDto"/> from the store.
        /// </summary>
        /// <param name="identifier"></param>
        public void Delete(string identifier)
        {
            Guard.NotNullOrEmpty(() => identifier, identifier);

            this.StoreContext.Delete(identifier);
        }
    }
}
