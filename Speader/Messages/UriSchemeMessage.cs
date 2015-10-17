using GalaSoft.MvvmLight.Messaging;
using Speader.Model;

namespace Speader.Messages
{
    public class UriSchemeMessage : MessageBase
    {
        public UriSchemeMessage(string content, bool isUri, SchemeType schemeType)
        {
            Content = content;
            IsUri = isUri;
            SchemeType = schemeType;
        }

        public string Content { get; set; }
        public bool IsUri { get; set; }
        public SchemeType SchemeType { get; set; }
    }
}