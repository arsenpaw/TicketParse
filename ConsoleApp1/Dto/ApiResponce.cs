using Newtonsoft.Json;

namespace ConsoleApp1.Dto;

public class GovTicketResponse
{
    [JsonProperty("rows")]
    public List<Ticket> Rows { get; set; } 

    [JsonProperty("trows")]
    public List<object> TRows { get; set; }  
    [JsonProperty("freedatesforoffice")]
    public List<Freedate> FreedatesForOffice { get; set; }
}