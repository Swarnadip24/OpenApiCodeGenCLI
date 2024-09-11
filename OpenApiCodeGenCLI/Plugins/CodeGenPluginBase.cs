using System.Collections.Generic;

namespace OpenApiCodeGenCLI.Plugins
{
    public abstract class CodeGenPluginBase : ICodeGenPlugin
    {
        public string Language { get; protected set; }
        public string Version { get; protected set; }
        protected string TemplateDirectory { get; private set; }
        protected IDictionary<string, string> Configuration { get; private set; }

        public virtual void Initialize(string templateDirectory, IDictionary<string, string> configuration = null)
        {
            TemplateDirectory = templateDirectory;
            Configuration = configuration ?? new Dictionary<string, string>();
        }

        public abstract string GenerateCode(string openApiSpec);

        public virtual void CleanUp() { }
    }
}