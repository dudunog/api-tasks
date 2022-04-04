using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Models;
using TasksApi.Models.Response;

namespace TasksApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly TasksDbContext tasksDbContext;
 
        public TaskService(TasksDbContext tasksDbContext)
        {
            this.tasksDbContext = tasksDbContext;
        }
 
        public async Task<DeleteTaskResponse> DeleteTask(int taskId, int userId)
        {
            Models.Entities.Task? task = await tasksDbContext.Tasks.FindAsync(taskId);
 
            if (task is null)
            {
                return new DeleteTaskResponse
                {
                    Success = false,
                    Error = "Task not found",
                    ErrorCode = "T01"
                };
            }
 
            if (task.UserId != userId)
            {
                return new DeleteTaskResponse
                {
                    Success = false,
                    Error = "You don't have access to delete this task",
                    ErrorCode = "T02"
                };
            }
 
            tasksDbContext.Tasks.Remove(task);
 
            int saveResponse = await tasksDbContext.SaveChangesAsync();
 
            if (saveResponse >= 0)
            {
                return new DeleteTaskResponse
                {
                    Success = true,
                    TaskId = task.Id
                };
            }
 
            return new DeleteTaskResponse
            {
                Success = false,
                Error = "Unable to delete task",
                ErrorCode = "T03"
            };
        }
 
        public async Task<GetTasksResponse> GetTasks(int userId)
        {
            IEnumerable<Models.Entities.Task> tasks = await tasksDbContext.Tasks.Where(t => t.UserId == userId).ToListAsync();
 
            if (tasks.Count() == 0)
            {
                return new GetTasksResponse
                { 
                    Success = false, 
                    Error = "No tasks found for this user", 
                    ErrorCode = "T04"
                };
            }

            return new GetTasksResponse { Success = true, Tasks = tasks.ToList() };
        }
 
        public async Task<SaveTaskResponse> SaveTask(Models.Entities.Task task)
        {
            await tasksDbContext.Tasks.AddAsync(task);
 
            int saveResponse = await tasksDbContext.SaveChangesAsync();
            
            if (saveResponse >= 0)
            {
                return new SaveTaskResponse
                {
                    Success = true,
                    Task = task
                };
            }
            
            return new SaveTaskResponse
            {
                Success = false,
                Error = "Unable to save task",
                ErrorCode = "T05"
            };
        }
    }
}
