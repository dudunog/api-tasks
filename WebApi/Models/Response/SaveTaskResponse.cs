namespace TasksApi.Models.Response
{
    public class SaveTaskResponse : BaseResponse
    {
        public Models.Entities.Task Task { get; set; }
    }
}
