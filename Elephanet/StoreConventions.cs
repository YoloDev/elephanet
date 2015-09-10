using System;
using Elephanet;
using Elephanet.Conventions;
using Elephanet.Serialization;

namespace Elephanet
{
    public class StoreConventions : IStoreConventions
    {
        IJsonConverter _jsonConverter;
        private ITableInfo _tableInfo;
        private EntityNotFoundBehavior _entityNotFoundBehavior = EntityNotFoundBehavior.Throw;

        public StoreConventions()
        {
            _jsonConverter = new JilJsonConverter();
            _tableInfo = new TableInfo();
        }

        public StoreConventions(IJsonConverter jsonConverter)
        {
            _jsonConverter = jsonConverter;
            _tableInfo = new TableInfo();
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

        public EntityNotFoundBehavior EntityNotFoundBehavior { get {return _entityNotFoundBehavior;} }

        /// <summary>
        /// Behavior of DocumentSession.GetById when the Entity is not found.
        /// </summary>
        /// <param name="behavior"></param>
        public void SetEntityNotFoundBehavior(EntityNotFoundBehavior behavior)
        {
            _entityNotFoundBehavior = behavior;
        }
    }
}
