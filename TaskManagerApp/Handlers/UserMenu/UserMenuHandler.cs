using Models;
using Services;
using System;
using System.Linq;
using TaskStatus = Models.TaskStatus;

namespace TaskManagerApp
{
    public static class UserMenuHandler
    {
        // User Menu
        public static void UserMenu(TaskService taskService, UserService userService, User user, ProjectService projectService, AuthServices authServices)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=======================================");
                    Console.WriteLine($"         USER MENU - {user.Username}    ");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("1) View My Projects and Tasks");
                    Console.WriteLine("2) Update Task Status");
                    Console.WriteLine("3) Change Password");
                    Console.WriteLine("4) Logout");
                    Console.WriteLine("=======================================");
                    Console.Write("Choose an option: ");

                    var choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            try
                            {
                                ViewUserProjectsAndTasks(taskService, projectService, user);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error viewing projects/tasks: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "2":
                            try
                            {
                                UpdateUserTaskStatus(taskService, projectService, user);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error updating task status: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "3":
                            try
                            {
                                UtilityClass.ChangeUserPassword(authServices, user);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error changing password: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "4":
                            Console.WriteLine("\nLogging out...");
                            Console.WriteLine("Press Enter to return to Main Menu...");
                            Console.ReadLine();
                            return;

                        default:
                            Console.WriteLine("\nInvalid option! Press Enter to continue...");
                            Console.ReadLine();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error in User Menu: {ex.Message}");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }


        // View project and task
        public static void ViewUserProjectsAndTasks(TaskService taskService, ProjectService projectService, User user)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Your Projects and Tasks -----");

                var projects = projectService.GetProjectsByUserId(user.Id);
                if (projects.Count == 0)
                {
                    Console.WriteLine("No projects assigned.");
                }
                else
                {
                    for (int i = 0; i < projects.Count; i++)
                    {
                        var project = projects[i];
                        Console.WriteLine($"\n{i + 1}) Project: {project.Name}");

                        var projectTasks = taskService.GetTasksForUser(user.Id)
                                                      .Where(t => t.ProjectId == project.Id)
                                                      .ToList();

                        if (projectTasks.Count == 0)
                            Console.WriteLine("   No tasks assigned for this project.");
                        else
                        {
                            for (int j = 0; j < projectTasks.Count; j++)
                            {
                                var task = projectTasks[j];
                                Console.WriteLine($"   {j + 1}. {task.Title} | Status: {task.Status} | Priority: {task.Priority} | Due: {task.DueDate:yyyy-MM-dd}");
                            }
                        }
                    }
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying projects and tasks: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        // Update task status
        public static void UpdateUserTaskStatus(TaskService taskService, ProjectService projectService, User user)
        {
            try
            {
                var tasks = taskService.GetTasksForUser(user.Id);
                if (tasks.Count == 0)
                {
                    Console.WriteLine("No tasks to update.");
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nSelect Task to update status:");
                for (int i = 0; i < tasks.Count; i++)
                {
                    var project = projectService.GetProjectById(tasks[i].ProjectId);
                    string projectName = project != null ? project.Name : "No Project";
                    Console.WriteLine($"{i + 1}) {tasks[i].Title} | Project: {projectName} | Status: {tasks[i].Status} | Priority: {tasks[i].Priority} | Due: {tasks[i].DueDate:yyyy-MM-dd}");
                }

                Console.Write("Enter Task serial number: ");
                if (int.TryParse(Console.ReadLine(), out int taskNo) && taskNo >= 1 && taskNo <= tasks.Count)
                {
                    var task = tasks[taskNo - 1];

                    Console.WriteLine("\nSelect new status:");
                    Console.WriteLine("1) Todo");
                    Console.WriteLine("2) InProgress");
                    Console.WriteLine("3) Done");
                    Console.Write("Enter choice: ");
                    var statusChoice = Console.ReadLine()?.Trim();

                    task.Status = statusChoice switch
                    {
                        "1" => TaskStatus.Todo,
                        "2" => TaskStatus.InProgress,
                        "3" => TaskStatus.Done,
                        _ => task.Status
                    };

                    taskService.UpdateTask(task);
                    Console.WriteLine("\nTask status updated successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task status: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

    }
}
