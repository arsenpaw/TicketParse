using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ConsoleApp1.Config;

public class ApplicationCredentials
{

    public string Cookie { get; set; }

    public string UserAgent { get; set; }
    public string XCsrfToken { get; set; }
    
    public string XRequestedWith { get; set; }

    [JsonPropertyName("referer")]
    public string Referer { get; set; }
}