namespace Common.Storage.DataEntities
{
    /// <summary>
    ///     Defines a keyed table entity.
    /// </summary>
    public abstract class KeyedEntity : IHasIdentifier
    {
        /// <summary>
        ///     Gets or sets the identifier of the entity.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the key for the row of the entity
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        ///     Merges the properties from the <see cref="entity" /> into this entity
        /// </summary>
        /// <param name="entity">The entity to merge</param>
        public abstract void Merge(KeyedEntity entity);
    }
}