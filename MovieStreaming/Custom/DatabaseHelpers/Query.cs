//using Dapper;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MovieStreaming.Custom.DatabaseHelpers
//{
//    public class Query
//    {
//        private string ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
//        private int TimeOut = 30 * 60;

//        public async Task<Dictionary<T1, T2>> SelectDictionary<T1, T2>(string query, object parameters = null)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return db.Query(query, parameters).ToDictionary(row => (T1)row.Key, row => (T2)row.Value);
//                }
//            }
//            catch (Exception error)
//            {
//                return null;
//            }
//        }

//        public async Task<DatabaseResult<T>> Select<T>(string query, object parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseResult<T>() { Result = await db.QueryAsync<T>(query, parameters, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseResult<T>() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseResult<T>> Select<T>(string query)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseResult<T>() { Result = await db.QueryAsync<T>(query, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseResult<T>() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseSingleResult<T>> SelectSingle<T>(string query, object parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseSingleResult<T>() { Result = await db.QuerySingleOrDefaultAsync<T>(query, parameters, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseSingleResult<T>() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseSingleResult<T>> SelectSingle<T>(string query)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseSingleResult<T>() { Result = await db.QuerySingleOrDefaultAsync<T>(query, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseSingleResult<T>() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseExecuteResult> Execute(string query, object parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseExecuteResult() { Count = await db.ExecuteAsync(query, parameters, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseExecuteResult() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseExecuteResult> Execute(string query, DynamicParameters parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseExecuteResult() { Count = await db.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseExecuteResult() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public async Task<DatabaseExecuteResult> Execute(string query)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return new DatabaseExecuteResult() { Count = await db.ExecuteAsync(query, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseExecuteResult() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        public DatabaseSingleResult<T> SelectSingleSync<T>(string query, object parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    db.OpenAsync();
//                    return new DatabaseSingleResult<T>() { Result = db.QuerySingleOrDefault<T>(query, parameters, commandTimeout: TimeOut) };
//                }
//            }
//            catch (Exception error)
//            {
//                return new DatabaseSingleResult<T>() { ErrorMessage = error.ToString(), HasError = true };
//            }
//        }

//        internal object Select<T>(DatabaseResult<T> databaseResult)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<int> ExecuteAndGetInsId(string query, object parameters)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return db.Query<int>(query, parameters, commandTimeout: TimeOut).Single();
//                }
//            }
//            catch (Exception error)
//            {
//                return 0;
//            }
//        }
//        public async Task<int> ExecuteAndGetInsId(string query)
//        {
//            try
//            {
//                using (var db = new SqlConnection(ConnectionString))
//                {
//                    await db.OpenAsync();
//                    return db.Query<int>(query, commandTimeout: TimeOut).Single();
//                }
//            }
//            catch (Exception error)
//            {
//                return 0;
//            }
//        }
//    }
//}
