using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;

namespace Speader.Design
{
    public class ApplicationSettingsServiceHandlerDesign : IApplicationSettingsServiceHandler
    {
        public bool Contains(string key)
        {
            return false;
        }

        public T Get<T>(string key)
        {
            return default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
        }

        public void Remove(string key)
        {
        }

        public async Task<IEnumerable<KeyValuePair<string, object>>> GetValuesAsync()
        {
            return new List<KeyValuePair<string, object>>();
        }
    }
}