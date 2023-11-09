using System;
using System.IO;

namespace LogReader
{
    public class LogReader : IDisposable
    {
        private Stream owningStream;
        private StreamReader streamReader;
        private bool disposedValue;

        public LogReader(Stream stream)
        {
            streamReader = new StreamReader(stream);
        }

        public LogReader(string path) : this(File.OpenRead(path))
        {
            owningStream = streamReader.BaseStream;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    streamReader = null;
                }

                // 自分で確保した場合のみ破棄する
                if (owningStream != null)
                {
                    owningStream.Dispose();
                    owningStream = null;
                }
                disposedValue = true;
            }
        }

        ~LogReader()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
