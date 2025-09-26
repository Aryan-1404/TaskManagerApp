using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services
{
    public class FileService
    {
        private readonly string _userFilePath;
        private readonly string _projectFilePath;
        private readonly string _taskFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileService(
            string userFilePath = "data/users.json.enc",
            string projectFilePath = "data/projects.json.enc",
            string taskFilePath = "data/tasks.json.enc")
        {
            _userFilePath = userFilePath;
            _projectFilePath = projectFilePath;
            _taskFilePath = taskFilePath;

            try
            {
                foreach (var path in new[] { _userFilePath, _projectFilePath, _taskFilePath })
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error ensuring directories: {ex.Message}");
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        // --- Users ---
        public List<User> LoadUsers()
        {
            try
            {
                if (!File.Exists(_userFilePath)) return new List<User>();
                var enc = File.ReadAllText(_userFilePath);
                if (string.IsNullOrWhiteSpace(enc)) return new List<User>();
                var json = CryptoHelper.Decrypt(enc);
                return JsonSerializer.Deserialize<List<User>>(json, _jsonOptions) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error loading users: {ex.Message}");
                return new List<User>();
            }
        }

        public void SaveUsers(List<User> users)
        {
            try
            {
                var json = JsonSerializer.Serialize(users, _jsonOptions);
                var enc = CryptoHelper.Encrypt(json);
                File.WriteAllText(_userFilePath, enc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error saving users: {ex.Message}");
            }
        }

        // --- Projects ---
        public List<Project> LoadProjects()
        {
            try
            {
                if (!File.Exists(_projectFilePath)) return new List<Project>();
                var enc = File.ReadAllText(_projectFilePath);
                if (string.IsNullOrWhiteSpace(enc)) return new List<Project>();
                var json = CryptoHelper.Decrypt(enc);
                return JsonSerializer.Deserialize<List<Project>>(json, _jsonOptions) ?? new List<Project>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error loading projects: {ex.Message}");
                return new List<Project>();
            }
        }

        public void SaveProjects(List<Project> projects)
        {
            try
            {
                var json = JsonSerializer.Serialize(projects, _jsonOptions);
                var enc = CryptoHelper.Encrypt(json);
                File.WriteAllText(_projectFilePath, enc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error saving projects: {ex.Message}");
            }
        }

        // --- Tasks ---
        public List<Tasks> LoadTasks()
        {
            try
            {
                if (!File.Exists(_taskFilePath)) return new List<Tasks>();
                var enc = File.ReadAllText(_taskFilePath);
                if (string.IsNullOrWhiteSpace(enc)) return new List<Tasks>();
                var json = CryptoHelper.Decrypt(enc);
                return JsonSerializer.Deserialize<List<Tasks>>(json, _jsonOptions) ?? new List<Tasks>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error loading tasks: {ex.Message}");
                return new List<Tasks>();
            }
        }

        public void SaveTasks(List<Tasks> tasks)
        {
            try
            {
                var json = JsonSerializer.Serialize(tasks, _jsonOptions);
                var enc = CryptoHelper.Encrypt(json);
                File.WriteAllText(_taskFilePath, enc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService] Error saving tasks: {ex.Message}");
            }
        }
    }
}
