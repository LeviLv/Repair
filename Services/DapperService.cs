using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Repair.Entitys;
using Repair.Models;

namespace Repair.Services
{
    public class DapperService
    {
        private static readonly string connectionString =
            "Data Source=47.92.194.55;Database=Repair;User ID=root;Password=Vitaminb88;pooling=true;CharSet=utf8;port=3306;sslmode=none";

        public static async Task<object> PageList(string sql, PageBase pageBase)
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var start = pageBase.PageIndex * pageBase.PageSize;
            var end = pageBase.PageSize;
            sql = $"select * from (${sql}) limit ${start},${end}";
            var list = await conn.QueryAsync(sql);
            return list;
        }

        public static async Task<QueryResult<T>> PageList<T>(string sql, PageBase pageBase, string groupBy = "") where T : class
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var start = (pageBase.PageIndex - 1) * pageBase.PageSize;
            var end = pageBase.PageSize;
            var countSql = $"select count(1) from ({sql}) as c ";
            sql = $" {sql} {groupBy} limit {start},{end} ";
            var list = await conn.QueryAsync<T>(sql);
            var count = await conn.ExecuteScalarAsync<int>(countSql);

            var result = new QueryResult<T>();
            result.List = list.ToList();
            result.Total = count;
            return result;
        }

        public static async Task<IEnumerable<T>> Query<T>(string sql)
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var list = await conn.QueryAsync<T>(sql);
            return list;
        }

        public static async Task<T> QueryFirst<T>(string sql)
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var list = await conn.QueryFirstAsync<T>(sql);
            await conn.CloseAsync();
            return list;
        }

        public static async Task Execute(string sql)
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            var list = await conn.ExecuteAsync(sql);
            await conn.CloseAsync();
        }
    }
}