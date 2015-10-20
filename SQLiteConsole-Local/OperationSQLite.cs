using SQLiteConsole_Local.Model;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace SQLiteConsole_Local
{
    public class OperationSQLite
    {
        public static int CreateTable(string createtable)
        {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string binpath = myAssembly.Location;
            DirectoryInfo dr = new DirectoryInfo(binpath);
            var path = dr.Parent.Parent.Parent;  //当前项目地址
            string con = "Data Source=" + path.FullName + @"\db" + System.Configuration.ConfigurationManager.AppSettings["SQLiteCon"].ToString();

           int result = SqliteDbHelper.ExecuteNonQuery(createtable, new SQLiteParameter() { });

            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public static int Add()
        {
            string sql = String.Format(@"insert into user values(14,1006,8,'test123','女','1993-1-1',1,{0}); ",DateTime.Now.ToShortDateString());

            return SqliteDbHelper.ExecuteNonQuery(sql);
        }

        public static User GetUserById(int id)
        {
            string sql = string.Format("select * from user where id=@id");
            return SqliteDbHelper.FindSingle<User>(sql, new { id = id});
        }

        public static int Edit(int id)
        {
            string sql = string.Format("");
            return SqliteDbHelper.ExecuteNonQuery(sql);
        }


    }
}