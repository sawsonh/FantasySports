using FS.Core.Repositories;
using FS.Core.Services;
using System;
using System.Collections.Generic;

namespace FS.Infrastructure.Services
{
    public class DataService<T> : IDataService<T> where T : class
    {
        private readonly IDataRepository<T> _repo;

        public DataService(IDataRepository<T> repo)
        {
            _repo = repo;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _repo.GetAll();
        }
        public virtual IEnumerable<T> GetList(Func<T, bool> predicate)
        {
            return _repo.GetList(predicate);
        }

        public virtual T GetSingle(Func<T, bool> predicate)
        {
            return _repo.GetSingle(predicate);
        }

        public virtual void Add(params T[] items)
        {
            _repo.Add(items);
        }

        public virtual void Remove(params T[] items)
        {
            _repo.Remove(items);
        }

        public virtual void Update(params T[] items)
        {
            _repo.Update(items);
        }
    }
}
