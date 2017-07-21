using FS.Core.Entities;
using FS.Core.Repositories;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;

namespace FS.Infrastructure.Repositories
{
    public class SqlRepository : ISqlRepository
    {
        private readonly DbContext _dbContext;

        public SqlRepository()
        {
            _dbContext = new FantasySportsEntities();
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public IDbSet<T> Set<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        private string ConnString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        public void ExecuteSQL(string sql, SqlParameter[] sqlParameters = null)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (sqlParameters != null)
                            cmd.Parameters.AddRange(sqlParameters);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    var parameters = new StringBuilder();
                    foreach (var sqlParameter in sqlParameters)
                    {
                        parameters.AppendLine(string.Format("{0} = '{1}'", sqlParameter.ParameterName, sqlParameter.Value));
                    }
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public T ExecuteSQL<T>(string sql, SqlParameter[] sqlParameters = null)
        {
            object obj = null;
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (sqlParameters != null)
                            cmd.Parameters.AddRange(sqlParameters);
                        if (typeof(T) == typeof(DataTable))
                        {
                            var ds = new DataSet();
                            using (var da = new SqlDataAdapter())
                            {
                                da.SelectCommand = cmd;
                                da.Fill(ds);
                            }
                            obj = ds.Tables[0];
                        }
                        else
                        {
                            obj = cmd.ExecuteScalar();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return obj is T ? (T)obj : (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
