namespace TasksApi.Models.Response
{
    public class GetTasksResponse : BaseResponse
    {
        public List<Models.Entities.Task> Tasks { get; set; }
    }
}
