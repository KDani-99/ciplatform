﻿using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class StartRunCommand
    {
        // TODO: remove this
        public RunConfiguration RunConfiguration { get; set; }
        public string ContextFilePath { get; set; } // github path
        public string RunId { get; set; }
    }
}