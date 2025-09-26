using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class ProjectService
    {
        private readonly FileService _fileService;
        private readonly List<Project> _projects;

        public ProjectService(FileService fileService)
        {
            if (fileService == null)
                throw new ArgumentNullException(nameof(fileService));

            _fileService = fileService;

            try
            {
                _projects = _fileService.LoadProjects() ?? new List<Project>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProjectService] Failed to load projects from file: {ex.Message}");
                _projects = new List<Project>();
            }

            try
            {
                foreach (var p in _projects)
                {
                    p.AssignedUserIds ??= new List<Guid>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProjectService] Error initializing project user lists: {ex.Message}");
            }
        }
        

        public bool CreateProject(string projectName, List<Guid> assignedUserIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectName)) return false;

                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = projectName,
                    AssignedUserIds = assignedUserIds ?? new List<Guid>()
                };

                _projects.Add(project);
                _fileService.SaveProjects(_projects);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] CreateProject failed. {ex.Message}");
                return false;
            }
        }

        public bool UpdateProject(Guid projectId, string newName)
        {
            try
            {
                var project = _projects.FirstOrDefault(p => p.Id == projectId);
                if (project == null || string.IsNullOrWhiteSpace(newName)) return false;

                project.Name = newName;
                _fileService.SaveProjects(_projects);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] UpdateProject failed. {ex.Message}");
                return false;
            }
        }

        public bool DeleteProject(Guid projectId)
        {
            try
            {
                var project = _projects.FirstOrDefault(p => p.Id == projectId);
                if (project == null) return false;

                _projects.Remove(project);
                _fileService.SaveProjects(_projects);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] DeleteProject failed. {ex.Message}");
                return false;
            }
        }

        public bool AddUserToProject(Guid projectId, List<Guid> userIds)
        {
            try
            {
                var project = _projects.FirstOrDefault(p => p.Id == projectId);
                if (project == null) return false;

                project.AssignedUserIds ??= new List<Guid>();

                foreach (var uid in userIds ?? new List<Guid>())
                {
                    if (!project.AssignedUserIds.Contains(uid))
                        project.AssignedUserIds.Add(uid);
                }

                _fileService.SaveProjects(_projects);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] AddUserToProject failed. {ex.Message}");
                return false;
            }
        }

        public List<Project> GetAllProjects() => _projects;

        public List<Project> GetProjectsByUserId(Guid userId)
        {
            try
            {
                return _projects.Where(p => p.AssignedUserIds.Contains(userId)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetProjectsByUserId failed. {ex.Message}");
                return new List<Project>();
            }
        }

        public Project? GetProjectById(Guid projectId)
        {
            try
            {
                return _projects.FirstOrDefault(p => p.Id == projectId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetProjectById failed. {ex.Message}");
                return null;
            }
        }
    }
}
