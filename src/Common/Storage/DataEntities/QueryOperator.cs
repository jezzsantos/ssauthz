namespace Common.Storage.DataEntities
{
    /// <summary>
    ///     Comparison operators for queries in table storage
    /// </summary>
    public enum QueryOperator
    {
        /// <summary>
        ///     Equals
        /// </summary>
        EQ,

        /// <summary>
        ///     Greater than
        /// </summary>
        GT,

        /// <summary>
        ///     Greater than or equal to
        /// </summary>
        GE,

        /// <summary>
        ///     Less than
        /// </summary>
        LT,

        /// <summary>
        ///     Less than or equal to
        /// </summary>
        LE,

        /// <summary>
        ///     Not equal
        /// </summary>
        NE,
    }
}