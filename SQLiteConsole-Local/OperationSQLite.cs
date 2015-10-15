using SQLiteConsole_Local.Model;
using System;

namespace SQLiteConsole_Local
{
    public class OperationSQLite
    {
        /// <summary>
        /// 添加
        /// </summary>
        public static void Add(User user)
        {
            string sql = String.Format(@"insert into user values(1,1000,1,'test1',23,'男','1990-1-1',1,'2015-10-14'); ");



        }
    }
}