using System;
using System.Threading;

namespace SQLiteConsole_Local
{
    public class TestLock_LockThis
    {
        private bool deadlocked = true;
        private static readonly object lockobject = new object();

        //locke的代码在同一时刻只能有一个线程访问
        public void LockMethod(object o)
        {
            //lock (this)
            lock (lockobject)
            {
                while (deadlocked)
                {
                    deadlocked = (bool)o;
                    Console.WriteLine("I am locked");
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// 所有线程都可以同时访问的方法
        /// </summary>
        public void NotLockMethod()
        {
            Console.WriteLine("I am not Locked");
        }
    }
}