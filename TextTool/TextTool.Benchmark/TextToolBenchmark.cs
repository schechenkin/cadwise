using BenchmarkDotNet.Attributes;

namespace TextTool.Benchmark
{
    [MemoryDiagnoser]
    public class TextToolBenchmark
    {
        private string input = "";
        private string output = "";
        private FileConverter converter;

        [Params(1024, 4096)]
        public long BufferSize { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            input = Path.Combine("G:", "Projects", "Cadwise_ATM", "big.txt");
            output = Path.Combine("G:", "Projects", "Cadwise_ATM", "big_out.txt");
        }

        [IterationSetup]
        public void IterationSetup()
        {
            converter = new FileConverter(input, output, 5, false);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            File.Delete(output);
        }

        [Benchmark]
        public async Task ConvertAsync()
        {
            await converter.ConvertAsync(BufferSize);
        }

        [Benchmark]
        public void Convert()
        {
            converter.Convert(BufferSize);
        }
    }
}
