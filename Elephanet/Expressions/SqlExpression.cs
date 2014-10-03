using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Npgsql;

namespace Elephanet.Expressions
{
    public class SqlExpression : Expression
    {
        private Sql _query;
 
        public SqlExpression(Sql query) : base() {
            _query = query; 
        }

        public Sql Query { get { return _query; } }

    }
}
