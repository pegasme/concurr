using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrTests
{
    internal class Plinq
    {
        const int count = 50;
        public void Start()
        {
            var items = Enumerable.Range(1, count).ToArray();
            var result = new int[count];

            items.AsParallel().ForAll(x => {
                var newVal = x * x;
                Console.WriteLine($"{newVal}");
                result[x - 1] = newVal;
            });
        }
    }
}
