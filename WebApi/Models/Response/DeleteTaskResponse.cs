using System.Text.Json.Serialization;

namespace TasksApi.Models.Response
{
    public class DeleteTaskResponse : BaseResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TaskId { get; set; }
    }
}
