using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql;
using System.Data;

using Elephanet.Expressions;

namespace Elephanet
{

    public class QueryTranslator
    {
        public string Translate(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            return string.Format("select body from {0};",elementType.Name.ToLower());
        }
    }

    public interface IJsonbQueryProvider : IQueryProvider
    {
        string GetQueryText(Expression expression);
        NpgsqlConnection Connection {get;}
        IJsonConverter JsonConverter { get; }
    }


    public class JsonbQueryProvider : IJsonbQueryProvider
    {
        private readonly NpgsqlConnection _conn;
        private readonly IJsonConverter _jsonConverter;
        private JsonbExpressionVisitor _translator;

        public JsonbQueryProvider(NpgsqlConnection connection, IJsonConverter jsonConverter)
        {
            _conn = connection;
            _jsonConverter = jsonConverter;
            _translator = new JsonbExpressionVisitor();
        }

        public NpgsqlConnection Connection { get { return _conn; } }
        public IJsonConverter JsonConverter { get { return _jsonConverter; } }

        public object Execute(Expression expression)
        {
            _translator.Visit(expression);
            _translator.Command.Connection = _conn;

            using (var command = _translator.Command)
            {

                Type elementType = TypeSystem.GetElementType(expression.Type);
                Type listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)Activator.CreateInstance(listType);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        object car = _jsonConverter.Deserialize(reader.GetString(0), elementType);
                        list.Add(car);
                    }
                }

                return list;

            }
            
        }

        public string GetQueryText(Expression expression)
        {
            _translator.Visit(expression);
            return _translator.Command.CommandText;
        }


        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }


        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new JsonbQueryable<TElement>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Console.WriteLine("Calling Create Query"); 
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IJsonbQueryable)Activator.CreateInstance(typeof(JsonbQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
    }
}
