using TasksApi.Models.Response;

namespace TasksApi.Models
{
    public interface ITaskService
    {
        Task<GetTasksResponse> GetTasks(int userId);
        Task<SaveTaskResponse> SaveTask(Models.Entities.Task task);
        Task<DeleteTaskResponse> DeleteTask(int taskId, int userId);
    }
}
