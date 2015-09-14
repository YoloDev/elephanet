using System;
using System.Collections.Generic;

namespace Elephanet
{
    public interface IDocumentSession : IDisposable
    {
        void Delete<T>(Guid id);
        void Delete<T>(T entity);
        T GetById<T>(Guid id);
        IEnumerable<T> GetByIds<T>(IEnumerable<Guid> ids);
        IEnumerable<T> GetAll<T>();
        IJsonbQueryable<T> Query<T>();
        void SaveChanges();
        void Store<T>(T entity);
        void DeleteAll<T>();
    }
}
