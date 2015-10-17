using Cimbalino.Toolkit.Services;

namespace Speader.Design
{
    public class StorageServiceDesign : IStorageService
    {
        public IStorageServiceHandler Local { get; private set; }
        public IStorageServiceHandler Roaming { get; private set; }
        public IStorageServiceHandler Temporary { get; private set; }
        public IStorageServiceHandler LocalCache { get; private set; }
        public IStorageServiceHandler Package { get; private set; }
    }
}
