using System;
using System.Threading;

public class Deadlock
{
    static readonly object _lock1 = new object();
    public static void Test()
    {
            lock (_lock1)
            {
                var t = new Thread(() => {
                    lock (_lock1)
                    {
                        Console.WriteLine("Lock1: ");
                    }
                });
                t.Start();
                t.Join();
            }
            Console.ReadKey();
    }
}