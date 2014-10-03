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
        readonly List<string> _tableNames;

        public DocumentStore(string connectionString)
        {
            _connectionString = connectionString;
            _conventions = new StoreConventions();
            _storeInfo = new StoreInfo();;
            _tableNames = new List<string>();
        }

        public DocumentStore(string connectionString, IStoreConventions conventions)
        {
            _connectionString = connectionString;
            _conventions = conventions;
            _storeInfo = new StoreInfo();
        }

        public DocumentStore(string connectionString, IStoreConventions conventions, IStoreInfo storeInfo)
        {
            _connectionString = connectionString;
            _conventions = conventions;
            _storeInfo = storeInfo;
        }

        public DocumentStore(string connectionString, IStoreInfo storeInfo)
        {
            _connectionString = connectionString;
            _conventions = new StoreConventions();
            _storeInfo = storeInfo;
        }

        public IStoreConventions Conventions { get { return _conventions; }}

        public List<string> TableNames { get {return _tableNames;} }

        public IDocumentSession OpenSession()
        {
            return new DocumentSession(this);
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    
        public  IStoreInfo StoreInfo { get {return _storeInfo;}}


        public void Destroy()
        {
              var connection = new NpgsqlConnection(_connectionString);
              try
              {
                   connection.Open();
                  foreach (var tablename in StoreInfo.Tables)
                  {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"drop table {0};", tablename);
                        Console.WriteLine(command.CommandText);
                        command.ExecuteNonQuery();
                    }
                  }
              } 

              catch (NpgsqlException exception)
              {
                    throw new Exception(String.Format("Could not drop table {0}; see the inner exception for more information.", _storeInfo.Name), exception);
              }

              finally
              {
                    connection.Dispose();
              }
        }

        public void Empty()
        {
              var connection = new NpgsqlConnection(_connectionString);
              try
              {
                   connection.Open();
                  foreach (var tablename in StoreInfo.Tables)
                  {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"delete from {0};", tablename);
                        Console.WriteLine(command.CommandText);
                        command.ExecuteNonQuery();
                    }
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
}
