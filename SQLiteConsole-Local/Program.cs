using SQLiteConsole_Local.Model;
using System;

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

            #region 操作sqlite

            //OperationSQLite.CreateTable(createtable);

            //int result1=  OperationSQLite.Add();

            #endregion 操作sqlite

            #region dapper 操作

            User u1 = OperationSQLite.GetUserById(1);

            #endregion dapper 操作

            #region 线程操作 -lock

            //实例1
            //Thread[] threads = new Thread[10];
            //TestLock acc = new TestLock(1000);
            //for (int i = 0; i < 10; i++)
            //{
            //    Thread t = new Thread(new ThreadStart(acc.DoTransactions));
            //    t.Name = "线程：" + i.ToString();
            //    threads[i] = t;
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    threads[i].Start();
            //}

            //实例2
            //在t1线程中调用LockMethod，并将deadlock设置为true,(将出现死锁)
            /*
            TestLock_LockThis testLockLockThis =new TestLock_LockThis();
            Thread t1=new Thread(testLockLockThis.LockMethod);
            t1.Start(true);
            Thread.Sleep(100);

            lock (testLockLockThis)
            {
                //调用没有被lock的方法
                testLockLockThis.NotLockMethod();
                //调用被lock的方法，并试图将deadlock解除
                testLockLockThis.LockMethod(false);
            }*/

            #endregion 线程操作 -lock

            //获取中文首字母
            string str = "C# 获取中文大写字母,长大了,长租,长城汽车,仇姓";

            string spell = ToChineseSpell.GetChineseSpell(str);

            Console.WriteLine(spell);

            Console.WriteLine(ChineseSpell.convert(str));

            Console.ReadLine();
        }
    }
}