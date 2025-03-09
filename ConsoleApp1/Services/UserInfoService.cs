using ConsoleApp1.Storage;
using ConsoleApp1.Storage.Entity;

namespace ConsoleApp1.Services;

public class UserInfoService
{
    public readonly UserInfoRepository UserInfoRepository;

    public UserInfoService(UserInfoRepository userInfoRepository)
    {
        UserInfoRepository = userInfoRepository;

    }

    public bool RemoveUser(long telegramId)
    {
        var userInfo = UserInfoRepository.GetByTelegramId(telegramId);
        if (userInfo is null)
        {
            throw new ApplicationException("User not found");
        }
        return UserInfoRepository.Delete(userInfo.Id);
    }
    public async Task AddUser(long telegramId, List<int> offices)
    {
        var oldUserInfo = UserInfoRepository.GetByTelegramId(telegramId);
        if (oldUserInfo is not null)
        {
            throw new ApplicationException("User already exists");
        }
        UserInfoRepository.Insert(new UserInfo(telegramId, offices));
    }
    public async Task SetUserOffices(long telegramId, List<int> offices)
    {
        var oldUserInfo = UserInfoRepository.GetByTelegramId(telegramId);
        if (oldUserInfo is null)
        {
            throw new ApplicationException("User not registered");
        }
        UserInfoRepository.SetOffices(oldUserInfo.Id, offices);
    }
    public List<UserInfo> GetAllUsers()
    {
        return UserInfoRepository.GetAll();
    }
}