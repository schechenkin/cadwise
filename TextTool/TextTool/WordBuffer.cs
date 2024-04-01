


namespace TextTool
{
    internal class WordBuffer
    {
        char[] _buffer;
        int _length;

        public WordBuffer(int capacity) 
        {
            _buffer = new char[capacity];
        }

        public bool TryAddSymbol(char symbol)
        {
            if(_length < _buffer.Length)
            {
                _buffer[_length] = symbol;
                _length++;
                return true;
            }
            return false;
        }

        internal void Reset()
        {
            _length = 0;
        }

        public int Length => _length;

        public ReadOnlySpan<char> Buffer => new ReadOnlySpan<char>(_buffer, 0, _length);
        public ReadOnlyMemory<char> Memory => new ReadOnlyMemory<char>(_buffer, 0, _length);

        public char? LastSymbol
        {
            get
            {
                if(_length > 0)
                    return _buffer[_length - 1];
                return null;
            }
        }
    }
}
