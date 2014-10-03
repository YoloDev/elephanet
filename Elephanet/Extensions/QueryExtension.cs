using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Elephanet.Expressions;
using Npgsql;

namespace Elephanet
{

    public static class StringExtension
    {
        public static string ReplaceDotWithUnderscore(this string text)
        {
            text = text.Replace(".", "_");
            return text;
        }
    }


    public static class QueryExtension
    {
        public static IJsonbQueryable<TSource> Where<TSource>(this IJsonbQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return (IJsonbQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[]
	{
		typeof(TSource)
	}), new Expression[]
	{
		source.Expression,
		Expression.Quote(predicate)
	}));
        }

        public static List<T> Where<T>(this IJsonbQueryable<T> source, Sql<T> query)
        {
            return Where<T>(source, query.Command);
        }

        public static List<T> Where<T>(this IJsonbQueryable<T> source, Sql query)
        {
            return Where<T>(source, query.Command);
        }

        private static List<T> Where<T>(IJsonbQueryable<T> source, NpgsqlCommand command)
        {
            var provider = (IJsonbQueryProvider)source.Provider;
            var jsonConverter = provider.JsonConverter;
            var conn = provider.Connection;
            Console.WriteLine(command.CommandText);

            using (command)
            {
                var list = new List<T>();
                command.Connection = conn;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        T car = jsonConverter.Deserialize<T>(reader.GetString(0));
                        list.Add(car);
                    }
                }

                return list;
            }
        }

      

    }

    
}
