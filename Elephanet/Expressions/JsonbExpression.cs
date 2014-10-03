using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Elephanet.Expressions
{

   

    public class JsonbExpression  : Expression
    {
        Expression _jsonbTable;
        List<Expression> _jsonbPaths;
        
        public JsonbExpression(ExpressionType expressionType, Type type)
            : base((ExpressionType)expressionType, type)
        {

        }
    }

    public class JsonbTableExpression: JsonbExpression
    {
        private string _name;
        public JsonbTableExpression(Type type)
            : base((ExpressionType)JsonbExpressionType.JsonbTable, type)
        {
            _name = string.Format("{0}_{1}", type.Namespace, type.Name);
        }

        public string Name { get { return _name; } }
    }
}
