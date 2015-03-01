﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Elephanet
{

    public interface IJsonbQueryable<T>: IOrderedQueryable<T>
    {
    }

    public interface IJsonbQueryable: IOrderedQueryable
    {
    }

   public class JsonbQueryable<T> : IJsonbQueryable<T>  
   {
        IJsonbQueryProvider _provider;
        Expression _expression;
 
        public JsonbQueryable(IJsonbQueryProvider provider) {
            if (provider == null) {
                throw new ArgumentNullException("provider");
            }
            _provider = provider;
            _expression = Expression.Constant(this);
        }
 
        public JsonbQueryable(IJsonbQueryProvider provider, Expression expression) {
            if (provider == null) {
                throw new ArgumentNullException("provider");
            }
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type)) {
                throw new ArgumentOutOfRangeException("expression");
            }
            _provider = provider;
            _expression = expression;
        }
 
        Expression IQueryable.Expression {
            get { return _expression; }
        }
 
        Type IQueryable.ElementType {
            get { return typeof(T); }
        }
 
        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();
        }
 
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }
 
        IQueryProvider IQueryable.Provider
        {
            get { return (IJsonbQueryProvider)_provider; }
        }
   }
  
}
