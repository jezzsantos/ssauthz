namespace Services.DataContracts
{
    /// <summary>
    ///     Defines an addrss for a <see cref="UserAccount" />
    /// </summary>
    public class Address
    {
        /// <summary>
        ///     Gets or sets the first street of the address
        /// </summary>
        public string Street1 { get; set; }

        /// <summary>
        ///     Gets or sets the second street of the address
        /// </summary>
        public string Street2 { get; set; }

        /// <summary>
        ///     Gets or sets the town of the address
        /// </summary>
        public string Town { get; set; }
    }
}