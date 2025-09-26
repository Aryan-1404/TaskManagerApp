using Services;
using System;
using System.Linq;
using TaskStatus = Models.TaskStatus;

namespace TaskManagerApp
{
    public static class ReportGenerator
    {
        public static void ReportMenu(TaskService taskService, UserService userService, ProjectService projectService)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=======================================");
                    Console.WriteLine("              REPORTS MENU             ");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("1) Show All Overdue Tasks");
                    Console.WriteLine("2) Group Tasks by User");
                    Console.WriteLine("3) Group Tasks by Project");
                    Console.WriteLine("4) Back to Admin Menu");
                    Console.WriteLine("=======================================");
                    Console.Write("Choose an option: ");

                    var ch = Console.ReadLine()?.Trim();

                    switch (ch)
                    {
                        case "1":
                            ShowOverdueTasks(taskService, userService);
                            break;

                        case "2":
                            GroupTasksByUser(taskService, userService);
                            break;

                        case "3":
                            GroupTasksByProject(taskService, projectService, userService);
                            break;

                        case "4":
                            return;

                        default:
                            Console.WriteLine("\nInvalid option! Press Enter to continue...");
                            Console.ReadLine();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError in Report Menu: {ex.Message}");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }


        public static void GroupTasksByProject(TaskService taskService, ProjectService projectService, UserService userService)
        {
            Console.Clear();
            Console.WriteLine("----- Tasks Grouped by Project -----");

            //answer-------------------

            var groupTaskByProject = taskService.GetAllTasks().GroupBy(x => x.ProjectId);
            foreach (var group in groupTaskByProject)
            {
                var project = projectService.GetProjectById(group.Key);
                Console.WriteLine($"\nProject: {project?.Name ?? "Unknown"}");

                foreach (var task in group)
                {
                    var user = userService.GetUserById(task.AssignedUserId);
                    Console.WriteLine($"   Task: {task.Title} | Assigned To: {user.Username} | Due: {task.DueDate} | Status: {task.Status}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();


        }

        public static void GroupTasksByUser(TaskService taskService, UserService userService)
        {
            //Group tasks by User.
            Console.Clear();
            Console.WriteLine("----- Tasks Grouped by User -----");
            //answer ----------------------------------
            var TaskByUser = taskService.GetAllTasks().GroupBy(x => x.AssignedUserId);
            foreach (var group in TaskByUser)
            {
                var user = userService.GetUserById(group.Key);
                Console.WriteLine($"\nUser: {user?.Username ?? "Unassigned"}");

                foreach (var task in group)
                {
                    Console.WriteLine($"   Task: {task.Title} | Due: {task.DueDate} | Status: {task.Status}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public static void ShowOverdueTasks(TaskService taskService, UserService userService)
        {
            //Show all overdue tasks (tasks past their due date and not marked as Done).
            Console.Clear();
            Console.WriteLine("----- Overdue Tasks -----");
            //answer ---------------------------------------
            var overdueTasks = taskService.GetAllTasks().Where(x => x.DueDate < DateTime.Now && x.Status != TaskStatus.Done);
            if (!overdueTasks.Any())
            {
                Console.WriteLine("No overdue tasks found.");
            }
            else
            {
                foreach (var task in overdueTasks)
                {
                    var user = userService.GetUserById(task.AssignedUserId);
                    Console.WriteLine($"Task: {task.Title} | Due: {task.DueDate} | Status: {task.Status} | Assigned To: {user.Username}");
                }
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();

        }
    }
}
