using System.Text.Json.Serialization;

namespace TasksApi.Models.Response
{
    public class BaseResponse
    {
        [JsonIgnore]
        public bool Success { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ErrorCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Error { get; set; }
    }
}
