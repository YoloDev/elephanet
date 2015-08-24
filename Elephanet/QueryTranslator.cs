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
        private ITableInfo _tableInfo;

        private Expression _where;
        private StringBuilder _limit;
        private StringBuilder _offset;
        private StringBuilder _orderBy;

        public QueryTranslator(ITableInfo tableInfo)
        {
            _sb = new StringBuilder();
            _limit = new StringBuilder();
            _offset = new StringBuilder();
            _orderBy = new StringBuilder();
            _tableInfo = tableInfo;
        }

        public string Translate(Expression expression)
        {
            var inlined = ExpressionEvaluator.EvaluateSubtrees(expression);
            Visit(inlined);

            _sb.Append(_orderBy);
            _sb.Append(_limit);
            _sb.Append(_offset);
            _sb.Append(";");
            Console.WriteLine(_sb.ToString());
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
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                switch (node.Method.Name)
                {
                    case "Where":
                    {
                        Type elementType = TypeSystem.GetElementType(node.Type);
                        _sb.Append(string.Format("select {0} from {1} where {0} ", columnName, _tableInfo.TableNameWithSchema(elementType)));
                        //Visit(node.Arguments[0]);
                        LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                        Visit(lambda.Body);
                        return node;
                    }
                    case "Take":
                    {
                        _limit.Append(" limit ");
                        VisitLimit((ConstantExpression)node.Arguments[1]);
                        VisitMethodCall((MethodCallExpression)node.Arguments[0]);
                        return node;
                    }
                    case "Skip":
                    {
                        _offset.Append(" offset ");
                        VisitOffset((ConstantExpression)node.Arguments[1]);
                        VisitMethodCall((MethodCallExpression)node.Arguments[0]);
                        return node;

                    }
                    case "OrderBy":
                    {
                        Type elementType = TypeSystem.GetElementType(node.Type);
                        LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                        VisitOrderBy((MemberExpression)lambda.Body);
                        VisitMethodCall((MethodCallExpression)node.Arguments[0]);
                        return node;
                    }
                    case "OrderByDescending":
                        {
                            Type elementType = TypeSystem.GetElementType(node.Type);
                            LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                            VisitOrderByDesc((MemberExpression)lambda.Body);
                            VisitMethodCall((MethodCallExpression)node.Arguments[0]);
                            return node;

                        }
                       

                }
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
         }

        private Expression VisitOrderByDesc(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                _orderBy.Append(string.Format(" order by body->>'{0}' desc", node.Member.Name));
                return node;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported from the OrderByDescending operator", node.Member.Name));
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

         private Expression VisitLimit(ConstantExpression node)
        {
            _limit.Append(string.Format("{0}",node.Value));
            return node;
        }


         private Expression VisitOffset(ConstantExpression node)
         {
             _offset.Append(string.Format("{0}", node.Value));
             return node;
         }

         private Expression VisitOrderBy(MemberExpression node)
         {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                _orderBy.Append(string.Format(" order by body->>'{0}'", node.Member.Name));
                return node;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));

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
