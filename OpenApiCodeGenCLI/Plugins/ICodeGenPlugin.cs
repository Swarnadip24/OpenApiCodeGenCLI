namespace OpenApiCodeGenCLI.Plugins
{
    public interface ICodeGenPlugin
    {
        string Language { get; }
        string Version { get; }
        void Initialize(string templateDirectory, IDictionary<string, string> configuration = null);
        string GenerateCode(string openApiSpec);
        void CleanUp();
    }
}