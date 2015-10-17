using System;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Design
{
    public class SettingsServiceDesign : ISettingsService
    {
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
        }

        public void LoadSettings()
        {
        }

        public void SetTheme()
        {
        }
    }
}
