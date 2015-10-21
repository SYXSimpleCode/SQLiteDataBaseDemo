using SQLiteConsole_Local.Model;

namespace SQLiteConsole_Local
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string createtable = @"create table user
                        (id int primarykey not null,
                         code nvarchar(50),
                         orgid int,
                         name nvarchar(50),
                         gender nvarchar(8),
                         birthday nvarchar(50),
                         u_validate int,
                         u_registtime varchar(50)
                        );";
            //OperationSQLite.CreateTable(createtable);

            //int result1=  OperationSQLite.Add();

            User u1 = OperationSQLite.GetUserById(1);
        }
    }
}