using ConsoleApp1.Command;

namespace ConsoleApp1.Commands.KeyboardCommand;

public static class KeyboardCommandResolver
{
    //Todo Bad way to handle this, should be refactored
    private static readonly Dictionary<string, string> _dictionary = new()
    {
        {"Check",CommandType.CheckCommand},
        {"Unsubscribe",CommandType.EndCommand},
        {"Home",CommandType.HomeCommand},
        {"Select office",CommandType.OfficeCommand}
        
    };
    public static string? Resolve(string keyboardInput)
    {
        return _dictionary.GetValueOrDefault(keyboardInput);
    }
}