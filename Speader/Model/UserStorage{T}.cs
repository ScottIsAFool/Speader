using AsyncOAuth;
using Newtonsoft.Json;

namespace Speader.Model
{
    public class UserStorage<T>
    {
        public UserStorage() { }

        public UserStorage(T profile, string token, string secret)
        {
            UserProfile = profile;
            AccessToken = token;
            AccessSecret = secret;
        }

        public T UserProfile { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }

        [JsonIgnore]
        public AccessToken AccessTokenObject
        {
            get
            {
                return new AccessToken(AccessToken, AccessSecret);
            }
        }
    }
}
