using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Storage.DataEntities;

namespace Common.Storage
{
    /// <summary>
    ///     An in-memory persistance store
    /// </summary>
    public class InMemoryStoreContext<TEntity> : IStoreContext<TEntity> where TEntity : KeyedEntity
    {
        private static readonly Dictionary<string, TEntity> StoredItems = new Dictionary<string, TEntity>();

        public string BuildQuery(string propertyName, QueryOperator operation, string givenValue)
        {
            return "'{0}' {1} '{2}'".FormatWithInvariant(propertyName, operation, givenValue);
        }

        public virtual TEntity Get(string identifier)
        {
            return StoredItems[identifier];
        }

        public IEnumerable<TEntity> Find(string queryString)
        {
            // TODO: Must complete this method
            // The AzureTableStorage classes from which this stuff was taken from supported a query syntax like this: 'PropertyName' EQ 'value'
            // So we need to convert this information into meaniful code to execute against a Dictionary<string, TEntity>.
            // We can convert the information into a class first.
            // note: I think we only need to support 'NE' and 'EQ' operators for this sample to work

            Query query = ParseQueryString(queryString);
            if (query.Operator == QueryOperator.EQ)
            {
                //TODO: Implement matching on property value
            }
            if (query.Operator == QueryOperator.NE)
            {
                //TODO: Implement not matching on property value
            }

            return Enumerable.Empty<TEntity>();
        }

        public void Add(TEntity entity)
        {
            StoredItems.Add(entity.Id, entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            entities.ToList()
                .ForEach(e => StoredItems.Add(e.Id, e));
        }

        public void Delete(string identifier)
        {
            StoredItems.Remove(identifier);
        }

        public TEntity Update(string identifier, TEntity entity)
        {
            StoredItems[identifier] = entity;
            return entity;
        }

        /// <summary>
        /// Converts a textual AzureTable storage query into the <see cref="Query"/> class
        /// </summary>
        private static Query ParseQueryString(string queryString)
        {
            Match matches = Regex.Match(queryString, @"(?<prop>'[\w]*') (?<op>[\w]{2}) (?<val>'[\w\d\.\@]*')");
            if (matches.Success)
            {
                return new Query
                {
                    PropertyName = matches.Groups["prop"].Value,
                    Operator = (QueryOperator) Enum.Parse(typeof (QueryOperator), matches.Groups["op"].Value),
                    Value = matches.Groups["val"].Value,
                };
            }

            throw new InvalidOperationException(@"The query format is not recognized");
        }

        private class Query
        {
            public string PropertyName { get; set; }

            public QueryOperator Operator { get; set; }

            public string Value { get; set; }
        }
    }
}