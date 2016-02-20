using NBench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopscotchHashMap
{
    public class CounterPerfSpecs
    {
        private HopscotchMap<int, int> map;
        private Random random;
        private Counter overflows;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            map = new HopscotchMap<int, int>(1000, 4);
            random = new Random(0);
            overflows = context.GetCounter("Overflows");
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 100, RunTimeMilliseconds = 2000, RunMode = RunMode.Throughput, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [CounterMeasurement("Overflows")]
        public void Benchmark()
        {
            int key = random.Next();
            if (map.PutIfAbsent(key, key) == PutResult.Overflow)
                overflows.Increment();
        }

        [PerfCleanup]
        public void Cleanup()
        {
        }
    }
}
