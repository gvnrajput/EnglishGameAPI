using EnglishGameAPI.Models;
using System.Text.Json;

namespace EnglishGameAPI.Services
{
    public class JsonProfileDataService : IProfileDataService
    {
        private const string _profilesFilePath = "Data/profiles.json";
        private readonly object _fileLock = new object();

        public Profile GetProfile(string username)
        {
            var profiles = LoadProfiles();
            return profiles.FirstOrDefault(p => p.Username == username);
        }

        public void AddProfile(Profile profile)
        {
            lock (_fileLock)
            {
                var profiles = LoadProfiles();
                if (profiles.Any(p => p.Username == profile.Username))
                {
                    throw new System.Exception("Username already exists");
                }

                profile.LastUpdated = DateTime.UtcNow;
                profiles.Add(profile);
                SaveProfiles(profiles);
            }
        }

        public void UpdateProfile(Profile profile)
        {
            lock (_fileLock)
            {
                var profiles = LoadProfiles();
                var existingProfile = profiles.FirstOrDefault(p => p.Username == profile.Username);

                if (existingProfile == null)
                {
                    throw new System.Exception("Profile not found");
                }

                existingProfile.Email = profile.Email;
                existingProfile.PasswordHash = profile.PasswordHash;
                existingProfile.LastUpdated = DateTime.UtcNow;
                SaveProfiles(profiles);
            }
        }

        private List<Profile> LoadProfiles()
        {
            lock (_fileLock)
            {
                if (!File.Exists(_profilesFilePath))
                {
                    return new List<Profile>();
                }

                string jsonData = File.ReadAllText(_profilesFilePath);
                return JsonSerializer.Deserialize<List<Profile>>(jsonData) ?? new List<Profile>();
            }
        }

        private void SaveProfiles(List<Profile> profiles)
        {
            lock (_fileLock)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonData = JsonSerializer.Serialize(profiles, options);
                Directory.CreateDirectory(Path.GetDirectoryName(_profilesFilePath));
                File.WriteAllText(_profilesFilePath, jsonData);
            }
        }
    }
}
