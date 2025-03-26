using EnglishGameAPI.Models;

namespace EnglishGameAPI.Services
{
    public interface IProfileDataService
    {
        Profile GetProfile(string username);
        void AddProfile(Profile profile);
        void UpdateProfile(Profile profile);
    }
}
