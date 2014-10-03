using System;
using Newtonsoft.Json;
using Elephanet;

namespace Elephanet
{
    public class StoreConventions : IStoreConventions
    {

        IJsonConverter _jsonConverter;
        private TableInfo _tableInfo;
        public StoreConventions()
        {
            _jsonConverter = new JsonConverter();
            _tableInfo = new TableInfo();
        }

        public StoreConventions(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public IJsonConverter JsonConverter
        {
            get { return _jsonConverter; }
        }

        public TableInfo TableInfo
        {
            get { return _tableInfo; }
        }
    }
}
