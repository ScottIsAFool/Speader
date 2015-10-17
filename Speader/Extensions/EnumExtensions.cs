using System;
using System.Collections.Generic;
using System.Linq;
using Speader.Model;

namespace Speader.Extensions
{
    public static class EnumExtensions
    {
        public static List<T> ToList<T>(this Array array)
        {
            return (from object item in array select (T)item).ToList();
        }

        public static string ToLocalisedSource(this SourceProvider source)
        {
            switch (source)
            {
                case SourceProvider.Local:
                    return App.Loader.GetString("Local");
                default:
                    return source.ToString();
            }
        }
    }
}
