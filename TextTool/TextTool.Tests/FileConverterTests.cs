using FluentAssertions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TextTool.Tests
{
    public class FileConverterTests
    {
        [Theory]
        [InlineData("", "", 2, false)]
        [InlineData("", "", 2, true)]
        [InlineData("", "", 0, true)]
        [InlineData("Tom!", "Tom", 0, true)]
        [InlineData("Tom!", "Tom", 3, true)]
        [InlineData("Tom!", "!", 4, false)]
        [InlineData("Peter!", "Peter!", 4, false)]
        [InlineData("hi !", "hi ", 1, true)]
        [InlineData("one,two,three,four,five,eleven", "eleven", 6, true)]
        [InlineData("one,two,three, four", ",,three, four", 4, false)]
        [InlineData("one,two,three, four", "three four", 4, true)]
        [InlineData("my name is Peter!", " name  Peter!", 4, false)]
        [InlineData("12345,1234567890,1234567", ",1234567890,1234567", 7, false)]
        [InlineData("12345,1234567890,1234567", "1234567890 1234567", 7, true)]
        [InlineData("12345,1234567890 ,1234567", "1234567890 1234567", 7, true)]

        [InlineData("one  two", "one  two", 2, false)]
        [InlineData("one    two", "one    two", 2, false)]
        [InlineData("one  two", "one  two", 2, true)]
        [InlineData("one    two", "one    two", 2, true)]

        [InlineData("123 1234!", " 1234!", 4, false)]
        [InlineData("123 1234!", " 1234", 4, true)]
        public void ConvertTest(string input, string expected, int wordMinLength, bool removePunctuation)
        {
            //Given
            string sourceFile = "input.txt";
            string destinationFile = "output.txt";
            CreateFile(sourceFile, input);
            EnsureFileNotExists(destinationFile);

            //When
            new FileConverter(sourceFile, destinationFile, wordMinLength, removePunctuation).Convert();

            //Then
            var result = File.ReadAllText(destinationFile);
            result.Should().Be(expected);
        }

        private void EnsureFileNotExists(string path)
        {
            if(File.Exists(path))
                File.Delete(path);
        }

        private void CreateFile(string filePath, string content)
        {
            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                outputFile.Write(content);
            }
        }
    }
}