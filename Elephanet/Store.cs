using System;
using System.Data;
using System.Data.Common;
using Npgsql;
using System.Collections.Generic;

namespace Elephanet
{

    public class DocumentStore : IDocumentStore
    {
        private string _connectionString;
        readonly IStoreConventions _conventions;
        readonly IStoreInfo _storeInfo;
        private string p;
        private IStoreInfo testStore;

        public DocumentStore(string connectionString)
        {
            _connectionString = connectionString;
            _conventions = new StoreConventions();
            _storeInfo = new StoreInfo();
            GetOrCreateTable();
        }

        public DocumentStore(string connectionString,IStoreConventions conventions)
        {
            _connectionString = connectionString;
            _conventions = conventions;
            _storeInfo = new StoreInfo();
            GetOrCreateTable();
        }

        public DocumentStore(string connectionString,IStoreConventions conventions, IStoreInfo storeInfo)
        {
            _connectionString = connectionString;
            _conventions = conventions;
            _storeInfo = storeInfo;
            GetOrCreateTable();
        }

        public DocumentStore(string connectionString, IStoreInfo storeInfo)
        {
            _connectionString = connectionString;
            _conventions = new StoreConventions();
            _storeInfo = storeInfo;
            GetOrCreateTable();
        }

        public void GetOrCreateTable()
        {

            object _lock = new object();
            lock (_lock)
            {
                var connection = new NpgsqlConnection(_connectionString);
                try
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"
                            CREATE TABLE IF NOT EXISTS public.{0}
                            (
                                id uuid NOT NULL DEFAULT uuid_generate_v1(), 
                                body json NOT NULL, 
                                created time without time zone NOT NULL DEFAULT now(), 
                                row_version integer NOT NULL DEFAULT 1, 
                                CONSTRAINT pk_{0} PRIMARY KEY (id)
                            );", _storeInfo.Name);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException exception)
                {
                    throw new Exception(String.Format("Could not create table {0}; see the inner exception for more information.", _storeInfo.Name), exception);
                }
                finally
                {
                    connection.Dispose();
                }
            }
        }

      

        public IStoreConventions Conventions { get { return _conventions; }}

      

        public IDocumentSession OpenSession()
        {
            return new DocumentSession(this);
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    
        public  IStoreInfo StoreInfo { get {return _storeInfo;}}



        public void Empty()
        {

              var connection = new NpgsqlConnection(_connectionString);
              try
              {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"DELETE FROM public.{0};", _storeInfo.Name);
                        command.ExecuteNonQuery();
                    }
              } 

              catch (NpgsqlException exception)
              {
                    throw new Exception(String.Format("Could not delete all from table {0}; see the inner exception for more information.", _storeInfo.Name), exception);
              }

              finally
              {
                    connection.Dispose();
              }
        }
    }

    public interface IDocumentStore
    {
        IDocumentSession OpenSession();
        string ConnectionString { get; }
        IStoreConventions Conventions { get; }
        IStoreInfo StoreInfo { get; }
        void Empty();
    }

    public interface IDocumentSession : IDisposable
    {
        void Delete<T>(T entity);
        T Load<T>(Guid id);
        T[] Load<T>(params Guid[] ids);
        T[] Load<T>(IEnumerable<Guid> ids);
        IEnumerable<T> Query<T>();
        void SaveChanges();
        void Store<T>(T entity);
    }

    public class StoreInfo : IStoreInfo
    {
        private string _name;
        public StoreInfo()
        {
            _name = "store";
        }

        public StoreInfo(string storeName)
        {
            _name = storeName;
        }

        public string Name { get { return _name; } }
    }


        public interface IStoreInfo 
        {
            string Name {get;}
        }

       
}
