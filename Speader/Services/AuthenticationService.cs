using AsyncOAuth;
using Cimbalino.Toolkit.Services;
using Insta.Portable;
using Insta.Portable.Models;
using Newtonsoft.Json;
using PocketSharp;
using PocketSharp.Models;
using PropertyChanged;
using Readability;
using Readability.Models;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Services
{
    [ImplementPropertyChanged]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IReadabilityClient _readabilityClient;
        private readonly IPocketClient _pocketClient;
        private readonly IInstapaperClient _instapaperClient;
        private readonly IApplicationSettingsServiceHandler _roamingSettings;
        //private readonly ApplicationDataContainer _roamingStorage;


        public AuthenticationService(
            IReadabilityClient readabilityClient,
            IPocketClient pocketClient,
            IInstapaperClient instapaperClient,
            IApplicationSettingsServiceHandler roamingSettings)
        {
            //_roamingStorage = ApplicationData.Current.RoamingSettings;
            _readabilityClient = readabilityClient;
            _pocketClient = pocketClient;
            _instapaperClient = instapaperClient;
            _roamingSettings = roamingSettings;
        }

        public void StartService()
        {
            GetPocketUser();
            GetReadabilityUser();
            GetInstapaperUser();
        }

        #region Pocket Methods/Properties
        public bool IsLoggedInToPocket => LoggedInPocketUser != null;

        public PocketUser LoggedInPocketUser { get; set; }

        public void SetPocketUser(PocketUser user)
        {
            SetPocketUser(user, true);
        }

        public void SetPocketUser(PocketUser user, bool saveToDisk)
        {
            LoggedInPocketUser = user;
            _pocketClient.AccessCode = LoggedInPocketUser.Code;

            if (saveToDisk)
            {
                var json = JsonConvert.SerializeObject(LoggedInPocketUser);
                WriteAllTextAsync(Constants.StorageSettings.PocketUserFile, json);
            }
        }

        public void SetPocketAccessToken(IPocketClient client)
        {
            if (IsLoggedInToPocket && !string.IsNullOrEmpty(LoggedInPocketUser.Code))
            {
                client.AccessCode = LoggedInPocketUser.Code;
            }
        }

        public void SignOutFromPocket()
        {
            LoggedInPocketUser = null;
            if (FileExistsAsync(Constants.StorageSettings.PocketUserFile))
            {
                DeleteFileAsync(Constants.StorageSettings.PocketUserFile);
            }
        }

        private void GetPocketUser()
        {
            if (FileExistsAsync(Constants.StorageSettings.PocketUserFile))
            {
                var json = ReadAllTextAsync(Constants.StorageSettings.PocketUserFile);
                var user = JsonConvert.DeserializeObject<PocketUser>(json);

                if (user != null)
                {
                    SetPocketUser(user, false);
                }
            }
        }
        #endregion

        #region Readability Methods/Properties
        public UserProfile LoggedInReadabilityUser { get; set; }
        public bool IsLoggedIntoReadability => LoggedInReadabilityUser != null;

        public void SetReadabilityUser(UserProfile user, string token, string secret)
        {
            SetReadabilityUser(user, token, secret, true);
        }

        public void SignOutFromReadability()
        {
            LoggedInReadabilityUser = null;
            if (FileExistsAsync(Constants.StorageSettings.ReadabilityUserFile))
            {
                DeleteFileAsync(Constants.StorageSettings.ReadabilityUserFile);
            }
        }

        private void SetReadabilityUser(UserProfile user, string token, string secret, bool saveToDisk)
        {
            LoggedInReadabilityUser = user;
            _readabilityClient.AccessToken = new AccessToken(token, secret);

            if (saveToDisk)
            {
                var storage = new UserStorage<UserProfile>(user, token, secret);
                var json = JsonConvert.SerializeObject(storage);
                WriteAllTextAsync(Constants.StorageSettings.ReadabilityUserFile, json);
            }
        }

        private void GetReadabilityUser()
        {
            if (FileExistsAsync(Constants.StorageSettings.ReadabilityUserFile))
            {
                var json = ReadAllTextAsync(Constants.StorageSettings.ReadabilityUserFile);
                var user = JsonConvert.DeserializeObject<UserStorage<UserProfile>>(json);

                if (user != null)
                {
                    SetReadabilityUser(user.UserProfile, user.AccessToken, user.AccessSecret, false);
                }
            }
        }

        #endregion
        public User LoggedInInstapaperUser { get; set; }
        public bool IsLoggedIntoInstapaper => LoggedInInstapaperUser != null;
        public void SetInstapaperUser(User user, string token, string secret)
        {
            SetInstapaperUser(user, token, secret, true);
        }

        public void SignOutFromInstapaper()
        {
            LoggedInInstapaperUser = null;
            if (FileExistsAsync(Constants.StorageSettings.InstapaperUserFile))
            {
                DeleteFileAsync(Constants.StorageSettings.InstapaperUserFile);
            }
        }

        private void SetInstapaperUser(User user, string token, string secret, bool saveToDisk)
        {
            LoggedInInstapaperUser = user;
            _instapaperClient.AccessToken = new AccessToken(token, secret);

            if (saveToDisk)
            {
                var storage = new UserStorage<User>(user, token, secret);
                var json = JsonConvert.SerializeObject(storage);
                WriteAllTextAsync(Constants.StorageSettings.InstapaperUserFile, json);
            }
        }

        private void GetInstapaperUser()
        {
            if (FileExistsAsync(Constants.StorageSettings.InstapaperUserFile))
            {
                var json = ReadAllTextAsync(Constants.StorageSettings.InstapaperUserFile);
                var user = JsonConvert.DeserializeObject<UserStorage<User>>(json);

                if (user != null)
                {
                    SetInstapaperUser(user.UserProfile, user.AccessToken, user.AccessSecret, false);
                }
            }
        }

        public void SignOutFromAll()
        {
            SignOutFromPocket();
            SignOutFromReadability();
        }

        private bool FileExistsAsync(string filename)
        {
            var fileResponse = _roamingSettings.Get<object>(filename);
            return fileResponse != null;
        }

        private void WriteAllTextAsync(string filename, string content)
        {
            _roamingSettings.Set(filename, content);
        }

        private void DeleteFileAsync(string filename)
        {
            _roamingSettings.Remove(filename);
        }

        private string ReadAllTextAsync(string filename)
        {
            return _roamingSettings.Get<string>(filename);
        }
    }
}