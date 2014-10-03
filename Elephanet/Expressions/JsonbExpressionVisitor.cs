using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Npgsql;
using System.Data;

namespace Elephanet.Expressions
{
    public class JsonbExpressionVisitor : ExpressionVisitor
    {
        private NpgsqlCommand _command;

        public JsonbExpressionVisitor()
        {
            _command = new NpgsqlCommand();
        }

        public NpgsqlCommand Command { get { return _command; } }

        protected override Expression VisitExtension(Expression node)
        {
            var jsonbNode = node as JsonbExpression;
            return jsonbNode == null ? node : VisitJsonb(jsonbNode);
        }

        protected virtual Expression VisitSql(SqlExpression node)
        {
            _command = node.Query.Command;
            _command.CommandType = CommandType.Text;
            return node;
        }

        protected virtual Expression VisitJsonb(JsonbExpression node)
        {
 //           switch (node.JsonbType)
 //           {
 //               case JsonbType.Type1:
 //                   return VisitJsonbType1((JsonbType1)node);
 //               case JsonbType.Type2:
 //                   return VisitJsonbType2((JsonbType2)node);
 //               default:
                    return node;
 //           }
        }

 //       protected virtual Expression VisitJsonbType1(JsonbType1 node)
 //       {
            // Visit node parts
 //       }

//        protected virtual Expression VisitJsonbType2(JsonbType2 node)
//        {
            // Visit node parts
//        }


    }
}
