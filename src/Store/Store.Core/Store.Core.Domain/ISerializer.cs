using System;

namespace Store.Core.Domain;

// TODO: think about streams
public interface ISerializer
{
    string Serialize<T>(T input);
        
    T Deserialize<T>(string input);
        
    object Deserialize(string input, Type type);
        
    byte[] SerializeToBytes<T>(T input);
        
    byte[] SerializeToBytes(object input, Type type);

    T DeserializeFromBytes<T>(byte[] input);
        
    object DeserializeFromBytes(byte[] input, Type type);
}