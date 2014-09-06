using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using MvcLib.Common;

namespace MvcLib.PluginLoader
{
    public static class PluginStorage
    {
        private static readonly Dictionary<string, Assembly> StoredAssemblies = new Dictionary<string, Assembly>();

        internal static void Register(string fileName)
        {
            try
            {
                var loadingAssembly = Assembly.LoadFile(fileName);
                
                Register(loadingAssembly);
            }
            catch (Exception ex)
            {
                var msg = "ERRO LOADING ASSEMBLY: {0}: {1}".Fmt(fileName, ex);
                Trace.TraceInformation(msg);
                LogEvent.Raise(ex.Message, ex);
            }
        }

        internal static void Register(Assembly assembly)
        {
            if (StoredAssemblies.ContainsKey(assembly.FullName))
            {
                return;
            }

            StoredAssemblies.Add(assembly.FullName, assembly);
            BuildManager.AddReferencedAssembly(assembly);

            Trace.TraceInformation("[PluginLoader]:Plugin registered: {0}", assembly.FullName);
        }

        internal static Assembly FindAssembly(string fullName)
        {
            return StoredAssemblies.ContainsKey(fullName)
                ? StoredAssemblies[fullName]
                : null;
        }

        public static IEnumerable<string> GetPluginNames()
        {
            return StoredAssemblies.Values.Select(item => item.GetName().Name);
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            return StoredAssemblies.Select(pair => pair.Value);
        }
    }
}