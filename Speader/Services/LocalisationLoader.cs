using Windows.ApplicationModel.Resources;
using Speader.Interfaces;

namespace Speader.Services
{
    public class LocalisationLoader : ILocalisationLoader
    {
        private static ResourceLoader _loader;

        private static ResourceLoader Loader => _loader ?? (_loader = new ResourceLoader());

        public string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return Loader.GetString(key);
        }
    }
}