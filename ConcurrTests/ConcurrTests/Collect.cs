using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrTests
{
    internal class Collect
    {
        public Collect()
        {
            _cts = new CancellationTokenSource();
        }

        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        public static void AddParis()
        {
            var success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? $"task {Task.CurrentId}" : "main";
            Console.WriteLine($"{who} {(success ? "added" : "failed")} Paris element");
        }
        
        internal void Start()
        {
            Task.Factory.StartNew(() => AddParis());
            AddParis();
        }

        static BlockingCollection<int> messages = new BlockingCollection<int>(new ConcurrentBag<int>(), 10);
        private readonly CancellationTokenSource _cts;
        static Random random = new Random();

        internal void Consume()
        {
            var producer = Task.Factory.StartNew(RunProducer);
            var consumer = Task.Factory.StartNew(RunConsumer);

            try
            {
                Task.WaitAll(producer, consumer);

            }
            catch (AggregateException e)
            {
                e.Handle(e => true);
            }

            Console.ReadKey();
            _cts.Cancel();
        }

        private void RunProducer()
        {
            while (true)
            {
                _cts.Token.ThrowIfCancellationRequested();
                int i = random.Next(100);
                messages.Add(i);
                Console.WriteLine($"{i} was added in thread {Task.CurrentId}\t");
                Thread.Sleep(random.Next(1000));
            }
        }

        private void RunConsumer()
        {
            foreach (var item in messages.GetConsumingEnumerable())
            {
                _cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"--{item}");
                Thread.Sleep(random.Next(1000));
            }
        }
    }
}
