using System;
using Elephanet;
using Elephanet.Serialization;

namespace Elephanet
{
    public class StoreConventions : IStoreConventions
    {
        IJsonConverter _jsonConverter;
        private ITableInfo _tableInfo;

        public StoreConventions()
        {
            _jsonConverter = new JilJsonConverter();
            _tableInfo = new TableInfo();
        }

        public StoreConventions(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
        }

        public StoreConventions(IJsonConverter jsonConverter, ITableInfo tableInfo)
        {
            _jsonConverter = jsonConverter;
            _tableInfo = tableInfo;
        }

        public IJsonConverter JsonConverter
        {
            get { return _jsonConverter; }
        }

        public ITableInfo TableInfo
        {
            get { return _tableInfo; }
        }
    }
}
