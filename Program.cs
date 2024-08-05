using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FileCopyFromLog
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: FileCopyFromLog.exe <destinationBasePath> <logFilePath> [levelsToSkip]");
                return;
            }

            string destinationBasePath = args[0];
            string logFilePath = args[1];
            int levelsToSkip = args.Length > 2 ? int.Parse(args[2]) : 0;

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
                        string sourcePath = logEntry.FullPath;
                        string destinationPath = Path.Combine(destinationBasePath, GetRelativePath(sourcePath, levelsToSkip));

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
                        else if (logEntry.ChangeType == "Renamed" && logEntry.OldFullPath != null)
                        {
                            string oldDestinationPath = Path.Combine(destinationBasePath, GetRelativePath(logEntry.OldFullPath, levelsToSkip));

                            // Delete the old file in the destination directory
                            if (File.Exists(oldDestinationPath))
                            {
                                File.Delete(oldDestinationPath);
                                Console.WriteLine($"Deleted (old path): {oldDestinationPath}");
                            }

                            // Ensure the new path directory exists and copy the file
                            string? newDestinationDirectory = Path.GetDirectoryName(destinationPath);
                            if (newDestinationDirectory != null)
                            {
                                Directory.CreateDirectory(newDestinationDirectory);
                                RunRobocopy(sourcePath, destinationPath);
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
                Arguments = $"\"{Path.GetDirectoryName(source)}\" \"{destinationDirectory}\" \"{Path.GetFileName(source)}\" /COPYALL /E /PURGE /Z",
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

        private static string GetRelativePath(string fullPath, int levelsToSkip)
        {
            // This function removes the specified number of initial parts of the network path to get the relative path
            var parts = fullPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (levelsToSkip >= parts.Length)
            {
                // If levelsToSkip is greater than or equal to the number of path segments, return the last segment
                return Path.Combine(parts[^1]);
            }
            // Skip the specified number of levels and return the remaining path
            return Path.Combine(parts[levelsToSkip..]);
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