using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenApiCodeGenCLI.Plugins;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OpenApiCodeGenCLI
{
    public class Config
    {
        public string Spec { get; set; }
        public string Output { get; set; }
        public string Language { get; set; }
        public string PluginPath { get; set; }
        public Dictionary<string, Dictionary<string, string>> PluginSettings { get; set; }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();

            // Load configuration
            Config config = null;
            if (File.Exists("config.json"))
            {
                string json = await File.ReadAllTextAsync("config.json");
                config = JsonSerializer.Deserialize<Config>(json);
            }
            else
            {
                logger.LogWarning("Configuration file 'config.json' not found.");
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            logger.LogInformation($"Current Directory: {currentDirectory}");

            var rootCommand = new RootCommand
            {
                new Option<string>("--spec", "Path to the OpenAPI specification file"),
                new Option<string>("--output", "Output directory for the generated code"),
                new Option<string>("--language", "Programming language for code generation")
            };

            rootCommand.Description = "OpenAPI Code Generation CLI";

            rootCommand.Handler = CommandHandler.Create<string, string, string>((spec, output, language) =>
            {
                spec = config?.Spec ?? spec;
                output = config?.Output ?? output;
                language = config?.Language ?? language;

                if (spec == null || output == null || language == null)
                {
                    logger.LogError("Spec, Output, and Language must all be provided.");
                    return 1;
                }

                if (!File.Exists(spec))
                {
                    logger.LogError($"Spec file '{spec}' not found.");
                    return 1;
                }

                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }

                logger.LogInformation($"Spec: {spec}, Output: {output}, Language: {language}");

                var pluginPath = config?.PluginPath ?? "Plugins";
                logger.LogInformation($"Looking for plugins in: {pluginPath}");

                if (Directory.Exists(pluginPath))
                {
                    var files = Directory.GetFiles(pluginPath);
                    logger.LogInformation($"Files in {pluginPath}: {string.Join(", ", files)}");
                }
                else
                {
                    logger.LogError($"Plugin directory '{pluginPath}' does not exist.");
                }

                var plugins = PluginRegistry.DiscoverPlugins(pluginPath);
                logger.LogInformation($"Discovered plugins: {string.Join(", ", plugins.Select(p => p.Name))}");

                var pluginMetadata = plugins.FirstOrDefault(p => p.Languages.Contains(language, StringComparer.OrdinalIgnoreCase));
                if (pluginMetadata != null)
                {
                    logger.LogInformation($"Found compatible plugin: {pluginMetadata.Name}");
                    var pluginAssemblyPath = Path.Combine(pluginPath, pluginMetadata.Assembly);
                    logger.LogInformation($"Loading plugin assembly from: {pluginAssemblyPath}");
                    ICodeGenPlugin plugin = PluginLoader.LoadPlugin(pluginAssemblyPath, language);

                    if (plugin != null)
                    {
                        var templateDirectory = Path.Combine(currentDirectory, "Templates", "CSharp");
                        logger.LogInformation($"Initializing plugin with template directory: {templateDirectory}");
                        var pluginConfiguration = config?.PluginSettings?[language];
                        plugin.Initialize(templateDirectory, pluginConfiguration);
                        logger.LogInformation("Generating code...");
                        var code = plugin.GenerateCode(spec);
                        logger.LogInformation("Writing generated code to file...");
                        File.WriteAllText(Path.Combine(output, $"{plugin.Language}.generated.cs"), code);
                        plugin.CleanUp();
                        logger.LogInformation("Plugin cleanup complete.");
                    }
                    else
                    {
                        logger.LogError($"Failed to load plugin from {pluginAssemblyPath}");
                        return 1;
                    }
                }
                else
                {
                    logger.LogError($"No compatible plugin found for language: {language}");
                    return 1;
                }

                return 0;
            });

            return await rootCommand.InvokeAsync(args);
        }
    }
}