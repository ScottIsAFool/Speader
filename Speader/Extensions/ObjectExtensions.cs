using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Speader.Extensions
{
    public static class ObjectExtensions
    {
        internal static void CopyItem<T>(this T source, T destination) where T : class
        {
            foreach (var sourcePropertyInfo in source.GetType().GetTypeInfo().DeclaredProperties)
            {
                var destPropertyInfo = source.GetType().GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name == sourcePropertyInfo.Name);

                if (sourcePropertyInfo.CanWrite)
                {
                    destPropertyInfo.SetValue(
                    destination,
                    sourcePropertyInfo.GetValue(source, null),
                    null);
                }
            }
        }

        internal static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
