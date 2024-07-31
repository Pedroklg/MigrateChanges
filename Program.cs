using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FileCopyFromLog
{
    class Program
    {
        static string sourceBasePath = @"C:\Users\pedro_silva\Desktop\PastaFrom"; // Ajuste conforme necessário
        static string destinationBasePath = @"\\backup\path"; // Ajuste conforme necessário
        static string logFilePath = @"C:\Users\pedro_silva\Desktop\LogsPastaP\events.json"; // Ajuste conforme necessário

        static void Main(string[] args)
        {
            try
            {
                // Read the log file
                if (File.Exists(logFilePath))
                {
                    string json = File.ReadAllText(logFilePath);
                    var logEntries = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();

                    // Process each log entry
                    foreach (var logEntry in logEntries)
                    {
                        string relativePath = Path.GetRelativePath(sourceBasePath, logEntry.FullPath);
                        string sourcePath = Path.Combine(sourceBasePath, relativePath);
                        string destinationPath = Path.Combine(destinationBasePath, relativePath);

                        if (logEntry.ChangeType == "Created" || logEntry.ChangeType == "Changed")
                        {
                            // Ensure destination directory exists and copy the file
                            string? destinationDirectory = Path.GetDirectoryName(destinationPath);
                            if (destinationDirectory != null)
                            {
                                Directory.CreateDirectory(destinationDirectory);
                                RunRobocopy(sourcePath, destinationPath);
                            }
                        }
                        else if (logEntry.ChangeType == "Deleted")
                        {
                            // Delete the file in the destination directory
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                                Console.WriteLine($"Deleted: {destinationPath}");
                            }
                        }
                    }

                    Console.WriteLine("File copy process completed.");
                }
                else
                {
                    Console.WriteLine("Log file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void RunRobocopy(string source, string destination)
        {
            // Ensure destination directory exists
            string? destinationDirectory = Path.GetDirectoryName(destination);
            if (destinationDirectory == null)
            {
                throw new InvalidOperationException("Destination directory path is null.");
            }
            Directory.CreateDirectory(destinationDirectory);

            // Set up the robocopy command
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{Path.GetDirectoryName(source)}\" \"{Path.GetDirectoryName(destination)}\" \"{Path.GetFileName(source)}\" /COPYALL /Z /MIR",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Run the robocopy command
            using (Process p = Process.Start(psi))
            {
                if (p != null)
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    Console.WriteLine(output);
                }
                else
                {
                    Console.WriteLine("Failed to start robocopy process.");
                }
            }
        }
    }

    public class LogEntry
    {
        public string Timestamp { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string? OldFullPath { get; set; }
    }
}
