using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.Data.SqlClient;


namespace SQLiteConsole_Local
{
    /// <summary>
    /// SQL分页 Dapper
    /// </summary>
    public class PageFinder
    {
        private static string strConn = System.Configuration.ConfigurationManager.AppSettings["SQLiteCon"].ToString();

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlPage">搜索SQL</param>
        /// <param name="sqlCount">计数SQL</param>
        /// <param name="page">分页信息</param>
        /// <param name="param">搜索参数</param>
        ///  <param name="total">返回总数量</param>
        ///  <param name="tempSql">临时表SQL</param>
        ///  <param name="dropSql">如果用到临时表，可在此处销毁</param>
        /// <returns></returns>
        public static List<T> PageQuery<T>(string sqlPage, string sqlCount, PageInfo page, dynamic param, out int total, string tempSql = "", string dropSql = "")
        {
            page.sortField = page.sortField == null ? "U_REGISTIME DESC" : page.sortField;
            page.pageIndex += 1;  //MINIUI 默认起始页为0,这里每页需要加1
            string sqlPageStr = @" {3} WITH    t AS ( {0}
                                                 ),
                                            t1
                                              AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY {1} ) rowid ,
                                                            *
                                                   FROM     t
                                                 )
                                        SELECT  *
                                        FROM    t1
                                        WHERE   rowid BETWEEN @rowStart AND @rowEnd  {2}";
            sqlPageStr = String.Format(sqlPageStr, sqlPage, page.sortField, dropSql, tempSql);
            sqlCount = tempSql + sqlCount + dropSql;
            try
            {
                using (var conn = new SqlConnection(strConn))
                {
                    var pageinfo = conn.Query<PageInfo>(sqlCount, param as object).FirstOrDefault();
                    var dbArgs = new DynamicParameters(param);
                    dbArgs.Add("rowStart", page.RowStart);
                    dbArgs.Add("rowEnd", page.RowEnd);
                    List<T> list = conn.Query<T>(sqlPageStr, dbArgs).ToList();
                    if (pageinfo != null && pageinfo.TotalCount <= 0)
                    {
                        List<T> listNull = new List<T>();
                        page.TotalCount = 0;
                        total = 0;
                        return listNull;
                    }
                    if (pageinfo != null) page.TotalCount = pageinfo.TotalCount;
                    total = pageinfo.TotalCount;
                    return list.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 分页方法(仅支持2012)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlPage">搜索SQL</param>
        /// <param name="sqlCount">计数SQL</param>
        /// <param name="page">分页信息</param>
        /// <param name="param">搜索参数</param>
        ///  <param name="total">返回总数量</param>
        ///  <param name="tempSql">临时表SQL</param>
        ///  <param name="dropSql">如果用到临时表，可在此处销毁</param>
        /// <returns></returns>
        public static List<T> PageQuery_SQL2012<T>(string sqlPage, string sqlCount, PageInfo page, dynamic param, out int total, string tempSql = "", string dropSql = "")
        {
            page.sortField = page.sortField == null ? "U_REGISTIME DESC" : page.sortField;
            page.pageIndex += 1;  //MINIUI 默认起始页为0,这里每页需要加1
            int skipNum = (page.pageIndex - 1) * page.pageSize;
            string sqlPageStr = @" {3}  {0}
                                    ORDER BY {1} 
                                    OFFSET {4} ROWS
		                            FETCH NEXT {5} ROWS ONLY
                                   {2}";
            sqlPageStr = String.Format(sqlPageStr, sqlPage, page.sortField, dropSql, tempSql, skipNum, page.pageSize);
            sqlCount = tempSql + sqlCount + dropSql;
            try
            {
                using (var conn = new SqlConnection(strConn))
                {
                    var pageinfo = conn.Query<PageInfo>(sqlCount, param as object).FirstOrDefault();
                    var dbArgs = new DynamicParameters(param);
                    dbArgs.Add("rowStart", page.RowStart);
                    dbArgs.Add("rowEnd", page.RowEnd);
                    List<T> list = conn.Query<T>(sqlPageStr, dbArgs).ToList();
                    if (pageinfo != null && pageinfo.TotalCount <= 0)
                    {
                        List<T> listNull = new List<T>();
                        page.TotalCount = 0;
                        total = 0;
                        return listNull;
                    }
                    if (pageinfo != null) page.TotalCount = pageinfo.TotalCount;
                    total = pageinfo.TotalCount;
                    return list.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 查询单个实体
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
                using (var conn = new SqlConnection(strConn))
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
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> FindList<T>(string sql, dynamic param)
        {
            try
            {
                using (var conn = new SqlConnection(strConn))
                {
                    var result = new List<T>();
                    var dbArgs = new DynamicParameters(param);
                    var query = conn.Query<T>(sql, dbArgs);
                    if (query == null)
                    {
                        return result;
                    }
                    result = query.ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
