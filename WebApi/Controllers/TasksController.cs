using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksApi.Models;
using TasksApi.Models.Requests;
using TasksApi.Models.Response;

namespace TasksApi.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    public class TasksController : BaseApiController
    {
        private readonly ITaskService taskService;
 
        public TasksController(ITaskService taskService)
        {
            this.taskService = taskService;
        }
 
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            GetTasksResponse getTasksResponse = await taskService.GetTasks(UserID);
 
            if (!getTasksResponse.Success)
            {
                return UnprocessableEntity(getTasksResponse);
            }
            
            IEnumerable<TaskResponse> tasksResponse = getTasksResponse.Tasks.ConvertAll(o => 
                new TaskResponse { Id = o.Id, IsCompleted = o.IsCompleted, Name = o.Name, Ts = o.Ts });
 
            return Ok(tasksResponse);
        }
 
        [HttpPost]
        public async Task<IActionResult> Post(TaskRequest taskRequest)
        {
            Models.Entities.Task task = new Models.Entities.Task { IsCompleted = taskRequest.IsCompleted, 
                Ts = taskRequest.Ts, Name = taskRequest.Name, UserId = UserID };
 
            SaveTaskResponse saveTaskResponse = await taskService.SaveTask(task);
 
            if (!saveTaskResponse.Success)
            {
                return UnprocessableEntity(saveTaskResponse);
            }
 
            TaskResponse taskResponse = new TaskResponse { Id = saveTaskResponse.Task.Id, 
                IsCompleted = saveTaskResponse.Task.IsCompleted, Name = saveTaskResponse.Task.Name, 
                Ts = saveTaskResponse.Task.Ts };
            
            return Ok(taskResponse);
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            DeleteTaskResponse deleteTaskResponse = await taskService.DeleteTask(id, UserID);
            
            if (!deleteTaskResponse.Success)
            {
                return UnprocessableEntity(deleteTaskResponse);
            }
 
            return Ok(deleteTaskResponse.TaskId);
        }
    }
}
