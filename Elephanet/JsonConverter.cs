using Newtonsoft.Json;
using System;

namespace Elephanet
{
    public class JsonConverter : IJsonConverter
    {

        public string Serialize<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
