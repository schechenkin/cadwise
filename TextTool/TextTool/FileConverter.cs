namespace TextTool
{
    internal class FileConverter
    {
        private char? _lastWrittenSymbol;
        private readonly string _source;
        private readonly string _destination;
        private readonly int _wordMaxLength;
        private readonly bool _removePunctuation;
        private readonly ISymbolAnalyazer _symbolAnalyazer;

        public FileConverter(string source, string destination, int wordMaxLength, bool removePunctuation, ISymbolAnalyazer symbolAnalyazer)
        {
            if (wordMaxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(wordMaxLength), "Value cannot be negative");

            _source = source;
            _destination = destination;
            _wordMaxLength = wordMaxLength;
            _removePunctuation = removePunctuation;
            _symbolAnalyazer = symbolAnalyazer;
        }

        public FileConverter(string source, string destination, int wordMaxLength, bool removePunctuation = false)
            :this(source, destination, wordMaxLength, removePunctuation, new SimpleSymbolAnalyazer())
        {
        }

        public void Convert()
        {
            using (StreamReader reader = new StreamReader(_source))
            using (StreamWriter writer = new StreamWriter(_destination, false, reader.CurrentEncoding))
            {
                WordBuffer wordBuffer = new WordBuffer(_wordMaxLength);

                while (!reader.EndOfStream)
                {
                    char symbol = (char)reader.Read();

                    if (_symbolAnalyazer.IsWordSymbol(symbol))
                    {
                        if (!wordBuffer.TryAddSymbol(symbol))
                        {
                            ReadRestOfWord(reader);
                            wordBuffer.Reset();
                        }
                    }
                    else
                    {
                        Write(wordBuffer, writer);
                        wordBuffer.Reset();

                        if (_symbolAnalyazer.IsPunctuation(symbol))
                        {
                            ProcessPunctuationSymbol(symbol, _removePunctuation, reader, writer);
                        }
                        else
                        {
                            Write(symbol, writer);
                        }
                    }
                }

                Write(wordBuffer, writer);
            }
        }

        public async Task ConvertAsync()
        {
            using (FileReader reader = new FileReader(_source))
            using (StreamWriter writer = new StreamWriter(_destination, false, reader.CurrentEncoding))
            {
                WordBuffer wordBuffer = new WordBuffer(_wordMaxLength);

                while (!reader.EndOfStream)
                {
                    char symbol = (char)await reader.ReadAsync();

                    if (_symbolAnalyazer.IsWordSymbol(symbol))
                    {
                        if (!wordBuffer.TryAddSymbol(symbol))
                        {
                            await ReadRestOfWordAsync(reader);
                            wordBuffer.Reset();
                        }
                    }
                    else
                    {
                        Write(wordBuffer, writer);
                        wordBuffer.Reset();

                        if (_symbolAnalyazer.IsPunctuation(symbol))
                        {
                            await ProcessPunctuationSymbolAsync(symbol, _removePunctuation, reader, writer);
                        }
                        else
                        {
                            Write(symbol, writer);
                        }
                    }
                }

                Write(wordBuffer, writer);
            }
        }

        private async Task ProcessPunctuationSymbolAsync(char symbol, bool removePunctuation, FileReader reader, StreamWriter writer)
        {
            if (removePunctuation == false)
            {
                Write(symbol, writer);
            }
            else
            {
                var nextSymbol = await reader.PeekAsync();
                if (_lastWrittenSymbol != null && _symbolAnalyazer.IsWordSymbol(_lastWrittenSymbol.Value) && nextSymbol != -1 && _symbolAnalyazer.IsWordSymbol((char)nextSymbol))
                {
                    Write(' ', writer);
                }
            }
        }

        private void ProcessPunctuationSymbol(char symbol, bool removePunctuation, StreamReader reader, StreamWriter writer)
        {
            if (removePunctuation == false)
            {
                Write(symbol, writer);
            }
            else
            {
                var nextSymbol = reader.Peek();
                if (_lastWrittenSymbol != null && _symbolAnalyazer.IsWordSymbol(_lastWrittenSymbol.Value) && nextSymbol != -1 && _symbolAnalyazer.IsWordSymbol((char)nextSymbol))
                {
                    Write(' ', writer);
                }
            }
        }
        private void Write(WordBuffer wordBuffer, StreamWriter writer)
        {
            if (wordBuffer.Length > 0)
            {
                writer.Write(wordBuffer.Buffer);
                _lastWrittenSymbol = wordBuffer.LastSymbol!.Value;
            }
        }

        private void Write(char symbol, StreamWriter writer)
        {
            writer.Write(symbol);
            _lastWrittenSymbol = symbol;
        }

        private async ValueTask ReadRestOfWordAsync(FileReader reader)
        {
            while (!reader.EndOfStream)
            {
                char symbol = (char)await reader.PeekAsync();
                if (_symbolAnalyazer.IsDelimiter(symbol))
                    return;
                else
                    await reader.ReadAsync();
            }
        }

        private void ReadRestOfWord(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                char symbol = (char)reader.Peek();
                if (_symbolAnalyazer.IsDelimiter(symbol))
                    return;
                else
                    reader.Read();
            }
        }

        private async ValueTask WriteAsync(WordBuffer wordBuffer, StreamWriter writer)
        {
            if (wordBuffer.Length > 0)
            {
                await writer.WriteAsync(wordBuffer.Memory);
                _lastWrittenSymbol = wordBuffer.LastSymbol!.Value;
            }
        }

        private async ValueTask WriteAsync(char symbol, StreamWriter writer)
        {
            await writer.WriteAsync(symbol);
            _lastWrittenSymbol = symbol;
        }

    }
}
