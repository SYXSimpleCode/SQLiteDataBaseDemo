using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SQLiteConsole_Local
{
    public class SqliteDbHelper
    {
        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection GetSQLiteConnection()
        {
            //1,web
            // return new SQLiteConnection("Data Source=" + System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["db"].ToString()));

            //2,控制台
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = myAssembly.Location;
            DirectoryInfo dr = new DirectoryInfo(path);
            var p = dr.Parent.Parent.Parent;  //当前项目地址
            string p1 = p.FullName;
            return new SQLiteConnection("Data Source=" + p1 + @"\db" + System.Configuration.ConfigurationManager.AppSettings["SQLiteCon"].ToString());
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, params object[] p)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (p != null)
            {
                foreach (object parm in p)
                    cmd.Parameters.AddWithValue(string.Empty, parm);
            }
        }

        /// <summary>
        /// 查询单个实体--dapper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tempSql"></param>
        /// <param name="dropSql"></param>
        /// <returns></returns>
        public static T FindSingle<T>(string sql, dynamic param)
        {
            try
            {
                using (var conn = GetSQLiteConnection())
                {
                    T result;
                    var dbArgs = new DynamicParameters(param);
                    result = conn.Query<T>(sql, dbArgs).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string cmdText, params object[] p)
        {
            DataSet ds = new DataSet();
            SQLiteCommand command = new SQLiteCommand();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                da.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="cmdText">a</param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params object[] commandParameters)
        {
            SQLiteCommand command = new SQLiteCommand();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                PrepareCommand(command, connection, cmdText, commandParameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 返回SqlDataReader对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string cmdText, params object[] commandParameters)
        {
            SQLiteCommand command = new SQLiteCommand();
            SQLiteConnection connection = GetSQLiteConnection();
            try
            {
                PrepareCommand(command, connection, cmdText, commandParameters);
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params object[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                PrepareCommand(cmd, connection, cmdText, commandParameters);
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cmdText"></param>
        /// <param name="countText"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params object[] p)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, p).ToString());
            DataSet ds = new DataSet();
            SQLiteCommand command = new SQLiteCommand();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                PrepareCommand(command, connection, cmdText, p);
                SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            }
            return ds;
        }

        #region advanced method

        /// <summary>
        /// 获取表所有数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <returns>表所有数据</returns>
        public static List<T> GetTableData<T>(string tableName) where T : class
        {
            List<T> dataList = new List<T>();
            try
            {
                using (SQLiteDataContext context = new SQLiteDataContext(GetSQLiteConnection()))
                {
                    string sql = "select * from " + tableName;
                    dataList = context.ExecuteQuery<T>(sql).ToList();
                    context.SubmitChanges();
                }
            }
            catch { }
            return dataList;
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="cmdText">sql语句</param>
        /// <param name="parameter">参数</param>
        /// <returns>表所有数据</returns>
        public static List<T> GetTableData<T>(string cmdText, params object[] parameter) where T : class
        {
            List<T> dataList = new List<T>();
            try
            {
                using (SQLiteDataContext context = new SQLiteDataContext(GetSQLiteConnection()))
                {
                    dataList = context.ExecuteQuery<T>(cmdText, parameter).ToList();
                }
            }
            catch { }
            return dataList;
        }

        /// <summary>
        /// 插入表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="dataList">数据集合</param>
        /// <returns>true或false</returns>
        public static bool BatchInsert<T>(string tableName, List<T> dataList)
        {
            try
            {
                if (dataList != null && dataList.Count > 0)
                {
                    var temp = dataList[0];
                    PropertyInfo[] propertyInfos = temp.GetType().GetProperties();
                    List<string> propertyStrs = new List<string>();
                    string propertyStr = "";
                    foreach (var propertyInfo in propertyInfos)
                    {
                        propertyStrs.Add(propertyInfo.Name);
                        propertyStr = propertyStr + "@" + propertyInfo.Name + ",";
                    }
                    propertyStr = propertyStr.Remove(propertyStr.Length - 1);

                    using (SQLiteConnection conn = GetSQLiteConnection())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(conn))
                        {
                            command.Connection.Open();
                            using (SQLiteTransaction transaction = conn.BeginTransaction())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "insert into " + tableName + " values(" + propertyStr + ")";
                                foreach (var needInsertData in dataList)
                                {
                                    command.Parameters.Clear();
                                    for (int i = 0; i < propertyStrs.Count; i++)
                                    {
                                        command.Parameters.AddWithValue("@" + propertyStrs[i], propertyInfos[i].GetValue(needInsertData, null));
                                    }
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>true或false</returns>
        public static bool DeleteTableData(string tableName)
        {
            try
            {
                using (SQLiteConnection conn = GetSQLiteConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        command.Connection.Open();
                        command.CommandText = "delete from " + tableName;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion advanced method
    }

    /// <summary>
    /// Linq to SQLite
    /// </summary>
    public class SQLiteDataContext : DataContext
    {
        public SQLiteDataContext(string connection, MappingSource mappingSource) :
            base(connection, mappingSource)
        {
        }

        public SQLiteDataContext(IDbConnection connection, MappingSource mappingSource) :
            base(connection, mappingSource)
        {
        }

        public SQLiteDataContext(string connectionString) :
            base(new SQLiteConnection(connectionString))
        {
        }

        public SQLiteDataContext(IDbConnection connection) :
            base(connection)
        {
        }
    }
}