using System.Collections.Generic;
using System.Diagnostics;

namespace CodeManagerAgent.Extensions
{
    public static class ProcessExtensions
    {
        public static void ConfigureCliProcess(this Process process,
                                               string executable,
                                               string arguments,
                                               IEnumerable<string> environment)
        {
            process.StartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = executable,
                Arguments = arguments
            };

            foreach (var env in environment)
            {
                var (key, value) = env.Split("=") switch {var result => (result[0], result[1])};
                process.StartInfo.Environment.Add(key, value);
            }
        }
    }
}