using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace Elephanet
{

    public class Sql<T>
    {
        private NpgsqlCommand _command;
        public Sql(string query, object[] parameters)
        {
            _command = new NpgsqlCommand();

            _command.CommandText = String.Format(@"SELECT body FROM public.{0}_{1} WHERE body @> {2}", typeof(T).Namespace.ReplaceDotWithUnderscore(), typeof(T).Name, query);
            foreach (var entry in MatchParameters(query, parameters))
            {
                string json = ConvertKeyValueToJson(entry);
                _command.Parameters.Add(new NpgsqlParameter(entry.Key,json));
            }
        }

        private string ConvertKeyValueToJson(KeyValuePair<string, object> entry)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return  string.Format("{{\"{0}\":\"{1}\"}}", textInfo.ToTitleCase(entry.Key.Substring(1)), entry.Value);
        }

        private Dictionary<string, object> MatchParameters(string query, object[] parameters)
        {
            var matches = new Dictionary<string, object>();
            int counter = 0;
            foreach (Match match in Regex.Matches(query,(@"(?<!\w):\w+")))
            {
                matches.Add(match.Value,parameters[counter]);
                counter = counter + 1;

            }
            return matches;
        }

        public NpgsqlCommand Command { get { return _command; } }

    }

    public class Sql
    {
        private NpgsqlCommand _command;
        public Sql(string query, object[] parameters)
        {
            _command = new NpgsqlCommand();
            _command.CommandText = query;
            foreach (var entry in MatchParameters(query, parameters))
            {
                _command.Parameters.Add(new NpgsqlParameter(entry.Key,entry.Value));
                // do something with entry.Value or entry.Key
            }
            Console.WriteLine(_command.CommandText);
        }

        private Dictionary<string, object> MatchParameters(string query, object[] parameters)
        {
            var matches = new Dictionary<string, object>();
            int counter = 0;
            foreach (Match match in Regex.Matches(query,(@"(?<!\w):\w+")))
            {
                matches.Add(match.Value,parameters[counter]);
                counter = counter + 1;

            }
            return matches;
        }

        public NpgsqlCommand Command { get { return _command; } }

    }
}
