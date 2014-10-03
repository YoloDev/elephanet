using System;

namespace Elephanet
{
    public interface IJsonConverter
    {
        string Serialize<T>(T entity);
        T Deserialize<T>(string json);
        object Deserialize(string json);
        object Deserialize(string json, Type type);
    }
}
