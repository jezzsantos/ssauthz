namespace Common
{
    /// <summary>
    ///     Defines a type which has an identifier field.
    /// </summary>
    public interface IHasIdentifier
    {
        /// <summary>
        ///     Gets or sets the identifier of the type.
        /// </summary>
        string Id { get; set; }
    }
}