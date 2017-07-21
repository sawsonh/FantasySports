using System.Data.SqlClient;

namespace FS.Core.Repositories
{
    public interface ISqlRepository
    {
        void ExecuteSQL(string sql, SqlParameter[] sqlParameters = null);
        T ExecuteSQL<T>(string sql, SqlParameter[] sqlParameters = null);
    }
}