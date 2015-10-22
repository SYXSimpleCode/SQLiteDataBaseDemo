using System;

namespace SQLiteConsole_Local
{
    /// <summary>
    /// 测试lock
    /// </summary>
    public class TestLock
    {
        private static readonly object thislock = new object();

        private int balance;

        private Random r = new Random();

        public TestLock(int initial)
        {
            balance = initial;
        }

        public int Withdraw(int amount)
        {
            // This condition will never be true unless the lock statement
            // is commented out:
            if (amount < 0)
            {
                throw new Exception("Nagative Balance");
            }
            // Comment out the next line to see the effect of leaving out
            // the lock keyword:
            lock (thislock)
            {
                if (balance >= amount)
                {
                    Console.WriteLine("-------:" + System.Threading.Thread.CurrentThread.Name + "-----");
                    Console.WriteLine(System.Threading.Thread.CurrentThread.Name + ":" + "Balance before Withdrawal:" + balance);
                    balance = balance - amount;
                    Console.WriteLine(System.Threading.Thread.CurrentThread.Name + ":" + "Balance after Withdrawal:" + balance);
                    return amount;
                }
                else
                {
                    return 0;//transaction rejected
                }
            }
        }

        public void DoTransactions()
        {
            for (int i = 0; i < 100; i++)
            {
                Withdraw(r.Next(1, 100));
            }
        }
    }
}