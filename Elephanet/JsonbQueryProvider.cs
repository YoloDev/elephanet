using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql;
using System.Data;
using System.Text;

using Elephanet.Expressions;

namespace Elephanet
{
    public interface IJsonbQueryProvider : IQueryProvider
    {
        NpgsqlConnection Connection {get;}
        IJsonConverter JsonConverter { get; }
    }

    public class JsonbQueryProvider : IJsonbQueryProvider
    {
        private readonly NpgsqlConnection _conn;
        private readonly IJsonConverter _jsonConverter;
        private QueryTranslator _translator;

        public JsonbQueryProvider(NpgsqlConnection connection, IJsonConverter jsonConverter, TableInfo tableInfo)
        {
            _conn = connection;
            _jsonConverter = jsonConverter;
            _translator = new QueryTranslator(tableInfo);
        }

        public NpgsqlConnection Connection { get { return _conn; } }
        public IJsonConverter JsonConverter { get { return _jsonConverter; } }

        public object Execute(Expression expression)
        {
            string sql = _translator.Translate(expression);
            Console.WriteLine(sql);
            using (var command = new NpgsqlCommand(sql,_conn))
            {
                Type elementType = TypeSystem.GetElementType(expression.Type);
                Type listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)Activator.CreateInstance(listType);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object entity = _jsonConverter.Deserialize(reader.GetString(0), elementType);
                        list.Add(entity);
                    }
                }

                return list;

            }
            
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
