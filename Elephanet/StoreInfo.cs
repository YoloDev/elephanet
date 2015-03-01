using System;
using System.Collections.Generic;

namespace Elephanet
{
    public class StoreInfo : IStoreInfo
    {
        private string _name;
        readonly HashSet<string> _tableNames;
        public StoreInfo()
        {
            _name = "store";
            _tableNames = new HashSet<string>();
        }

        public HashSet<string> Tables { get { return _tableNames; } }

        public void Add(string tableName)
        {
            _tableNames.Add(tableName);
        }

        public StoreInfo(string storeName)
        {
            _name = storeName;
        }

        public string Name { get { return _name; } }
    }
}
