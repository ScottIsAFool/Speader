using Insta.Portable.Models;
using PocketSharp;
using PocketSharp.Models;
using Readability.Models;
using Speader.Interfaces;

namespace Speader.Design
{
    public class AuthenticationServiceDesign : IAuthenticationService
    {
        public void StartService()
        {
        }

        public bool IsLoggedInToPocket { get; private set; }
        public PocketUser LoggedInPocketUser { get; set; }
        public UserProfile LoggedInReadabilityUser { get; set; }
        public bool IsLoggedIntoReadability { get; private set; }
        public User LoggedInInstapaperUser { get; set; }
        public bool IsLoggedIntoInstapaper { get { return true; } }
        public void SetPocketUser(PocketUser user)
        {
        }

        public void SetPocketUser(PocketUser user, bool saveToDisk)
        {
        }

        public void SetPocketAccessToken(IPocketClient client)
        {
        }

        public void SignOutFromPocket()
        {
        }

        public void SetReadabilityUser(UserProfile user, string token, string secret)
        {
        }

        public void SignOutFromReadability()
        {
        }

        public void SetInstapaperUser(User user, string token, string secret)
        {
        }

        public void SignOutFromInstapaper()
        {
        }

        public void SignOutFromAll()
        {
        }
    }
}