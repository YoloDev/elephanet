using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;
using Elephanet.Serialization;
using System.Text;

namespace Elephanet
{
    public class DocumentSession : IDocumentSession
    {
        private readonly IDocumentStore _documentStore;
        private NpgsqlConnection _conn;
        protected readonly Dictionary<Guid, object> _entities = new Dictionary<Guid, object>();
        readonly IJsonConverter _jsonConverter;
        private JsonbQueryProvider _queryProvider;
        private TableInfo _tableInfo;
      

        public DocumentSession(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _tableInfo = _documentStore.Conventions.TableInfo;
            _conn = new NpgsqlConnection(documentStore.ConnectionString);
            _conn.Open();
            _jsonConverter = documentStore.Conventions.JsonConverter;
            _queryProvider = new JsonbQueryProvider(_conn, _jsonConverter, _tableInfo);
        }

        public void Delete<T>(Guid Id)
        {
            using (var command = _conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = String.Format(@"Delete FROM {0} WHERE id = :id;", _tableInfo.TableNameWithSchema(typeof(T)));
                command.Parameters.AddWithValue(":id", Id);;
                command.ExecuteNonQuery();
            }
        }

        public void DeleteAll<T>()
        {
                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"DELETE FROM {0};", _tableInfo.TableNameWithSchema(typeof(T)));
                    command.ExecuteNonQuery();
                }

        }

        public T LoadInternal<T>(Guid id) 
        {

                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"SELECT body FROM {0} WHERE id = :id LIMIT 1;", _tableInfo.TableNameWithSchema(typeof(T)));

                    command.Parameters.AddWithValue(":id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return  _jsonConverter.Deserialize<T>(reader.GetString(0));
                        }

                        return default(T);
                    }
                }
        }

        public IJsonbQueryable<T> Query<T>()
        {
            IJsonbQueryable<T> query = new JsonbQueryable<T>(new JsonbQueryProvider(_conn, _jsonConverter, _tableInfo));
            return query;
        }

        public void SaveChanges()
        {
            //save the cache out to the db
            SaveInternal();
        }

        HashSet<Tuple<Type, string,string>> MatchEntityToFinalTableAndTemporaryTable(Dictionary<Guid, object> entities)
        {
            HashSet<Tuple<Type, string, string>> typeToTableMap = new HashSet<Tuple<Type, string, string>>();

            var types = entities.Values.Select(v => v.GetType()).Distinct();
            foreach (Type type in types)
            {
                typeToTableMap.Add(new Tuple<Type, string, string> ( type, _tableInfo.TableNameWithSchema(type), Guid.NewGuid().ToString() ));
            }

            return typeToTableMap;
        }

        void SaveInternal()
        {
            StringBuilder sb = new StringBuilder();

            HashSet<Tuple<Type, string,string>> matches = MatchEntityToFinalTableAndTemporaryTable(_entities);


            foreach (var item in _entities)
            {
                //make sure we have tables for all types
                GetOrCreateTable(item.Value.GetType());
            }

            sb.Append("BEGIN;");
            foreach (var match in matches)
            {
                sb.Append(string.Format("CREATE TEMPORARY TABLE \"{0}\" (id uuid, body jsonb);", match.Item3)); 
            }

            foreach (var item in _entities)
            {
                sb.Append(string.Format("INSERT INTO \"{0}\" (id, body) VALUES ('{1}', '{2}');", matches.Where(c => c.Item1 == item.Value.GetType()).Select(j => j.Item3).First(),item.Key, _jsonConverter.Serialize(item.Value)));
            }

            foreach (var match in matches)
            {
                sb.Append(string.Format("LOCK TABLE {0} IN EXCLUSIVE MODE;", match.Item2));
                sb.Append(string.Format("UPDATE {0} SET body = tmp.body from \"{1}\" tmp where tmp.id = {0}.id;", match.Item2, match.Item3));
                sb.Append(string.Format("INSERT INTO {0} SELECT tmp.id, tmp.body from \"{1}\" tmp LEFT OUTER JOIN {0} ON ({0}.id = tmp.id) where {0}.id IS NULL;", match.Item2, match.Item3));
            }


            sb.Append("COMMIT;");

            using (var command = _conn.CreateCommand())
            {
                command.CommandTimeout = 60;
                command.CommandType = CommandType.Text;
                command.CommandText = sb.ToString();
                command.ExecuteNonQuery();
            }

            _entities.Clear();
        }

        public void Store<T>(T entity)
        {
            var id = IdentityFactory.SetEntityId(entity);
            _entities[id] = entity;
        }

        private bool IndexDoesNotExist(Type type)
        {
            using (var command = _conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(@"select count(*)
                from pg_indexes
                where schemaname = '{0}'
                and tablename = '{1}'
                and indexname = 'idx_{1}_body';", _tableInfo.Schema, _tableInfo.TableNameWithoutSchema(type));
                var indexCount = (Int64)command.ExecuteScalar();
                return indexCount == 0;
            }

        }
        private void CreateIndex(Type type)
        {
            if (IndexDoesNotExist(type))
            {
                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(@"CREATE INDEX idx_{0}_body ON {0} USING gin (body);", _tableInfo.TableNameWithoutSchema(type));
                    command.ExecuteNonQuery();
                }
            }
        }


        private void GetOrCreateTable(Type type)
        {
            if (!_documentStore.StoreInfo.Tables.Contains(_tableInfo.TableNameWithSchema(type)))
            {
                _documentStore.StoreInfo.Tables.Add(_tableInfo.TableNameWithSchema(type));
                try 
                {
                    using (var command = _conn.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"
                            CREATE TABLE IF NOT EXISTS {0}
                            (
                                id uuid NOT NULL, 
                                body jsonb NOT NULL, 
                                created time without time zone NOT NULL DEFAULT now(), 
                                row_version integer NOT NULL DEFAULT 1, 
                                CONSTRAINT pk_{1} PRIMARY KEY (id)
                            );",_tableInfo.TableNameWithSchema(type), _tableInfo.TableNameWithoutSchema(type));
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(String.Format("Could not create table {0}; see the inner exception for more information.", _tableInfo.TableNameWithSchema(type)), exception);
                }
                try
                {
                        CreateIndex(type);
                }
                catch (Exception exception)
                {
                    throw new Exception(String.Format("Could not create index on table {0}; see the inner exception for more information.", _tableInfo.TableNameWithSchema(type)), exception);
                }
            }
        }

        public void Dispose()
        {
            _conn.Close();
        }


        public void Delete<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(Guid id)
        {
            GetOrCreateTable(typeof(T));
            //hit the db first, so we get most up-to-date
            var entity = LoadInternal<T>(id);
            //try the cache just incase hasn't been saved to db yet, but is in session
            if ((entity == null) && _entities.ContainsKey(id))
                entity = (T)_entities[id];

            if (entity == null)
            {
                throw new NullReferenceException(string.Format("Entity id {0} does not exist.", id));
            }
            return entity;
        }

        public IEnumerable<T> GetByIds<T>(IEnumerable<Guid> ids)
        {

                 using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"SELECT body FROM {0} WHERE id in ({1});", _tableInfo.TableNameWithSchema(typeof(T)), JoinAndCommaSeperateAndSurroundWithSingleQuotes(ids));
                    Console.WriteLine(command.CommandText);

                    List<T> entities = new List<T>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T entity = _jsonConverter.Deserialize<T>(reader.GetString(0));
                            entities.Add(entity);
                        }
                    }
                    return entities;
                }
        }

        private string JoinAndCommaSeperateAndSurroundWithSingleQuotes<T>(IEnumerable<T> ids)
        {
            return string.Join(",",ids.Select(n => n.ToString().SurroundWithSingleQuote()).ToArray());
        }

        public IEnumerable<T> GetAll<T>()
        {
            using (var command = _conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = String.Format(@"SELECT body FROM {0};", _tableInfo.TableNameWithSchema(typeof(T)));
                Console.WriteLine(command.CommandText);

                List<T> entities = new List<T>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T entity = _jsonConverter.Deserialize<T>(reader.GetString(0));
                        entities.Add(entity);
                    }
                }
                return entities;
            }
        }
    }
}
