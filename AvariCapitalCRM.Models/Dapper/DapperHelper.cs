using Dapper;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Dapper
{
    public class DapperHelper
    {

        static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        static int commandTimeout = 30;
        public static IDbConnection GetConnection(string connString)
        {
            if (string.IsNullOrEmpty(connString))
                return new SqlConnection(connectionString);
            return new SqlConnection(connString);
        }

        public static T Get<T>(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            using (IDbConnection conn = GetConnection(connString))
            {
                conn.Open();
                return conn.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, transaction: transaction);
            }
        }

        public static async Task<T> GetAsync<T>(string sql, object param = null, string connString = null)
        {
            using (IDbConnection conn = GetConnection(connString))
            {
                conn.Open();
                return await conn.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout).ConfigureAwait(false);
            }
        }

        public static List<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            using (IDbConnection conn = GetConnection(connString))
            {
                conn.Open();
                return conn.Query<T>(sql, param, commandTimeout: commandTimeout, transaction: transaction).ToList();
            }
        }

        public static async Task<List<T>> QueryListAsync<T>(string sql, object param = null, string connString = null)
        {
            using (IDbConnection conn = GetConnection(connString))
            {
                conn.Open();
                var list = await conn.QueryAsync<T>(sql, param, commandTimeout: commandTimeout).ConfigureAwait(false);
                return list.ToList();
            }
        }


        public static int Execute(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(connString))
                {
                    conn.Open();
                    return conn.Execute(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.Execute(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
            }
        }
        public static object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(connString))
                {
                    conn.Open();
                    return conn.ExecuteScalar(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.ExecuteScalar(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
            }
        }

        public static T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(connString))
                {
                    conn.Open();
                    return conn.ExecuteScalar<T>(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.ExecuteScalar<T>(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
            }
        }

        public static async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, string connString = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(connString))
                {
                    conn.Open();
                    return await conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text).ConfigureAwait(false);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return await conn.ExecuteAsync(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text).ConfigureAwait(false);
            }
        }


        public static PageResult<T> GetPageList<T>(string sql, string sort, BaseReqestParams request, object param = null, string connString = null)
        {
            int pageIndex = request.page, pageSize = request.limit;
            PageResult<T> result = new PageResult<T>();
            string pageSql = @"SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) _row_number_,*  FROM 
              ({2})temp )temp1 WHERE temp1._row_number_>{3} ORDER BY _row_number_";

            string execSql = string.Format(pageSql, pageSize, sort, sql, pageSize * (pageIndex - 1));

            string countSql = string.Format(@" SELECT COUNT(0) FROM ( {0} ) t ", sql);
            using (IDbConnection conn = GetConnection(connString))
            {
                conn.Open();
                request.count = conn.Query<long>(countSql, param, commandTimeout: commandTimeout).FirstOrDefault();
                result.page = request;
                result.code =  0;
                result.msg = "";
                result.data = conn.Query<T>(execSql, param, commandTimeout: commandTimeout).ToList();
                return result;
            }

        }


    }
}
