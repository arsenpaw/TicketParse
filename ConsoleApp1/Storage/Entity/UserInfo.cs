namespace ConsoleApp1.Storage.Entity;

public class UserInfo
{
    public UserInfo(long telegramId, List<int> offices)
    {
        Id = Guid.NewGuid();
        TelegramId = telegramId;
        Offices =  offices;
    }

    public Guid Id { get; set; } = new();
    
    public long TelegramId { get; set; }
    
    public List<int> Offices { get; set; }
}