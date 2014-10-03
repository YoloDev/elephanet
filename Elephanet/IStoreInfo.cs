using System;
using System.Collections.Generic;

namespace Elephanet
{
    public interface IStoreInfo
    {
        string Name { get; }
        List<string> Tables { get; }
        void Add(string tableName);
    }
}
