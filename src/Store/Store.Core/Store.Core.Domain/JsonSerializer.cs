using System;

namespace Store.Core.Domain
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T input)
        {
            return System.Text.Json.JsonSerializer.Serialize(input);
        }

        public T Deserialize<T>(string input)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(input);
        }

        public object Deserialize(string input, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(input, type);
        }

        public byte[] SerializeToBytes<T>(T input)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(input);
        }

        public byte[] SerializeToBytes(object input, Type type)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(input, type);
        }

        public T DeserializeFromBytes<T>(byte[] input)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(input);
        }

        public object DeserializeFromBytes(byte[] input, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(input, type);
        }
    }
}