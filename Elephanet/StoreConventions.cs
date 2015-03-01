using System;
using Newtonsoft.Json;
using Elephanet;
using Elephanet.Serialization;

namespace Elephanet
{
    public class StoreConventions : IStoreConventions
    {

        IJsonConverter _jsonConverter;
        private TableInfo _tableInfo;
        public StoreConventions()
        {
            _jsonConverter = new JilJsonConverter();
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
