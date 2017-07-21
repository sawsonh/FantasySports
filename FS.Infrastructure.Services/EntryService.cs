using FS.Core.Entities;
using FS.Core.Repositories;
using FS.Core.Services;

namespace FS.Infrastructure.Services
{
    public class EntryService : DataService<Entry>, IEntryService
    {
        private readonly IDataRepository<Entry> _repo;

        public EntryService(IDataRepository<Entry> repo) : base(repo)
        {
            _repo = repo;
        }
    }
}
