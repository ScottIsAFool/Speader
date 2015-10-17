namespace Speader
{
    public static class Constants
    {
        public const int MaxArticleCount = 100;
        public const int MinWordsAtATime = 1;
        public const int MaxWordsAtATime = 4;
        public const int MinWordsPerMinute = 100;
        public const int MaxWordsPerMinute = 1000;

        public static class Tiles
        {
            public const string LocalId = "Speader.Local";
            public const string PocketId = "Speader.Pocket";
            public const string ReadabilityId = "Speader.Readability";
            public const string InstapaperId = "Speader.Instapaper";
        }

        public static class UriScheme
        {
            public const string Scheme = "speader://";
            public const string Read = Scheme + "read";
            public const string Save = Scheme + "save";
            public const string Edit = Scheme + "edit";
        }

        public static class ThemeFiles
        {
            public const string ThemeFileFormat = "ms-appx:///Themes/{0}.xaml";
            public const string LightTheme = "ms-appx:///Themes/Light.xaml";
            public const string DarkTheme = "ms-appx:///Themes/Dark.xaml";
            public const string SepiaTheme = "ms-appx:///Themes/Sepia.xaml";
        }

        public static class StorageSettings
        {
            public const string PocketUserFile = "PocketUser";
            public const string ReadabilityUserFile = "ReadabilityUser";
            public const string InstapaperUserFile = "InstapaperUser";
            public const string SettingsFile = "Settings";
        }

        public static class Pocket
        {
            public const string PocketConsumerKey = "46723-f6cfa4007b4d91e49d179a15"; 
            public const string CallBackUri = "speader:PocketAuthorise"; 
        }

        public static class Readability
        {
            public const string ConsumerKey = "scottisafool";
            public const string ConsumerSecret = "Gy6vJLFdwZsPGfrN5yPW9VvWKrnY6ZSR";
            public const string CallBackUri = "speader:ReadabilityAuthorise";
        }

        public static class Instapaper
        {
            public const string ConsumerKey = "5828dcadb4844d6797b5b763a9f22b38";
            public const string ConsumerSecret = "a8786104f27c46f790264a13c3f7bb40";
            public const string CallBackUri = "speader:InstapaperAuthorise";
        }

        public static class Messages
        {
            public const string PocketAuthMsg = "PocketAuthMsg";
            public const string ReadabilityAuthMsg = "ReadabilityAuthMsg";
            public const string SaveSettingsMsg = "SaveSettingsMsg";
            public const string ReaderViewLeftMsg = "ReaderViewLeftMsg";
            public const string StopPullToRefreshMsg = "StopPullToRefreshMsg";
            public const string ReloadInProgressMsg = "ReloadInProgressMsg";
            public const string ResetWebMsg = "ResetWebMsg";
            public const string RefreshPinMsg = "RefreshPinMsg";
        }
    }
}
