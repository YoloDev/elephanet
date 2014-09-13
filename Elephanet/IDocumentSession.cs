﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Elephanet
{
    public interface IDocumentSession : IDisposable
    {
        void Delete<T>(T entity);
        T Load<T>(Guid id);
        T[] Load<T>(params Guid[] ids);
        T[] Load<T>(IEnumerable<Guid> ids);
        IQueryable<T> Query<T>();
        void SaveChanges();
        void Store<T>(T entity);
    }
}