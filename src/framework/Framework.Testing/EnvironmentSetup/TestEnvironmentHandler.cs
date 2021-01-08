using System;

namespace HumanaEdge.Webcore.Framework.Testing.EnvironmentSetup
{
    /// <summary>
    /// Class for handling environment.
    /// </summary>
    internal static class TestEnvironmentHandler
    {
        /// <summary>
        /// Retrieves the current application environment setting for tests.
        /// </summary>
        internal static string GetEnvironment
        {
            get
            {
                var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (environment == null)
                {
                    Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", "DEV");
                    environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                }

                return environment !;
            }
        }
    }
}