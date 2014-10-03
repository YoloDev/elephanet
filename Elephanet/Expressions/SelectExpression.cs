using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Elephanet.Expressions
{
    public class SelectExpression : Expression
    {
        private Expression _from;
        private Expression _where;
        public SelectExpression(Type type, Expression from, Expression where)
        {
            _from = from;
            _where = where;
        }
    }
}
