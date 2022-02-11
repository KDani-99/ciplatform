using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeManagerAgent.Extensions;
using NUnit.Framework;

namespace CodeManagerAgent.Tests
{
    public class ProcessExtensionsTests
    {
        [Test]
        public void Configure_CliProcess_ShouldConfigureProcessInstance()
        {
            // Assert
            var process = new Process();
            var executable = "powershell";
            var args = "test-args";
            var key = "KEY";
            var value = "VALUE";
            var envVar = $"{key}={value}";
            var environmentVariables = new List<string>
            {
                envVar
            };
            // Act
            process.ConfigureCliProcess(executable, args, environmentVariables);

            // Assert
            Assert.AreEqual(executable, process.StartInfo.FileName);
            Assert.AreEqual(args, process.StartInfo.Arguments);
            Assert.Contains(key, process.StartInfo.Environment.Keys.ToList());
            Assert.Contains(value, process.StartInfo.Environment.Values.ToList());
        }
    }
}