using System.Diagnostics;
using System.IO;

namespace Elephanet.Tests.Infrastructure
{
    internal static class TestStoreDatabaseFactory
    {
        private const string _psqlPath = @"C:\Program Files\PostgreSQL\9.4\bin\psql.exe"; //todo add alternative paths or move to config if necessary
        
        internal static void CreateCleanStoreDatabase()
        {
            Debug.WriteLine("Test Store Database Creating");

            var tempScriptFileName = WriteTempSqlScript();
            var command = $"-f {tempScriptFileName} -U postgres";

            Debug.WriteLine($"Executing script {tempScriptFileName} with exe {_psqlPath} and command {command}.");

            StartAndOutputProcess(command);

            Debug.WriteLine("Test Store Database Created");

            File.Delete(tempScriptFileName);
        }

        private static void StartAndOutputProcess(string command)
        {
            var process = new Process { StartInfo = ProcessInfo(command) };
            process.Start();
            WriteOutputToDebug(process);
        }

        private static void WriteOutputToDebug(Process process)
        {
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (line != null)
                {
                    Debug.WriteLine(line);
                }
            }
        }

        private static ProcessStartInfo ProcessInfo(string command)
        {
            return new ProcessStartInfo
            {
                FileName = _psqlPath,
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
        }

        private static string WriteTempSqlScript()
        {
            var script = typeof(TestStore).ReadResourceString("CreateStore.sql");

            var tempScriptFileName = Path.GetTempFileName();
            File.WriteAllText(tempScriptFileName, script);
            return tempScriptFileName;
        }

    }
}
