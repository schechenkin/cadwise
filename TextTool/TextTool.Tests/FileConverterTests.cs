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
        [InlineData("hi", "hi", 2, false)]
        [InlineData("hello", "", 2, false)]
        [InlineData("hi!", "hi!", 2, false)]
        [InlineData("hi !", "hi !", 2, false)]
        [InlineData("my name is Peter!", "my  is !", 2, false)]
        [InlineData("my name is Peter!", "my name is !", 4, false)]
        [InlineData("my name is Peter!", "my name is Peter!", 5, false)]
        [InlineData("one,two,three", "one,two,three", 5, false)]

        [InlineData("hi!", "hi", 2, true)]
        [InlineData("hi !", "hi ", 2, true)]
        [InlineData("my name is Peter!", "my  is ", 2, true)]
        [InlineData("one,two,three", "one two three", 5, true)]
        [InlineData("one,,two", "one two", 3, true)]
        [InlineData("hi,f!d", ",!", 0, false)]
        [InlineData("hi,f!d", "", 0, true)]

        [InlineData("one  two", "one  two", 3, false)]
        [InlineData("one    two", "one    two", 3, false)]
        public void ConvertTest(string input, string expected, int wordMaxLength, bool removePunctuation)
        {
            //Given
            string sourceFile = "input.txt";
            string destinationFile = "output.txt";
            CreateFile(sourceFile, input);

            //When
            new FileConverter(sourceFile, destinationFile, wordMaxLength, removePunctuation).Convert();

            //Then
            var result = File.ReadAllText(destinationFile);
            result.Should().Be(expected);

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