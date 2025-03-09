using Newtonsoft.Json;

namespace ConsoleApp1.Dto;

public class Freedate
{
    [JsonProperty("cnt")]
    public int Count { get; set; }

    [JsonProperty("chdate")]
    public string ChangeDate { get; set; } 
}
