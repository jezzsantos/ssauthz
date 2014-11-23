namespace Common
{
    /// <summary>
    ///     A global Licensing class.
    /// </summary>
    public static class Licensing
    {
        private const string ServiceStackLicenseKey =
            "<YOURSERVICESTACKLICENEKEYHERE>";

        /// <summary>
        ///     License the ServiceStack runtime.
        /// </summary>
        public static void LicenseServiceStackRuntime()
        {
            ServiceStack.Licensing.RegisterLicense(ServiceStackLicenseKey);
        }
    }
}