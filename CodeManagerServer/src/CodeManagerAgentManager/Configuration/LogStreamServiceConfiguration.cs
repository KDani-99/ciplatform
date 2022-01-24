namespace CodeManagerAgentManager.Configuration
{
    public class LogStreamServiceConfiguration
    {
        public int MaxLinePerFile { get; set; } 
        public int MaxFileSize { get; set; } // in bytes
        public string LogPath { get; set; }
    }
}