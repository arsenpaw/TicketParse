using System.Text.Json.Serialization;

namespace ConsoleApp1.Config;

public class ApplicationRules
{
    [JsonPropertyName("StartSerachingDate")]
    public DateTime? StartSearchingDate { get; set; }

    [JsonPropertyName("DaysToSearch")]
    public int DaysToSearch { get; set; }

    [JsonPropertyName("OfficeIds")]
    public List<int> OfficeIds { get; set; }

    [JsonPropertyName("OfficeId")]
    public float MinutesRequestDelay { get; set; }


}