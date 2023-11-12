using System;
using System.Collections.Generic;
using System.IO;

namespace MifuminLib.AccessAnalyzer
{
    public class LogReader : IDisposable
    {
        private Stream owningStream;
        private StreamReader streamReader;
        private bool disposedValue;

        public readonly struct ErrorLine
        {
            public readonly LogParser.State State;
            public readonly string Line;

            public ErrorLine(LogParser.State state, string line)
            {
                State = state;
                Line = line;
            }
        }

        public LogReader(Stream stream)
        {
            streamReader = new StreamReader(stream);
        }

        public LogReader(string path) : this(File.OpenRead(path))
        {
            owningStream = streamReader.BaseStream;
        }

        public IEnumerable<Log> EnumerateAsCombined(Action<ErrorLine> onError = null)
        {
            var reader = streamReader;
            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                var result = LogParser.ParseAsCombined(line);
                if (result.Log != null)
                {
                    yield return result.Log;
                }
                else
                {
                    onError?.Invoke(new ErrorLine(result.State, line));
                }
            }
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
