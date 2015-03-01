using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Elephanet
{
     public class QueryTranslator : ExpressionVisitor
    {
        const string columnName = "body";
        private StringBuilder _sb;
        private TableInfo _tableInfo;
        public QueryTranslator(TableInfo tableInfo)
        {
            _sb = new StringBuilder();
            _tableInfo = tableInfo;
        }

        public string Translate(Expression expression)
        {
            Visit(expression);
            return _sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                Type elementType = TypeSystem.GetElementType(node.Type);
                _sb.Append(string.Format("select {0} from {1} where {0} ", columnName, _tableInfo.TableNameWithSchema(elementType)));
                Visit(node.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                Visit(lambda.Body);
                return node;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
         }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            switch (node.NodeType) {
                case ExpressionType.Equal:
                    _sb.Append("@>");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The operator '{0}' is not yet supported", node.NodeType));
            }
            Visit(node.Right);

            return node;
        }
    }
}
