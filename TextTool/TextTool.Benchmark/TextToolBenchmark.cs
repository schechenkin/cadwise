using BenchmarkDotNet.Attributes;

namespace TextTool.Benchmark
{
    [MemoryDiagnoser]
    public class TextToolBenchmark
    {
        private string input = "";
        private string output = "";
        private FileConverter converter;

        [GlobalSetup]
        public void Setup()
        {
            input = Path.Combine("G:", "Projects", "Cadwise", "big.txt");
            output = Path.Combine("G:", "Projects", "Cadwise", "big_out.txt");
        }

        [IterationSetup]
        public void IterationSetup()
        {
            converter = new FileConverter(input, output, 6, false);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            File.Delete(output);
        }

        [Benchmark]
        public async Task ConvertAsync()
        {
            await converter.ConvertAsync();
        }

        [Benchmark]
        public void Conver()
        {
            converter.Convert();
        }
    }
}
