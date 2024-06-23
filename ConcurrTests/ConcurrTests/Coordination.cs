using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrTests
{
    internal class Coordination
    {
        static Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished");
        });

        public static void Water()
        {
            Console.WriteLine("kettle");
            Thread.Sleep(2000);
            barrier.SignalAndWait();
        }

        public static void Cup()
        {
            Console.WriteLine("Find cup");
            barrier.SignalAndWait(); // signal wait
            Console.WriteLine("Add water");
        }

        static CountdownEvent ce = new CountdownEvent(5);
        static Random random = new Random();

        public void Start()
        {
            var water = Task.Factory.StartNew(Water);
            var cup = Task.Factory.StartNew(Cup);

            var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks =>
            {
                Console.WriteLine("Enjoy");
            });

            for (int i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    Thread.Sleep(random.Next(3000));
                    ce.Signal();
                    Console.WriteLine($"Exiting task {Task.CurrentId}");
                });
            }

            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting other to complete");
                ce.Wait();
                Console.WriteLine("All is done");
            });
        }

        public void Start2()
        {
            var evt = new ManualResetEventSlim();

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 1");
                Thread.Sleep(1000);
                evt.Set();
                Thread.Sleep(1000);
                Console.WriteLine("Task 1part 2");
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Start task 2");
                evt.Wait();
                evt.Wait();
                evt.Wait();
                Console.WriteLine("Complete task 2");
            });
        }

        public void Start3()
        {
            var evt = new AutoResetEvent(false);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 1");
                Thread.Sleep(1000);
                evt.Set();
                Thread.Sleep(1000);
                Console.WriteLine("Task 1part 2");
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Start task 2");
                evt.WaitOne();
                var ok = evt.WaitOne(1000);
                if (!ok)
                {
                    Console.WriteLine("Oops");
                }
                Console.WriteLine("Complete task 2");
            });
        }

        public void Semaphor()
        {
            var semathor = new SemaphoreSlim(2, 10);

            for (int i = 0; i < 20; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Start Task {i} - {Task.CurrentId}");
                    semathor.Wait();
                    Console.WriteLine($"Processing Task {i} - {Task.CurrentId}");
                });
            }

            while (semathor.CurrentCount <= 2)
            {
                Console.WriteLine($"{semathor.CurrentCount}");
                Console.ReadKey();
                semathor.Release(12);
            }

        }
    }
}
