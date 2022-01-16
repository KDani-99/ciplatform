using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class StartRunCommand
    {
        public RunConfiguration RunConfiguration { get; set; }
        public string ContextFilePath { get; set; }
    }
}