using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrTests
{
    internal class LinkedTokenSource
    {
        public void Start()
        {
            var basic = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            var panic = CancellationTokenSource.CreateLinkedTokenSource(basic.Token, emergency.Token);
            var token = panic.Token;

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Panic is requested");
                        break;
                    }
                    Console.WriteLine($"{i++}");
                }
            }, token);

            Console.ReadKey();
            basic.Cancel();
        }
    }
}
