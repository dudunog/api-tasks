namespace TasksApi.Models.Requests
{
    public class TaskRequest
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime Ts { get; set; }
    }
}
