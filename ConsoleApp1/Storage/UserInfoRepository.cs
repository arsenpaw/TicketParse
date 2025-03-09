using ConsoleApp1.Storage.Entity;
using LiteDB;

namespace ConsoleApp1.Storage;

public class UserInfoRepository
{
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<UserInfo> _users;

    public UserInfoRepository(LiteDatabase database)
    {
        _database = database;
        _users = _database.GetCollection<UserInfo>("users");
        _users.EnsureIndex(u => u.Id, unique: true);
        _users.EnsureIndex(u => u.TelegramId, unique: true);
    }

    public void Insert(UserInfo user)
    {
        _users.Insert(user);
    }
    public void SetOffices(Guid id, List<int> offices)
    {
        var user = _users.FindOne(u => u.Id == id);
        user.Offices = offices;
        _users.Update(user);
    }
    public UserInfo GetById(Guid id)
    {
        return _users.FindOne(u => u.Id == id);
    }
    public bool Delete(Guid id)
    {
        return _users.Delete(id);
    }
    public UserInfo GetByTelegramId(long id)
    {
        return _users.FindOne(u => u.TelegramId == id);
    }

    public List<UserInfo> GetAll()
    {
        return _users.FindAll().ToList();
    }
}