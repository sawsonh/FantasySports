using FS.Core.Entities;
using FS.Core.Repositories;
using FS.Core.Services;

namespace FS.Infrastructure.Services
{
    public class IdentityProviderService : DataService<IdentityProvider>, IIdentityProviderService
    {
        private readonly IDataRepository<IdentityProvider> _repo;

        public IdentityProviderService(IDataRepository<IdentityProvider> repo) : base(repo)
        {
            _repo = repo;
        }
    }
}
