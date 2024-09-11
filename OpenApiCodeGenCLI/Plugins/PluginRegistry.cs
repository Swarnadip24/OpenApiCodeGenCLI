using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace OpenApiCodeGenCLI.Plugins
{
    public class PluginRegistry
    {
        public static IEnumerable<PluginMetadata> DiscoverPlugins(string pluginDirectory)
        {
            var plugins = new List<PluginMetadata>();

            if (!Directory.Exists(pluginDirectory))
            {
                Console.WriteLine($"Plugin directory '{pluginDirectory}' does not exist.");
                return plugins;
            }

            var pluginFiles = Directory.GetFiles(pluginDirectory, "plugin.json", SearchOption.AllDirectories);
            Console.WriteLine($"Found {pluginFiles.Length} plugin.json files.");

            foreach (var pluginFile in pluginFiles)
            {
                Console.WriteLine($"Reading plugin metadata from: {pluginFile}");
                var json = File.ReadAllText(pluginFile);
                var metadata = JsonSerializer.Deserialize<PluginMetadata>(json);
                if (metadata != null)
                {
                    Console.WriteLine($"Discovered plugin: {metadata.Name} for languages: {string.Join(", ", metadata.Languages)}");
                    plugins.Add(metadata);
                }
                else
                {
                    Console.WriteLine($"Failed to deserialize plugin metadata from: {pluginFile}");
                }
            }

            return plugins;
        }
    }

    public class PluginMetadata
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string[] Languages { get; set; }
        public string Assembly { get; set; }
    }
}