using System;

namespace Elephanet
{
    /// <summary>
    /// The default TableInfo which provides the default naming convention. Table names
    /// are created based on the namespace and type name.
    /// </summary>
    public class TableInfo : ITableInfo
    {
        private string _schema;

        public TableInfo()
        {
            _schema = "public";
        }

        public TableInfo(string schema)
        {
            _schema = schema;
        }

        public string TableNameWithSchema(Type type)
        {
            return String.Format("{0}.{1}_{2}", _schema, type.Namespace.ReplaceDotWithUnderscore().ToLower(), type.Name.ToLower());
        }

        public string TableNameWithoutSchema(Type type)
        {
            return String.Format("{0}_{1}", type.Namespace.ReplaceDotWithUnderscore().ToLower(), type.Name.ToLower());
        }

        public string Schema
        {
            get
            {
                return _schema;
            }
        }
    }
}
