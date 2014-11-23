using System;
using System.Diagnostics;
using System.Linq;
using Common;
using Common.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.MessageContracts;

namespace Services.IntTests
{
    [TestClass]
    public abstract class IntegrationTest
    {
        private const string WebServerServiceProcessName = @"IntegrationTest.WebServerProcessName";
        private const string WebServerStartCommandSettingName = @"IntegrationTest.WebServerStartCommand";
        private const string WebServerStartArgumentsSettingName = @"IntegrationTest.WebServerStartArguments";

        protected static IConfigurationSettings Settings = new AppConfigurationSettings();
        protected ServiceClient AuthZClient;
        protected ServiceClient Client;

        [AssemblyInitialize]
        public static void InitializeAllContexts(TestContext context)
        {
            Licensing.LicenseServiceStackRuntime();
            StartupWebSite();
        }

        [AssemblyCleanup]
        public static void CleanupAllContexts()
        {
            KillProcesses();
        }

        /// <summary>
        ///     Resets Azure Storage emulator every test, and authenticates client
        /// </summary>
        protected void InitializeContext()
        {
            string baseUri = Settings.GetSetting(@"AuthZService.BaseUrl");

            AuthZClient = new ServiceClient(baseUri);
            Client = new ServiceClient(baseUri);

            ResetServices();
            ResetStorage();
        }

        private static void ResetStorage()
        {
            //TODO: reset all storage
        }

        private void ResetServices()
        {
            ////Call ResetWebRole Endpoint for service
            this.Client.Get(new ResetWebRole());
        }

        private static void StartupWebSite()
        {
            string toolPath = Settings.GetSetting(WebServerStartCommandSettingName);
            string arguments = Settings.GetSetting(WebServerStartArgumentsSettingName);

            RunCommand(toolPath, arguments, true);
        }

        private static void RunCommand(string toolPath, string args, bool elevated = true)
        {
            string toolPathResolved = Environment.ExpandEnvironmentVariables(toolPath);
            var startInfo = new ProcessStartInfo(toolPathResolved, args)
            {
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            if (elevated)
            {
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
            }

            using (var proc = new Process {StartInfo = startInfo})
            {
                try
                {
                    proc.Start();
                }
                catch
                {
                    //ignore issue
                }
            }
        }

        private static void KillProcesses()
        {
            try
            {
                // Kill remaining process(es)
                string processName = Settings.GetSetting(WebServerServiceProcessName);
                Process.GetProcessesByName(processName).ToList().ForEach(p =>
                {
                    p.Kill();
                    p.WaitForExit();
                });
            }
            catch (Exception)
            {
                //ignore problem
            }
        }
    }
}