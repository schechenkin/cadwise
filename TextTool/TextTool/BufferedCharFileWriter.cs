using System.Text;

namespace TextTool
{
    internal class BufferedCharFileWriter : IDisposable
    {
        readonly FileStream _fileStream;
        readonly Encoding _encoding;
        char[] _charBuffer;
        int _charBufferCount = 0;
        byte[]? _byteBuffer;

        public BufferedCharFileWriter(string path, Encoding encoding, long bufferSize = 1024) 
        {
            _fileStream = new FileStream(path: path, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.CreateNew });
            _encoding = encoding;
            _charBuffer = new char[bufferSize];
        }

        public void Write(char symbol)
        {
            if (_charBufferCount < _charBuffer.Length)
            {
                _charBuffer[_charBufferCount] = symbol;
                _charBufferCount++;
            }
            else
            {
                WriteCharBufferToFileStream();

                _charBuffer[0] = symbol;
                _charBufferCount = 1;
            }
        }

        // only for demo
        public async ValueTask WriteAsync(char symbol)
        {
            if (_charBufferCount < _charBuffer.Length)
            {
                _charBuffer[_charBufferCount] = symbol;
                _charBufferCount++;
            }
            else
            {
                await WriteCharBufferToFileStreamAsync();

                _charBuffer[0] = symbol;
                _charBufferCount = 1;
            }
        }

        public void SeekBack(long bytesCount)
        {
            if (bytesCount < 0)
                throw new ArgumentOutOfRangeException(nameof(bytesCount), "Value cannot be negative");

            if (_charBufferCount - bytesCount >= 0)
            {
                _charBufferCount -= (int)bytesCount;
            }
            else
            {
                _fileStream.Seek(-(bytesCount - _charBufferCount), SeekOrigin.End);
                _charBufferCount = 0;
            }
        }

        private void WriteCharBufferToFileStream()
        {
            scoped Span<byte> byteBuffer;
            if (_byteBuffer is not null)
            {
                byteBuffer = _byteBuffer;
            }
            else
            {
                int maxBytesForCharPos = _encoding.GetMaxByteCount(_charBufferCount);
                byteBuffer = maxBytesForCharPos <= 1024 ?
                    stackalloc byte[1024] :
                    (_byteBuffer = new byte[_encoding.GetMaxByteCount(_charBuffer.Length)]);
            }

            int count = _encoding.GetBytes(new ReadOnlySpan<char>(_charBuffer, 0, _charBufferCount), byteBuffer);
            if (count > 0)
            {
                _fileStream.Write(byteBuffer.Slice(0, count));
            }
        }

        // only for demo
        private async ValueTask WriteCharBufferToFileStreamAsync()
        {
            Memory<byte> byteBuffer;
            if (_byteBuffer is not null)
            {
                byteBuffer = _byteBuffer;
            }
            else
            {
                byteBuffer = _byteBuffer = new byte[_encoding.GetMaxByteCount(_charBuffer.Length)];
            }

            int count = _encoding.GetBytes(new ReadOnlySpan<char>(_charBuffer, 0, _charBufferCount), byteBuffer.Span);

            if (count > 0)
            {
                await _fileStream.WriteAsync(byteBuffer.Slice(0, count));
            }
        }

        public void Dispose()
        {
            WriteCharBufferToFileStream();
            _fileStream.Flush();
            _fileStream.Dispose();
        }
    }
}
