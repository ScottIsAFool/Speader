using System;
using Speader.Model;

namespace Speader.Interfaces
{
    public interface ISettingsService
    {
        bool UseTransparentTile { get; set; }
        int WordsPerMin { get; set; }
        int WordsAtATime { get; set; }
        Theme Theme { get; set; }
        ReaderFont ReaderFont { get; set; }
        
        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="itemTapped">The item tapped.</param>
        void ShowErrorMessage(string message, Action itemTapped = null);

        /// <summary>
        /// Hides the error message.
        /// </summary>
        void HideErrorMessage();

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        void LoadSettings();
    }
}