using Microsoft.EntityFrameworkCore;
using TaskManager.API.Models;

namespace TaskManager.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TaskDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Tasks.AnyAsync(cancellationToken))
        {
            return;
        }

        var seedTasks = new List<TaskItem>
        {
            new()
            {
                Title = "Learn Docker",
                Description = "Learn Docker fundamentals and containerization basics",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Learn Jenkins",
                Description = "Set up automated build pipelines",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Learn Kubernetes",
                Description = "Master cluster architecture and deployments",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Build DevOps project",
                Description = "Complete all stages from local app to GitOps delivery",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Tasks.AddRangeAsync(seedTasks, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
