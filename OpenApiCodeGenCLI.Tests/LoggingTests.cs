using Xunit;
using Microsoft.Extensions.Logging;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using System;

namespace OpenApiCodeGenCLI.Tests
{
    public class LoggingTests
    {
        [Fact]
        public async Task ShouldLogInformationCorrectly()
        {
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
                logger.LogInformation($"Spec: {spec}, Output: {output}, Language: {language}");
            });

            // Redirect console output for testing purposes
            using var sw = new StringWriter();
            Console.SetOut(sw);

            await rootCommand.InvokeAsync("--spec api.yaml --output ./output --language csharp");

            var output = sw.ToString();
            Assert.Contains("Spec: api.yaml, Output: ./output, Language: csharp", output);
        }
    }
}