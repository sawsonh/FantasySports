using FS.Core.Entities;
using FS.Core.Repositories;
using FS.Core.Services;

namespace FS.Infrastructure.Services
{
    public class UserService : DataService<vUser>, IUserService
    {
        private readonly IDataRepository<vUser> _repo;

        public UserService(IDataRepository<vUser> repo) : base(repo)
        {
            _repo = repo;
        }
    }
}
