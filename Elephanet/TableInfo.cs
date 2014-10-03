using System;

namespace Elephanet
{
    public class TableInfo
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
