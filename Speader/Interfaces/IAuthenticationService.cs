using Insta.Portable.Models;
using PocketSharp;
using PocketSharp.Models;
using Readability.Models;

namespace Speader.Interfaces
{
    public interface IAuthenticationService
    {
        void StartService();
        bool IsLoggedInToPocket { get; }
        PocketUser LoggedInPocketUser { get; set; }
        UserProfile LoggedInReadabilityUser { get; set; }
        bool IsLoggedIntoReadability { get; }
        User LoggedInInstapaperUser { get; set; }
        bool IsLoggedIntoInstapaper { get; }
        void SetPocketUser(PocketUser user);
        void SetPocketUser(PocketUser user, bool saveToDisk);
        void SetPocketAccessToken(IPocketClient client);
        void SignOutFromPocket();
        void SetReadabilityUser(UserProfile user, string token, string secret);
        void SignOutFromReadability();
        void SetInstapaperUser(User user, string token, string secret);
        void SignOutFromInstapaper();
        void SignOutFromAll();
    }
}