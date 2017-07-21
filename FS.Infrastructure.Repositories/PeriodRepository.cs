using FS.Core.Entities;
using FS.Core.Repositories;
using System;
using System.Data.SqlClient;

namespace FS.Infrastructure.Repositories
{
    public class PeriodRepository : DataRepository<Period>, IPeriodRepository
    {
        private readonly ISqlRepository _sqlRepo;

        public PeriodRepository(ISqlRepository sqlRepo)
        {
            _sqlRepo = sqlRepo;
        }
    }
}
