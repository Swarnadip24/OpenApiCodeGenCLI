using Xunit;
using OpenApiCodeGenCLI.Plugins;
using System.IO;
using CSharpPlugin; // Include the CSharpPlugin namespace

namespace OpenApiCodeGenCLI.Tests
{
    public class PluginTests
    {
        [Fact]
        public void ShouldGenerateCodeUsingCSharpPlugin()
        {
            var plugin = new CSharpPlugin.CSharpPlugin();
            plugin.Initialize("./Templates/CSharp");
            var code = plugin.GenerateCode("openapi.yaml");
            Assert.Contains("public class SampleClass", code);
        }
    }
}