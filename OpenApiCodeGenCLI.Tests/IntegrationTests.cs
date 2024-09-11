using Xunit;
using System.IO;
using System.Threading.Tasks;

namespace OpenApiCodeGenCLI.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task ShouldGenerateCodeUsingCLI()
        {
            // Set up a test OpenAPI spec and template
            var specPath = "test-openapi.yaml";
            var outputPath = "output";
            var templatePath = "Templates/CSharp/class.sbn";
            Directory.CreateDirectory("Templates/CSharp");
            await File.WriteAllTextAsync(specPath, "openapi content");
            await File.WriteAllTextAsync(templatePath, "public class {{ ClassName }} { }");

            var args = $"--spec {specPath} --output {outputPath} --language csharp";
            await Program.Main(args.Split(' ')); // Use the public Program class

            var generatedCode = File.ReadAllText(Path.Combine(outputPath, "csharp.generated.cs"));
            Assert.Contains("public class SampleClass", generatedCode);

            // Clean up
            Directory.Delete("Templates", true);
            Directory.Delete(outputPath, true);
            File.Delete(specPath);
        }
    }
}