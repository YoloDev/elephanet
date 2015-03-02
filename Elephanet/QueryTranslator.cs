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
            var inlined = ExpressionEvaluator.EvaluateSubtrees(expression);
            Visit(inlined);
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
                //Visit(node.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                Visit(lambda.Body);
                return node;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
         }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType) {
                case ExpressionType.Equal:
                    _sb.Append("@>");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The operator '{0}' is not yet supported", node.NodeType));
            }
            //wrap up values in json
            _sb.Append("'{");
            Visit(node.Left);
            _sb.Append(":");
            Visit(node.Right);
            _sb.Append("}'");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _sb.Append(string.Format("\"{0}\"",node.Value));
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(string.Format("\"{0}\"", node.Member.Name));
                return node;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
        }
    }
}
