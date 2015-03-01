using Jil;

namespace Elephanet.Serialization
{
    public class JilJsonConverter : IJsonConverter
    {
        public string Serialize<T>(T entity)
        {
            return JSON.Serialize<T>(entity);
        }

        public T Deserialize<T>(string json)
        {
            return JSON.Deserialize<T>(json);
        }

        public object Deserialize(string json)
        {
            return JSON.DeserializeDynamic(json);
        }

        public object Deserialize(string json, System.Type type)
        {
            return JSON.Deserialize(json, type);
        }
    }
}
