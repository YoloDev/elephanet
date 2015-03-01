using System;
using System.Collections.Generic;

namespace Elephanet
{
    public interface IStoreInfo
    {
        string Name { get; }
        HashSet<string> Tables { get; }
        void Add(string tableName);
    }
}
