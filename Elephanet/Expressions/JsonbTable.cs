using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Elephanet
{
    public class JsonbTable 
    {
        private string _name;
        public JsonbTable(Type type, Expression expression)
        {
            _name = string.Format("@0_@1", type.Namespace, type.Name);
        }

        public string Name { get { return _name; } }
    }

    public class JsonbPath 
    {
       private string _name;
       public JsonbPath(string name, Expression expression)
        {
            _name = name;
        }

        public string Name { get { return _name; } }

    }

    public class JsonbValue 
    {
        private string _value;
        private Expression _expression;
        public JsonbValue(string value, Expression expression)
        {
            _value = value;
            _expression = expression;
        }
    }
}
