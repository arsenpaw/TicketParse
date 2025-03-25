using Newtonsoft.Json;

namespace ConsoleApp1.Dto;

public  class Ticket
{
    public DateTime Time { get; set; }
    
   public string Location { get; set; } = string.Empty;
   
   public int OfficeId { get; set; }
}