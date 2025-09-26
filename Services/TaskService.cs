using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskStatus = Models.TaskStatus;

namespace Services
{
    public class TaskService
    {
        private readonly FileService _fileService;
        private readonly List<Tasks> _tasks;

        public TaskService(FileService fileService)
        {
            if (fileService == null)
                throw new ArgumentNullException(nameof(fileService));

            _fileService = fileService;

            try
            {
                _tasks = _fileService.LoadTasks() ?? new List<Tasks>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskService] Failed to load tasks from file: {ex.Message}");
                _tasks = new List<Tasks>();
            }
        }

        public bool CreateTask(string title, string description, Guid projectId, Guid assignedUserId,
                               DateTime dueDate, TaskPriority priority, ProjectService projectService)
        {
            try
            {
                var project = projectService.GetProjectById(projectId);
                if (project == null || !project.AssignedUserIds.Contains(assignedUserId))
                    return false;

                var task = new Tasks
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Description = description,
                    ProjectId = projectId,
                    AssignedUserId = assignedUserId,
                    DueDate = dueDate,
                    Priority = priority,
                    Status = TaskStatus.Todo
                };

                _tasks.Add(task);
                _fileService.SaveTasks(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] CreateTask failed. {ex.Message}");
                return false;
            }
        }

        public bool UpdateTask(Tasks task)
        {
            try
            {
                if (task == null) return false;

                var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
                if (existing == null) return false;

                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.AssignedUserId = task.AssignedUserId;
                existing.DueDate = task.DueDate;
                existing.Priority = task.Priority;
                existing.Status = task.Status;

                _fileService.SaveTasks(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] UpdateTask failed. {ex.Message}");
                return false;
            }
        }

        public bool DeleteTask(Guid taskId)
        {
            try
            {
                var task = _tasks.FirstOrDefault(x => x.Id == taskId);
                if (task == null) return false;

                _tasks.Remove(task);
                _fileService.SaveTasks(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] DeleteTask failed. {ex.Message}");
                return false;
            }
        }

        public bool DeleteTasksForProject(Guid projectId)
        {
            try
            {
                var tasks = _tasks.Where(t => t.ProjectId == projectId).ToList();
                if (tasks.Count == 0) return false;

                foreach (var task in tasks)
                    _tasks.Remove(task);

                _fileService.SaveTasks(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] DeleteTasksForProject failed. {ex.Message}");
                return false;
            }
        }

        public List<Tasks> GetTasksForProject(Guid projectId)
        {
            try
            {
                return _tasks.Where(x => x.ProjectId == projectId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetTasksForProject failed. {ex.Message}");
                return new List<Tasks>();
            }
        }

        public List<Tasks> GetTasksForUser(Guid userId)
        {
            try
            {
                return _tasks.Where(x => x.AssignedUserId == userId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetTasksForUser failed. {ex.Message}");
                return new List<Tasks>();
            }
        }

        public bool UpdateTaskStatus(Guid taskId, TaskStatus status)
        {
            try
            {
                var task = _tasks.FirstOrDefault(x => x.Id == taskId);
                if (task == null) return false;

                task.Status = status;
                _fileService.SaveTasks(_tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] UpdateTaskStatus failed. {ex.Message}");
                return false;
            }
        }

        public List<Tasks> GetAllTasks()
        {
            try
            {
                return _tasks.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetAllTasks failed. {ex.Message}");
                return new List<Tasks>();
            }
        }

        public Project? GetProjectByTaskId(Guid taskId, List<Project> projects)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return null;

            return projects.FirstOrDefault(p => p.Id == task.ProjectId);
        }
    }
}
