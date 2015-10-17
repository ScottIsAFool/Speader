using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PropertyChanged;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Model;
using ThemeManagerRt;

namespace Speader.Services
{
    [ImplementPropertyChanged]
    public class SettingsService : ISettingsService
    {
        private readonly IApplicationSettingsServiceHandler _roamingSettings;

        public SettingsService()
        {
            _roamingSettings = SimpleIoc.Default.GetInstance<IApplicationSettingsServiceHandler>();
            WordsPerMin = 200;
            WordsAtATime = 1;
        }

        public bool UseTransparentTile { get; set; }
        public int WordsPerMin { get; set; }
        public int WordsAtATime { get; set; }
        public Theme Theme { get; set; }
        public ReaderFont ReaderFont { get; set; }

        public void ShowErrorMessage(string message, Action itemTapped = null)
        {
        }

        public void HideErrorMessage()
        {
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            _roamingSettings.Set(Constants.StorageSettings.SettingsFile, json);
        }

        public void LoadSettings()
        {
            var json = _roamingSettings.Get(Constants.StorageSettings.SettingsFile, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var settings = JsonConvert.DeserializeObject<SettingsService>(json);
            settings.CopyItem(this);
        }

        [UsedImplicitly]
        private void OnThemeChanged()
        {
            ThemeManager.ChangeTheme(new Uri(string.Format(Constants.ThemeFiles.ThemeFileFormat, Theme)));
        }

        [UsedImplicitly]
        private async void OnUseTransparentTileChanged()
        {
            
        }
    }
}