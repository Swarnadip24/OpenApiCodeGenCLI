using Scriban;
using System.IO;
using OpenApiCodeGenCLI.Plugins;

namespace CSharpPlugin
{
    public class CSharpPlugin : CodeGenPluginBase
    {
        public CSharpPlugin()
        {
            Language = "csharp";
            Version = "1.0.0";
        }

        public override string GenerateCode(string openApiSpec)
        {
            // Load template
            var templatePath = Path.Combine(TemplateDirectory, "class.sbn");
            var templateText = File.ReadAllText(templatePath);

            // Parse OpenAPI spec (simplified for demo purposes)
            var template = Template.Parse(templateText);

            // Generate code
            return template.Render(new { ClassName = "SampleClass" });
        }
    }
}