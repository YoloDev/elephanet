using System;

namespace Elephanet
{
    /// <summary>
    /// Provides the flexibility to inject and control table naming behavior
    /// </summary>
    public interface ITableInfo
    {
        /// <summary>
        /// Return the table name for the given Type with its Schema
        /// </summary>
        /// <param name="type">The Type mapped to to the table</param>
        /// <returns>Table name</returns>
        string TableNameWithSchema(Type type);

        /// <summary>
        /// Return the table name for the given Type without its Schema
        /// </summary>
        /// <param name="type">The Type mapped to the table</param>
        /// <returns>Table name</returns>
        string TableNameWithoutSchema(Type type);

        /// <summary>
        /// The PostgreSql schema that all tables will be created in
        /// </summary>
        string Schema { get; }
    }
}