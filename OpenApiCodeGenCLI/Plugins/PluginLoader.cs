using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenApiCodeGenCLI.Plugins
{
    public class PluginLoader
    {
        public static ICodeGenPlugin? LoadPlugin(string pluginPath, string language)
        {
            if (!File.Exists(pluginPath))
            {
                throw new FileNotFoundException($"Plugin assembly not found at {pluginPath}");
            }

            var assembly = Assembly.LoadFrom(pluginPath);
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(ICodeGenPlugin).IsAssignableFrom(t) && !t.IsAbstract && t.Name.ToLower().Contains(language.ToLower()));

            return pluginType != null ? (ICodeGenPlugin?)Activator.CreateInstance(pluginType) : null;
        }
    }
}