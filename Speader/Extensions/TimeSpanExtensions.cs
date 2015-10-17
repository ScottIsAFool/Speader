using System;

namespace Speader.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToFriendlyText(this TimeSpan ts)
        {
            var loader = App.Loader;
            var format = string.Empty;
            if (ts.Days > 0)
            {
                format = loader.GetString("DaysText");
                return string.Format(format, ts.Days);
            }

            if (ts.Hours > 0)
            {
                format = loader.GetString("HoursText");
                return string.Format(format, ts.Hours);
            }

            if (ts.Minutes > 0)
            {
                format = loader.GetString("MinutesText");
                return string.Format(format, ts.Minutes);
            }

            if (ts.Seconds > 0)
            {
                format = loader.GetString("SecondsText");
                return string.Format(format, ts.Seconds);
            }

            format = loader.GetString("SecondsText");
            return string.Format(format, 0);
        }
    }
}
