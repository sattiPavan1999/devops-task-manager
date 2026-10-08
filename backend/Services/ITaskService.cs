using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> UpdateTaskAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}
