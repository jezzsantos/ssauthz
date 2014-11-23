using System.Configuration;

namespace Common.Configuration
{
    /// <summary>
    ///     Defines configurations settings stored in an application configuration file.
    /// </summary>
    public class AppConfigurationSettings : IConfigurationSettings
    {
        /// <summary>
        ///     Returns the setting with the specified name
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        public virtual string GetSetting(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }
    }
}