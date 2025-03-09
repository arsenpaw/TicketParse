using ConsoleApp1.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ConsoleApp1.Services;

public class AppSettingRuntimeChangerService
{
    private readonly IConfigurationRoot _configurationRoot;
    
    public AppSettingRuntimeChangerService(IConfigurationRoot configurationRoot)
    {
        _configurationRoot = configurationRoot;
    }
    private string filePath = "appsettings.json";
    private FullAplicationSettings ReadAppsettings()
    {
        var json =  File.ReadAllText(filePath);
        var jsonObj = JsonConvert.DeserializeObject<FullAplicationSettings>(json);
        if (jsonObj == null)
        {
            throw new InvalidDataException("Invalid appsettings.json file.");
        }
        return jsonObj;

    }
    
    private void SaveAppsettins(object json)
    {
        var output = JsonConvert.SerializeObject(json, Formatting.Indented);
        File.WriteAllText(filePath, output);
        _configurationRoot.Reload();
    }
    public void SetRsrfToken(string token)
    {
        var appSettings = ReadAppsettings();
        appSettings.Credentials.XCsrfToken = token;
        SaveAppsettins(appSettings);
        OnApplicationCredentialsChange?.Invoke();
    }
    public void SetCookie(string cookie)
    {
        var appSettings = ReadAppsettings();
        appSettings.Credentials.Cookie = cookie;
        SaveAppsettins(appSettings);
        OnApplicationCredentialsChange?.Invoke();
    }

    public void SetDateTime(int searchDuration, DateTime? startDate)
    {
        var appSettings = ReadAppsettings();
        appSettings.ApplicationRules.StartSearchingDate = startDate;
        appSettings.ApplicationRules.DaysToSearch = searchDuration;
        SaveAppsettins(appSettings);
    }
    
    public event Action OnApplicationCredentialsChange;
}