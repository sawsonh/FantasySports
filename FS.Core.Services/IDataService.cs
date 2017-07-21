using System;
using System.Collections.Generic;

namespace FS.Core.Services
{
    public interface IDataService<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetList(Func<T, bool> predicate);
        T GetSingle(Func<T, bool> predicate);
        void Add(params T[] items);
        void Update(params T[] items);
        void Remove(params T[] items);
    }
}
