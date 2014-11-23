namespace Common.Configuration
{
    /// <summary>
    ///     Defines a configuration settings class
    /// </summary>
    public interface IConfigurationSettings
    {
        /// <summary>
        ///     Returns the setting with the specified name
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        string GetSetting(string settingName);
    }
}