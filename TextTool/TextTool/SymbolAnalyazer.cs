namespace TextTool
{
    internal interface ISymbolAnalyazer
    {
        bool IsWordSymbol(char symbol);
        bool IsPunctuation(char symbol);
        bool IsDelimiter(char symbol);
    }

    internal class SimpleSymbolAnalyazer : ISymbolAnalyazer
    {
        public bool IsDelimiter(char symbol)
        {
            return char.IsPunctuation(symbol) || char.IsWhiteSpace(symbol) || char.IsSeparator(symbol);
        }

        public bool IsPunctuation(char symbol)
        {
            return char.IsPunctuation(symbol);
        }

        public bool IsWordSymbol(char symbol)
        {
            return !IsDelimiter(symbol);
        }
    }
}
