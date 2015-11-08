using System;
using System.Collections.Generic;

namespace Elephanet
{
    public interface IStoreInfo
    {
        /// <summary>
        /// Using custom ConcurrentHashSet.  This may be a micro optimisation over using ConcurrentDictionary / ConcurrentList
        /// which should be switched out at the first sign of issues.
        /// </summary>
        string Name { get; }
        ConcurrentHashSet<string> Tables { get; }
        void Add(string tableName);
        void Clear();
    }
}
