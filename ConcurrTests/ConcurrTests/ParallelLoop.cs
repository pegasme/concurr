using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrTests
{
    internal class ParallelLoop
    {
        public void Start()
        {
            int[] values = new int[100];
            var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
            var b = new Action(() => Console.WriteLine($"Second {Task.CurrentId}"));
            var c = new Action(() => Console.WriteLine($"Third {Task.CurrentId}"));

            Parallel.Invoke(a, b, c);

            Console.WriteLine("ForEach");

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3,
            };

            Parallel.ForEach(Range(1, 20, 3), Console.WriteLine);

            Console.WriteLine("ForEach");

            var result = Parallel.For(1, 20, (x, state) =>
            {
                Console.WriteLine($"{x} in thread id {Task.CurrentId}");
                if (x == 10)
                {
                    state.Break();
                }
            });

            Console.WriteLine($"IsCompleted: {result.IsCompleted} {result.LowestBreakIteration }");

        }

        private IEnumerable<int> Range(int start, int end, int v3)
        {
            for (int i = start; i < end; i+= v3)
            {
                yield return i;
            }
        }
    }
}
