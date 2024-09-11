using Xunit;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using System;

namespace OpenApiCodeGenCLI.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public async Task ShouldLoadConfigurationFromFile()
        {
            // Create a sample config file
            var config = new Config { Spec = "api.yaml", Output = "./output", Language = "csharp" };
            await File.WriteAllTextAsync("config.json", JsonSerializer.Serialize(config));

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();

            var rootCommand = new RootCommand
            {
                new Option<string>("--spec", "Path to the OpenAPI specification file"),
                new Option<string>("--output", "Output directory for the generated code"),
                new Option<string>("--language", "Programming language for code generation")
            };

            rootCommand.Handler = CommandHandler.Create<string, string, string>((spec, output, language) =>
            {
                spec = config.Spec;
                output = config.Output;
                language = config.Language;
                logger.LogInformation($"Spec: {spec}, Output: {output}, Language: {language}");
            });

            using var sw = new StringWriter();
            Console.SetOut(sw);

            await rootCommand.InvokeAsync("");

            var output = sw.ToString();
            Assert.Contains($"Spec: api.yaml, Output: ./output, Language: csharp", output);

            // Clean up
            File.Delete("config.json");
        }
    }
}