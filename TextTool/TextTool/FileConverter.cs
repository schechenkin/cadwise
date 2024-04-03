namespace TextTool
{
    internal class FileConverter
    {
        private char? _lastWrittenSymbol;
        private readonly string _source;
        private readonly string _destination;
        private readonly long _wordMinLength;
        private readonly bool _removePunctuation;
        private readonly ISymbolAnalyazer _symbolAnalyazer;

        public FileConverter(string source, string destination, long wordMinLength, bool removePunctuation, ISymbolAnalyazer symbolAnalyazer)
        {
            if (wordMinLength < 0)
                throw new ArgumentOutOfRangeException(nameof(wordMinLength), "Value cannot be negative");

            _source = source;
            _destination = destination;
            _wordMinLength = wordMinLength;
            _removePunctuation = removePunctuation;
            _symbolAnalyazer = symbolAnalyazer;
        }

        public FileConverter(string source, string destination, long wordMinLength, bool removePunctuation = false)
            :this(source, destination, wordMinLength, removePunctuation, new SimpleSymbolAnalyazer())
        {
        }

        public void Convert(long bufferSize = 4096)
        {
            using (var reader = new StreamReader(_source))
            using (var writer = new BufferedCharFileWriter(_destination, reader.CurrentEncoding, bufferSize))
            {
                bool anyWordConfirmed = false;

                while (!reader.EndOfStream)
                {
                    char symbol = (char)reader.Peek();

                    if (_symbolAnalyazer.IsWordSymbol(symbol))
                    {
                        var wordLength = WriteWholeWordToOutput(reader, writer);
                        if(wordLength < _wordMinLength)
                            writer.SeekBack(wordLength);
                        else
                            anyWordConfirmed = true;

                    }
                    else
                    {
                        reader.Read();

                        if (_symbolAnalyazer.IsPunctuation(symbol))
                        {
                            if (_removePunctuation)
                            {
                                var nextSymbol = reader.Peek();
                                if (anyWordConfirmed && _lastWrittenSymbol != ' ' && nextSymbol != -1 && _symbolAnalyazer.IsWordSymbol((char)nextSymbol))
                                    Write(' ', writer);
                            }
                            else
                            {
                                Write(symbol, writer);
                            }
                        }
                        else
                        {
                            Write(symbol, writer);
                        }
                    }
                }
            }
        }

        // only for demo
        public async Task ConvertAsync(long bufferSize = 4096)
        {
            using (var reader = new FileReader(_source, bufferSize))
            using (var writer = new BufferedCharFileWriter(_destination, reader.CurrentEncoding, bufferSize))
            {
                bool anyWordConfirmed = false;

                while (!reader.EndOfStream)
                {
                    char symbol = (char)await reader.PeekAsync();

                    if (_symbolAnalyazer.IsWordSymbol(symbol))
                    {
                        var wordLength = await WriteWholeWordToOutputAsync(reader, writer);
                        if (wordLength < _wordMinLength)
                            writer.SeekBack(wordLength);
                        else
                            anyWordConfirmed = true;

                    }
                    else
                    {
                        await reader.ReadAsync();

                        if (_symbolAnalyazer.IsPunctuation(symbol))
                        {
                            if (_removePunctuation)
                            {
                                var nextSymbol = await reader.PeekAsync();
                                if (anyWordConfirmed && _lastWrittenSymbol != ' ' && nextSymbol != -1 && _symbolAnalyazer.IsWordSymbol((char)nextSymbol))
                                    await WriteAsync(' ', writer);
                            }
                            else
                            {
                                await WriteAsync(symbol, writer);
                            }
                        }
                        else
                        {
                            await WriteAsync(symbol, writer);
                        }
                    }
                }
            }
        }

        private long WriteWholeWordToOutput(StreamReader reader, BufferedCharFileWriter writer)
        {
            long length = 0;

            while (!reader.EndOfStream && _symbolAnalyazer.IsWordSymbol((char)reader.Peek()))
            {
                char symbol = (char)reader.Read();
                writer.Write(symbol);
                length++;
            }

            return length;
        }

        // only for demo
        private async ValueTask<long> WriteWholeWordToOutputAsync(FileReader reader, BufferedCharFileWriter writer)
        {
            long length = 0;

            while (!reader.EndOfStream && _symbolAnalyazer.IsWordSymbol((char) await reader.PeekAsync()))
            {
                char symbol = (char) await reader.ReadAsync();
                await writer.WriteAsync(symbol);
                length++;
            }

            return length;
        }

        private void Write(char symbol, BufferedCharFileWriter writer)
        {
            writer.Write(symbol);
            _lastWrittenSymbol = symbol;
        }

        // only for demo
        private ValueTask WriteAsync(char symbol, BufferedCharFileWriter writer)
        {
            _lastWrittenSymbol = symbol;
            return writer.WriteAsync(symbol);
        }
    }
}
