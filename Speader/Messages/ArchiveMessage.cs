using System;
using GalaSoft.MvvmLight.Messaging;
using Speader.Model;

namespace Speader.Messages
{
    public class ArchiveMessage : MessageBase
    {
        public ArchiveMessage(SourceProvider source, ReaderItem item, Action action)
        {
            SourceProvider = source;
            Item = item;
            Action = action;
        }

        public SourceProvider SourceProvider { get; set; }
        public ReaderItem Item { get; set; }
        public Action Action { get; set; }
    }
}