using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Handlers.AdminMenu
{
    public static class AdminProjectManagementMenu
    {
        
        public static void ProjectManagementMenu(ProjectService projectService, UserService userService, TaskService taskService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=======================================");
                Console.WriteLine("        PROJECT MANAGEMENT MENU         ");
                Console.WriteLine("=======================================");
                Console.WriteLine("1) Create Project");
                Console.WriteLine("2) Add Users to Project");
                Console.WriteLine("3) View All Projects");
                Console.WriteLine("4) Edit Project");
                Console.WriteLine("5) Delete Project");
                Console.WriteLine("6) Back to Admin Menu");
                Console.WriteLine("=======================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        CreateProject(projectService);
                        break;
                    case "2":
                        AddUsersToProject(projectService, userService);
                        break;
                    case "3":
                        ViewAllProjects(projectService, taskService, userService);
                        break;
                    case "4":
                        EditProject(projectService);
                        break;
                    case "5":
                        DeleteProject(projectService, taskService);
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("\nInvalid option! Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        

        private static void CreateProject(ProjectService projectService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Create Project -----");
                Console.Write("Project Name: ");
                var name = Console.ReadLine()?.Trim();
                var created = projectService.CreateProject(name, new List<Guid>());
                Console.WriteLine(created ? "\nProject created." : "\nFailed to create project.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating project: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void AddUsersToProject(ProjectService projectService, UserService userService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Add Users to Project -----");

                var projects = projectService.GetAllProjects();
                if (projects.Count == 0)
                {
                    Console.WriteLine("No projects available.");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < projects.Count; i++)
                    Console.WriteLine($"{i + 1}) {projects[i].Name}");
                Console.Write("Select Project serial number: ");
                if (!int.TryParse(Console.ReadLine(), out int pNo) || pNo < 1 || pNo > projects.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }
                var selectedProject = projects[pNo - 1];

                var users = userService.GetAllUser();
                for (int i = 0; i < users.Count; i++)
                    Console.WriteLine($"{i + 1}) {users[i].Username}");
                Console.Write("Enter User serial numbers (comma separated): ");
                var input = Console.ReadLine();
                List<Guid> userIds = new List<Guid>();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    foreach (var s in input.Split(','))
                    {
                        if (int.TryParse(s.Trim(), out int uNo) && uNo >= 1 && uNo <= users.Count)
                            userIds.Add(users[uNo - 1].Id);
                    }
                }

                var added = projectService.AddUserToProject(selectedProject.Id, userIds);
                Console.WriteLine(added ? "\nUsers added successfully." : "\nFailed to add users.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding users to project: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void ViewAllProjects(ProjectService projectService, TaskService taskService, UserService userService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- All Projects & Tasks -----");
                var allProjects = projectService.GetAllProjects();
                foreach (var proj in allProjects)
                {
                    Console.WriteLine($"\nProject: {proj.Name} (Id: {proj.Id})");
                    var projTasks = taskService.GetTasksForProject(proj.Id);
                    if (projTasks.Count == 0) Console.WriteLine("  No tasks yet.");
                    else
                    {
                        foreach (var t in projTasks)
                        {
                            var uname = userService.GetAllUser().FirstOrDefault(u => u.Id == t.AssignedUserId)?.Username ?? "Unknown";
                            Console.WriteLine($"  Task: {t.Title} | Assigned to: {uname} | Due: {t.DueDate:yyyy-MM-dd} | Priority: {t.Priority} | Status: {t.Status}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing projects: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void EditProject(ProjectService projectService)
        {
            try
            {
                var editProjects = projectService.GetAllProjects();
                if (editProjects.Count == 0)
                {
                    Console.WriteLine("No projects available.");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < editProjects.Count; i++)
                    Console.WriteLine($"{i + 1}) {editProjects[i].Name}");
                Console.Write("Select Project serial number to edit: ");
                if (!int.TryParse(Console.ReadLine(), out int editNo) || editNo < 1 || editNo > editProjects.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }

                var projectToEdit = editProjects[editNo - 1];
                Console.Write($"Enter new name for project '{projectToEdit.Name}': ");
                var newName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                {
                    Console.WriteLine("Project name cannot be empty. Edit cancelled.");
                    Console.ReadLine();
                    return;
                }

                var edited = projectService.UpdateProject(projectToEdit.Id, newName);
                Console.WriteLine(edited ? "Project updated successfully." : "Failed to update project.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing project: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void DeleteProject(ProjectService projectService, TaskService taskService)
        {
            try
            {
                var deleteProjects = projectService.GetAllProjects();
                if (deleteProjects.Count == 0)
                {
                    Console.WriteLine("No projects available.");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < deleteProjects.Count; i++)
                    Console.WriteLine($"{i + 1}) {deleteProjects[i].Name}");
                Console.Write("Select Project serial number to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int delNo) || delNo < 1 || delNo > deleteProjects.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    Console.ReadLine();
                    return;
                }

                var projectToDelete = deleteProjects[delNo - 1];
                Console.Write($"Are you sure you want to delete project '{projectToDelete.Name}'? (y/n): ");
                var confirm = Console.ReadLine()?.Trim().ToLower();
                if (confirm == "y")
                {
                    var projDeleted = projectService.DeleteProject(projectToDelete.Id);
                    taskService.DeleteTasksForProject(projectToDelete.Id);
                    Console.WriteLine(projDeleted ? "Project and its tasks deleted successfully." : "Failed to delete project.");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting project: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
