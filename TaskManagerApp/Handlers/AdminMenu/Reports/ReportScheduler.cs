using Models;
using Services;
using System;
using System.Linq;
using System.Threading;

public static class ReportScheduler
{
    private static Timer? _timer;
    private static readonly int intervalSeconds = 10; // Time interval for scheduler

    public static void ReportSchedulerMenu(TaskService taskService, ProjectService projectService)
    {
        while (true)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=======================================");
                Console.WriteLine("        REPORT SCHEDULER MENU          ");
                Console.WriteLine("=======================================");
                Console.WriteLine("1) Start Scheduler");
                Console.WriteLine("2) Stop Scheduler");
                Console.WriteLine("3) Back to Admin Menu");
                Console.WriteLine("=======================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        Start(taskService, projectService, intervalSeconds);
                        break;
                    case "2":
                        Stop();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError in Report Scheduler Menu: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
    }

    public static void Start(TaskService taskService, ProjectService projectService, int intervalSeconds)
    {
        try
        {
            if (_timer != null)
            {
                Console.WriteLine("Scheduler is already running.");
                return;
            }

            _timer = new Timer(state =>
            {
                try
                {
                    ShowOverdueSummary(taskService, projectService);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[Scheduler Error] {ex.Message}");
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds));

            Console.WriteLine($"Scheduler started. It will show overdue tasks every {intervalSeconds} seconds.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start scheduler: {ex.Message}");
        }
    }

    public static void Stop()
    {
        try
        {
            if (_timer == null)
            {
                Console.WriteLine("Scheduler is not running.");
                return;
            }

            _timer.Dispose();
            _timer = null;
            Console.WriteLine("Scheduler stopped.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop scheduler: {ex.Message}");
        }
    }

    private static void ShowOverdueSummary(TaskService taskService, ProjectService projectService)
    {
        try
        {
            var overdueTasks = taskService.GetAllTasks()
                .Where(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Done)
                .ToList();

            if (!overdueTasks.Any()) return;

            var projectCount = overdueTasks.Select(t => t.ProjectId).Distinct().Count();
            Console.WriteLine($"\n[Scheduler] {overdueTasks.Count} tasks are overdue across {projectCount} projects.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while generating overdue summary: {ex.Message}");
        }
    }
}
