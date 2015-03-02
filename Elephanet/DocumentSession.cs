using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;
using Elephanet.Serialization;

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

        public T Load<T>(Guid id)
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

        public T[] Load<T>(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public T[] Load<T>(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
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

        void SaveInternal()
        {
            //TODO:  implement upsert from http://stackoverflow.com/questions/17267417/how-do-i-do-an-upsert-merge-insert-on-duplicate-update-in-postgresql

            foreach (var item in _entities)
            {
                GetOrCreateTable(item.Value.GetType());

                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"INSERT INTO {0} (id, body) VALUES (:id, :body);", _tableInfo.TableNameWithSchema(item.Value.GetType()));

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", _jsonConverter.Serialize(item.Value));
                    command.ExecuteNonQuery();
                }
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
                                id uuid NOT NULL DEFAULT uuid_generate_v1(), 
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
    }
}
