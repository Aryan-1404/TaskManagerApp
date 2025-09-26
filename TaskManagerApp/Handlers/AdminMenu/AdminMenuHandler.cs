using Helpers;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagerApp.Handlers.AdminMenu;

namespace TaskManagerApp
{
    public static class AdminMenuHandler
    {
        // Admin Menu
        public static void AdminMenu(UserService userService, User admin, ProjectService projectService, TaskService taskService, AuthServices authServices)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=======================================");
                    Console.WriteLine($"        ADMIN MENU - {admin.Username}   ");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("1) User Management");
                    Console.WriteLine("2) Project Management");
                    Console.WriteLine("3) Task Management for Projects");
                    Console.WriteLine("4) View My Tasks");
                    Console.WriteLine("5) Update Status of My Tasks");
                    Console.WriteLine("6) Change My Password");
                    Console.WriteLine("7) ReportGeneration Of Query's");
                    Console.WriteLine("8) Report Scheduler");
                    Console.WriteLine("9) Logout");
                    Console.WriteLine("=======================================");
                    Console.Write("Choose an option: ");

                    var ch = Console.ReadLine()?.Trim();

                    switch (ch)
                    {
                        case "1":
                            try
                            {
                                AdminUserManagementMenu.UserManagementMenu(userService, authServices, admin);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in User Management: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "2":
                            try
                            {
                                AdminProjectManagementMenu.ProjectManagementMenu(projectService, userService, taskService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Project Management: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "3":
                            try
                            {
                                AdminTaskManagementMenu.TaskManagementMenu(projectService, taskService, userService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Task Management: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "4":
                            try
                            {
                                ViewAdminProjectsAndTasks(admin, taskService, projectService, userService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error viewing tasks: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "5":
                            try
                            {
                                UpdateMyTaskStatus(admin, taskService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error updating task status: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "6":
                            try
                            {
                                UtilityClass.ChangeUserPassword(authServices, admin);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error changing password: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "7":
                            try
                            {
                                ReportGenerator.ReportMenu(taskService, userService, projectService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error generating report: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "8":
                            try
                            {
                                ReportScheduler.ReportSchedulerMenu(taskService, projectService);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Report Scheduler: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case "9":
                            try
                            {
                                ReportScheduler.Stop();
                                Console.WriteLine("\nLogging Out...");
                                Console.WriteLine("Press Enter to return to Main Menu...");
                                Console.ReadLine();
                                return;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error during logout: {ex.Message}");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                            break;

                        default:
                            Console.WriteLine("\nInvalid option! Press Enter to continue...");
                            Console.ReadLine();
                            break;
                    }
                }
                catch (Exception ex)
                {                  
                    Console.WriteLine($"\nUnexpected error: {ex.Message}");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }


        // View my task
        public static void ViewAdminProjectsAndTasks(User admin, TaskService taskService, ProjectService projectService, UserService userService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Your Projects and Tasks (Admin) -----");

                // Get only projects assigned to this admin
                var projects = projectService.GetProjectsByUserId(admin.Id);
                if (projects.Count == 0)
                {
                    Console.WriteLine("No projects assigned to you.");
                }
                else
                {
                    for (int i = 0; i < projects.Count; i++)
                    {
                        var project = projects[i];
                        Console.WriteLine($"\n{i + 1}) Project: {project.Name}");

                        // Get tasks under this project
                        var projectTasks = taskService.GetTasksForUser(admin.Id)
                                                      .Where(t => t.ProjectId == project.Id)
                                                      .ToList();

                        if (projectTasks.Count == 0)
                        {
                            Console.WriteLine("   No tasks assigned for this project.");
                        }
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



        // Update status of my task
        public static void UpdateMyTaskStatus(User user, TaskService taskService)
        {
            try
            {
                var myTasks = taskService.GetTasksForUser(user.Id);
                if (myTasks.Count == 0)
                {
                    Console.WriteLine("\nNo tasks to update. Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nSelect Task to update status:");
                for (int i = 0; i < myTasks.Count; i++)
                    Console.WriteLine($"{i + 1}) {myTasks[i].Title} | Status: {myTasks[i].Status} | Due: {myTasks[i].DueDate:yyyy-MM-dd}");

                Console.Write("Enter Task serial number: ");
                if (!int.TryParse(Console.ReadLine(), out int tNo) || tNo < 1 || tNo > myTasks.Count)
                {
                    Console.WriteLine("Invalid selection. Press Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                var task = myTasks[tNo - 1];

                Console.WriteLine("Select Status: 1) Todo 2) InProgress 3) Done");
                var stat = Console.ReadLine()?.Trim();

                task.Status = stat switch
                {
                    "1" => Models.TaskStatus.Todo,
                    "2" => Models.TaskStatus.InProgress,
                    "3" => Models.TaskStatus.Done,
                    _ => task.Status //keep same if not assigned at the time of editing
                };

                taskService.UpdateTask(task);
                Console.WriteLine("Task status updated successfully. Press Enter to continue...");
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
