using System.Text;

namespace TextTool
{

    public class FileReader : IDisposable
    {
        StreamReader reader;
        char[] buffer;
        int index;
        int bytesRead;

        public FileReader(string path, long bufferSize = 4096)
        {
            reader = new StreamReader(path);
            buffer = new char[bufferSize];
        }

        public async ValueTask<int> ReadAsync()
        {
            if (index < bytesRead)
                return buffer[index++];

            if (reader.EndOfStream)
                return -1;

            bytesRead = await reader.ReadBlockAsync(buffer);
            index = 0;

            if (bytesRead == 0)
                return -1;

            return buffer[index++];

        }

        public Encoding CurrentEncoding => reader.CurrentEncoding;

        public bool EndOfStream => index == bytesRead && reader.EndOfStream;

        public void Dispose()
        {
            reader.Dispose();
        }

        public async ValueTask<int> PeekAsync()
        {
            if (index < bytesRead)
                return buffer[index];

            if (reader.EndOfStream)
                return -1;

            bytesRead = await reader.ReadBlockAsync(buffer);
            index = 0;

            if (bytesRead == 0)
                return -1;

            return buffer[index];
        }
    }
}
