using System;

namespace MifuminLib.AccessAnalyzer
{
    public static class LogParser
    {
        public enum State
        {
            Success,
        }

        public readonly struct Result
        {
            public readonly State State;
            public readonly Log Log;
        }

        public static Result ParseAsCombined(string line)
        {
            // LogFormat "%h %l %u %t \"%r\" %>s %b \"%{Referer}i\" \"%{User-Agent}i\"" combined
            // %s %s %s [%02d/%3s/%04d:%02d:%02d:%02d %1s%04d] \"%s %s %s\" %3d %d \"%s\" \"%s\"
            throw new NotImplementedException();
        }
    }
}
