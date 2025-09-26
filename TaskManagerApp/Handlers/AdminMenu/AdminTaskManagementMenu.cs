using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Handlers.AdminMenu
{
    public static class AdminTaskManagementMenu
    {
        
        public static void TaskManagementMenu(ProjectService projectService, TaskService taskService, UserService userService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Task Management -----");
                var projects = projectService.GetAllProjects();
                if (projects.Count == 0)
                {
                    Console.WriteLine("No projects available.");
                    Console.ReadLine();
                    return;
                }

                // Show all projects
                for (int i = 0; i < projects.Count; i++)
                    Console.WriteLine($"{i + 1}) {projects[i].Name}");
                Console.Write("Select Project serial number: ");
                if (!int.TryParse(Console.ReadLine(), out int pNo) || pNo < 1 || pNo > projects.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }
                var project = projects[pNo - 1];

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"----- Tasks for Project: {project.Name} -----");

                    var projTasks = taskService.GetTasksForProject(project.Id);
                    DisplayTasks(projTasks, project, userService);

                    Console.WriteLine("\n1) Add Task");
                    Console.WriteLine("2) Edit Task");
                    Console.WriteLine("3) Delete Task");
                    Console.WriteLine("4) Back to Admin Menu");
                    Console.Write("Choose an option: ");
                    var choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            AddTask(project, taskService, userService, projectService);
                            break;
                        case "2":
                            EditTask(projTasks, taskService);
                            break;
                        case "3":
                            DeleteTask(projTasks, taskService);
                            break;
                        case "4":
                            return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Task Management Menu: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        

        private static void DisplayTasks(List<Tasks> projTasks, Project project, UserService userService)
        {
            if (projTasks.Count == 0)
            {
                Console.WriteLine("No tasks yet.");
                return;
            }

            for (int i = 0; i < projTasks.Count; i++)
            {
                var uname = userService.GetAllUser()
                    .FirstOrDefault(u => u.Id == projTasks[i].AssignedUserId)?.Username ?? "Unknown";
                Console.WriteLine($"{i + 1}) {projTasks[i].Title} | Assigned to: {uname} | Due: {projTasks[i].DueDate:yyyy-MM-dd} | Priority: {projTasks[i].Priority} | Status: {projTasks[i].Status}");
            }
        }

        private static void AddTask(Project project, TaskService taskService, UserService userService, ProjectService projectService)
        {
            try
            {
                Console.Write("Task Title: ");
                var title = Console.ReadLine()?.Trim() ?? "";
                Console.Write("Description: ");
                var desc = Console.ReadLine()?.Trim() ?? "";

                var projUsers = userService.GetAllUser().Where(u => project.AssignedUserIds.Contains(u.Id)).ToList();
                if (projUsers.Count == 0)
                {
                    Console.WriteLine("No users assigned to this project. Add users first.");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < projUsers.Count; i++)
                    Console.WriteLine($"{i + 1}) {projUsers[i].Username}");
                Console.Write("Assign to User serial number: ");
                if (!int.TryParse(Console.ReadLine(), out int uNo) || uNo < 1 || uNo > projUsers.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }
                var uid = projUsers[uNo - 1].Id;

                Console.Write("Due Date (yyyy-MM-dd): ");
                var duedatestr = Console.ReadLine() ?? "";
                if (!DateTime.TryParse(duedatestr, out var duedate))
                {
                    Console.WriteLine("Invalid date. Task creation cancelled.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Select Priority: 1) Low  2) Medium  3) High");
                var pChoice = Console.ReadLine();
                TaskPriority priority = pChoice switch
                {
                    "1" => TaskPriority.Low,
                    "3" => TaskPriority.High,
                    _ => TaskPriority.Medium
                };

                var ok = taskService.CreateTask(title, desc, project.Id, uid, duedate, priority, projectService);
                Console.WriteLine(ok ? "\nTask added successfully." : "\nFailed to add task.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding task: {ex.Message}");
                Console.ReadLine();
            }
        }

        private static void EditTask(List<Tasks> projTasks, TaskService taskService)
        {
            try
            {
                if (projTasks.Count == 0)
                {
                    Console.WriteLine("No tasks to edit.");
                    Console.ReadLine();
                    return;
                }

                Console.Write("Enter Task serial number to edit: ");
                if (!int.TryParse(Console.ReadLine(), out int tNo) || tNo < 1 || tNo > projTasks.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }
                var task = projTasks[tNo - 1];

                Console.Write($"New Title (current: {task.Title}): ");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle)) task.Title = newTitle;

                Console.Write($"New Description (current: {task.Description}): ");
                var newDesc = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDesc)) task.Description = newDesc;

                Console.Write($"New Due Date (yyyy-MM-dd) (current: {task.DueDate:yyyy-MM-dd}): ");
                var newDue = Console.ReadLine();
                if (DateTime.TryParse(newDue, out var newDt)) task.DueDate = newDt;

                Console.WriteLine($"Select Priority (current: {task.Priority}): 1) Low 2) Medium 3) High");
                var pri = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(pri))
                    task.Priority = pri switch
                    {
                        "1" => TaskPriority.Low,
                        "3" => TaskPriority.High,
                        _ => TaskPriority.Medium
                    };

                Console.WriteLine($"Select Status (current: {task.Status}): 1) Todo 2) InProgress 3) Done");
                var stat = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(stat))
                    task.Status = stat switch
                    {
                        "1" => Models.TaskStatus.Todo,
                        "2" => Models.TaskStatus.InProgress,
                        "3" => Models.TaskStatus.Done,
                        _ => task.Status
                    };

                taskService.UpdateTask(task);
                Console.WriteLine("Task updated successfully. Press Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
                Console.ReadLine();
            }
        }

        private static void DeleteTask(List<Tasks> projTasks, TaskService taskService)
        {
            try
            {
                if (projTasks.Count == 0)
                {
                    Console.WriteLine("No tasks to delete.");
                    Console.ReadLine();
                    return;
                }

                Console.Write("Enter Task serial number to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int delNo) || delNo < 1 || delNo > projTasks.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }

                var tid = projTasks[delNo - 1].Id;
                var deleted = taskService.DeleteTask(tid);
                Console.WriteLine(deleted ? "Task deleted successfully." : "Failed to delete task.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
}
