using Windows.UI.Xaml;

namespace Speader.Extensions
{
    public static class ApplicationExtensions
    {
        public static T GetThemeResource<T>(this Application app, string key) where T :class 
        {
            var rd = (ResourceDictionary)app.Resources.ThemeDictionaries["Dark"];
            var obj = rd[key] as T;

            return obj;
        }
    }
}