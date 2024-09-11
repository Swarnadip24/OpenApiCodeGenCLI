using Xunit;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace OpenApiCodeGenCLI.Tests
{
    public class ArgumentParsingTests
    {
        [Fact]
        public async Task ShouldParseArgumentsCorrectly()
        {
            var rootCommand = new RootCommand
            {
                new Option<string>("--spec", "Path to the OpenAPI specification file"),
                new Option<string>("--output", "Output directory for the generated code"),
                new Option<string>("--language", "Programming language for code generation")
            };

            string spec = null;
            string output = null;
            string language = null;

            rootCommand.Handler = CommandHandler.Create<string, string, string>((s, o, l) =>
            {
                spec = s;
                output = o;
                language = l;
            });

            await rootCommand.InvokeAsync("--spec api.yaml --output ./output --language csharp");

            Assert.Equal("api.yaml", spec);
            Assert.Equal("./output", output);
            Assert.Equal("csharp", language);
        }
    }
}