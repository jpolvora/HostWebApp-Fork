using System;
using System.Configuration;
using System.Diagnostics;
using MvcLib.Common.Configuration;

namespace MvcLib.Common
{
    public static class Config
    {
        public static T ValueOrDefault<T>(string key, T defaultValue)
        {
            var cfgValue = ConfigurationManager.AppSettings[key];
<<<<<<< HEAD
            if (BootstrapperSection.Instance.Verbose)
=======
            if (Debugger.IsAttached)
>>>>>>> b7a9f8fb0cc3b8774ccc2bf228cf6db187f2d02e
            {
                Trace.TraceInformation("[Config]: Value for '{0}' is '{1}' (default is {2})", key, cfgValue, defaultValue);
            }

            if (string.IsNullOrEmpty(cfgValue))
            {
                return defaultValue;
            }

            return cfgValue.As<T>();
        }

        public static bool IsInDebugMode { get; private set; }

        static Config()
        {
            var environment = ValueOrDefault("Environment", "Debug");
            Trace.TraceInformation("Environment is {0}", environment);
            IsInDebugMode = environment.Equals("Debug", StringComparison.OrdinalIgnoreCase);
        }
    }
}