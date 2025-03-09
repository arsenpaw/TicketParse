using Newtonsoft.Json;

namespace ConsoleApp1.Dto;

public  class Ticket
{
    [JsonProperty("chtime")]
    public DateTime Time { get; set; }
}