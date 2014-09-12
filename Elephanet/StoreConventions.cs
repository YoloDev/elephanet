using System;
using Newtonsoft.Json;

namespace Elephanet
{
    public class StoreConventions : IStoreConventions
    {

        IJsonConverter _jsonConverter;

        public StoreConventions()
        {
            _jsonConverter = new JsonConverter();
        }

        public StoreConventions(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public IJsonConverter JsonConverter
        {
            get { return _jsonConverter; }
        }

        public string TableName { get { return "store"; } }
    }

    public interface IStoreConventions
    {
        IJsonConverter JsonConverter { get; }
        string TableName { get; }
    }

    public interface IJsonConverter
    {
        string Serialize<T>(T entity);
        T Deserialize<T>(string json);
    }
    

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

    }
}
