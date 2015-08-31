using Jil;

namespace Elephanet.Serialization
{
    public class JilJsonConverter : IJsonConverter
    {
        private readonly Options _options;
        public JilJsonConverter()
        {
            _options = new Options(includeInherited: true, dateFormat:DateTimeFormat.ISO8601);
        }

        public string Serialize<T>(T entity)
        {
            
            return JSON.Serialize<T>(entity,_options);
        }

        public T Deserialize<T>(string json)
        {
            return JSON.Deserialize<T>(json,_options);
        }

        public object Deserialize(string json)
        {
            return JSON.DeserializeDynamic(json, _options);
        }

        public object Deserialize(string json, System.Type type)
        {
            return JSON.Deserialize(json, type,_options);
        }
    }
}
