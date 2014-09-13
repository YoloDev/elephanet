﻿using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;
using System;


namespace Elephanet
{
    public class DocumentSession : IDocumentSession
    {
        private readonly IDocumentStore _documentStore;
        private NpgsqlConnection _conn;
        protected readonly Dictionary<Guid, object> _entities = new Dictionary<Guid, object>();
        readonly IJsonConverter _jsonConverter;
      

        public DocumentSession(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _conn = new NpgsqlConnection(documentStore.ConnectionString);
            _conn.Open();
            _jsonConverter = documentStore.Conventions.JsonConverter;
        }

        public void Delete<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public T Load<T>(Guid id)
        {
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
                    command.CommandText = String.Format(@"SELECT body FROM public.{0} WHERE id = :id LIMIT 1;", typeof(T).Name);

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

        public IQueryable<T> Query<T>()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            //save the cache out to the db
            SaveInternal();
        }

        void SaveInternal()
        {
            var storeInfo = _documentStore.StoreInfo.Name;
            //make sure all the entities have tables, if not, created them


            foreach (var item in _entities)
            {
                GetOrCreateTable(item.Value.GetType());
                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"INSERT INTO public.{0} (id, body) VALUES (:id, :body);", item.Value.GetType().Name);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", _jsonConverter.Serialize(item.Value));

                    command.ExecuteNonQuery();
                }
            }
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
                where schemaname = 'public'
                and tablename = '{0}'
                and indexname = 'idx_{0}_body';", type.Name);
                var indexCount = (Int64)command.ExecuteScalar();
                return indexCount != 0;
            }

        }
        private void CreateIndex(Type type)
        {
            if (IndexDoesNotExist(type))
            {
                using (var command = _conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(@"CREATE INDEX idx_{0}_body ON {0} USING gin (body);", type.Name);
                    command.ExecuteNonQuery();
                }
            }
        }


        private void GetOrCreateTable(Type type)
        {
                try 
                {
                    using (var command = _conn.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"
                            CREATE TABLE IF NOT EXISTS public.{0}
                            (
                                id uuid NOT NULL DEFAULT uuid_generate_v1(), 
                                body jsonb NOT NULL, 
                                created time without time zone NOT NULL DEFAULT now(), 
                                row_version integer NOT NULL DEFAULT 1, 
                                CONSTRAINT pk_{0} PRIMARY KEY (id)
                            );", type.Name);
                        command.ExecuteNonQuery();
                        CreateIndex(type);
                    }
                }
                catch (NpgsqlException exception)
                {
                    throw new Exception(String.Format("Could not create table {0}; see the inner exception for more information.", type.Name), exception);
                }
               
            }

        public void Dispose()
        {
            _conn.Close();
        }
    }
}