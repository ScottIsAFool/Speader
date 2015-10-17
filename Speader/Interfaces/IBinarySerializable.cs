using System.IO;

namespace Speader.Interfaces
{
    public interface IBinarySerializable
    {
        void Write(BinaryWriter writer);
        T InitFromCache<T>(BinaryReader reader) where T : class, IBinarySerializable;
        void Read(BinaryReader reader);
    }
}
