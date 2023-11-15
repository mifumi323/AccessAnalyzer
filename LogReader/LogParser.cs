using System;
using System.Text.RegularExpressions;

namespace MifuminLib.AccessAnalyzer
{
    /// <summary>ログ一行のパースを行います。</summary>
    public static class LogParser
    {
        /// <summary>ログをパースする際に発生したエラーの組み合わせを表します。</summary>
        [Flags]
        public enum ErrorFlag
        {
            /// <summary>エラーなし</summary>
            None = 0,
            /// <summary>構造のエラー</summary>
            Structure = 1 << 0,
            /// <summary>ホスト名orIP</summary>
            Host = 1 << 1,
            /// <summary>リモートログ名</summary>
            RemoteLog = 1 << 2,
            /// <summary>ユーザー名</summary>
            User = 1 << 3,
            /// <summary>日付</summary>
            Date = 1 << 4,
            /// <summary>メソッド</summary>
            Method = 1 << 5,
            /// <summary>リクエスト先</summary>
            Requested = 1 << 6,
            /// <summary>HTTPのバージョン</summary>
            HTTP = 1 << 7,
            /// <summary>ステータスコード</summary>
            Status = 1 << 8,
            /// <summary>転送量</summary>
            SendSize = 1 << 9,
            /// <summary>参照元</summary>
            Referer = 1 << 10,
            /// <summary>ユーザーエージェント</summary>
            UserAgent = 1 << 11,
        }

        /// <summary>パース結果を表します。</summary>
        public readonly struct Result
        {
            /// <summary>エラーフラグの組み合わせ。エラーなく成功した場合は None。</summary>
            public readonly ErrorFlag ErrorFlag;
            /// <summary>ログオブジェクト。パース失敗した場合は null。</summary>
            public readonly Log Log;

            public Result(ErrorFlag errorFlag, Log log)
            {
                ErrorFlag = errorFlag;
                Log = log;
            }
        }

        /// <summary>
        /// Combined 形式としてログ一行のパースを行います。
        /// </summary>
        /// <param name="line">ログ一行(改行コードを含まず)</param>
        /// <returns>成功したら、ErrorFlag.None と Log オブジェクトを含んだ Result。失敗したら、対応する ErrorFlag と nullを含んだResult。</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Result ParseAsCombined(string line)
        {
            // LogFormat "%h %l %u %t \"%r\" %>s %b \"%{Referer}i\" \"%{User-Agent}i\"" combined
            // %s %s %s [%02d/%3s/%04d:%02d:%02d:%02d %1s%04d] \"%s %s %s\" %3d %d \"%s\" \"%s\"
            var m = Regex.Match(line, @"^(?<Host>[^ ]+) (?<RemoteLog>[^ ]+) (?<User>[^ ]+) \[(?<Date>[^]]+)\] ""(?<Method>[^ ]+) (?<Requested>[^ ]+) (?<HTTP>[^""]+)"" (?<Status>[^ ]+) (?<SendSize>[^ ]+) ""(?<Referer>[^""]+)"" ""(?<UserAgent>[^""]+)""$");
            if (!m.Success)
            {
                return new Result(ErrorFlag.Structure, null);
            }

            var errorFlag = ErrorFlag.None;

            var host = m.Groups["Host"].Value;
            var remoteLog = m.Groups["RemoteLog"].Value;
            var user = m.Groups["User"].Value;
            var date = m.Groups["Date"].Value;
            if (!TryParseDateTime(date, out var dateTime))
            {
                errorFlag |= ErrorFlag.Date;
            }
            var method = m.Groups["Method"].Value;
            if (!TryParseMethod(method, out var eMethod))
            {
                errorFlag |= ErrorFlag.Method;
            }
            var requested = m.Groups["Requested"].Value;
            var http = m.Groups["HTTP"].Value;
            if (!TryParseHttp(http, out var eHttp))
            {
                errorFlag |= ErrorFlag.HTTP;
            }
            var status = m.Groups["Status"].Value;
            if (!short.TryParse(status, out var statusValue))
            {
                errorFlag |= ErrorFlag.Status;
            }
            var sendSize = m.Groups["SendSize"].Value;
            if (!int.TryParse(sendSize, out var sendSizeValue))
            {
                errorFlag |= ErrorFlag.SendSize;
            }
            var referer = m.Groups["Referer"].Value;
            var userAgent = m.Groups["UserAgent"].Value;

            var log = errorFlag != ErrorFlag.None ? null : new Log()
            {
                Host = host,
                RemoteLog = remoteLog,
                User = user,
                lDate = dateTime.Ticks,
                Method = eMethod,
                Requested = requested,
                eHTTP = eHttp,
                Status = statusValue,
                SendSize = sendSizeValue,
                Referer = referer,
                UserAgent = userAgent,
            };
            return new Result(errorFlag, log);
        }

        private static bool TryParseDateTime(string date, out DateTime dateTime)
        {
            var m = Regex.Match(date, @"^(?<Day>\d{2})/(?<Month>\w{3})/(?<Year>\d{4}):(?<Hour>\d{2}):(?<Minute>\d{2}):(?<Second>\d{2}) (?<OffsetHour>[-+]\d{2})(?<OffsetMinute>\d{2})$");
            if (!m.Success)
            {
                dateTime = default;
                return false;
            }

            var year = int.Parse(m.Groups["Year"].Value);
            if (year <= 0)
            {
                dateTime = default;
                return false;
            }
            var month = ParseMonth(m.Groups["Month"].Value);
            if (month <= 0)
            {
                dateTime = default;
                return false;
            }
            var day = int.Parse(m.Groups["Day"].Value);
            if (day <= 0 || DateTime.DaysInMonth(year, month) < day)
            {
                dateTime = default;
                return false;
            }
            var hour = int.Parse(m.Groups["Hour"].Value);
            if (hour < 0 || 24 <= hour)
            {
                dateTime = default;
                return false;
            }
            var minute = int.Parse(m.Groups["Minute"].Value);
            if (minute < 0 || 60 <= minute)
            {
                dateTime = default;
                return false;
            }
            var second = int.Parse(m.Groups["Second"].Value);
            if (second < 0 || 60 <= second)
            {
                dateTime = default;
                return false;
            }
            dateTime = new DateTime(year, month, day, hour, minute, second);
            // タイムゾーンはひとまず無視しよう。
            return true;
        }

        private static int ParseMonth(string value)
        {
            switch (value)
            {
                case "Jan": return 1;
                case "Feb": return 2;
                case "Mar": return 3;
                case "Apr": return 4;
                case "May": return 5;
                case "Jun": return 6;
                case "Jul": return 7;
                case "Aug": return 8;
                case "Sep": return 9;
                case "Oct": return 10;
                case "Nov": return 11;
                case "Dec": return 12;
                default: return 0;
            }
        }

        private static bool TryParseMethod(string method, out Log.EMethod eMethod)
        {
            switch (method)
            {
                case "GET":
                    eMethod = Log.EMethod.GET;
                    return true;
                case "HEAD":
                    eMethod = Log.EMethod.HEAD;
                    return true;
                case "POST":
                    eMethod = Log.EMethod.POST;
                    return true;
                case "PUT":
                    eMethod = Log.EMethod.PUT;
                    return true;
                case "DELETE":
                    eMethod = Log.EMethod.DELETE;
                    return true;
                case "CONNECT":
                    eMethod = Log.EMethod.CONNECT;
                    return true;
                case "OPTIONS":
                    eMethod = Log.EMethod.OPTIONS;
                    return true;
                case "TRACE":
                    eMethod = Log.EMethod.TRACE;
                    return true;
                case "LINK":
                    eMethod = Log.EMethod.LINK;
                    return true;
                case "UNLINK":
                    eMethod = Log.EMethod.UNLINK;
                    return true;
                default:
                    eMethod = default;
                    return false;
            }
        }

        private static bool TryParseHttp(string http, out Log.EHTTP eHttp)
        {
            switch (http)
            {
                case "HTTP/1.0":
                    eHttp = Log.EHTTP.HTTP10;
                    return true;
                case "HTTP/1.1":
                    eHttp = Log.EHTTP.HTTP11;
                    return true;
                case "HTTP/2.0":
                    eHttp = Log.EHTTP.HTTP20;
                    return true;
                default:
                    eHttp = default;
                    return false;
            }
        }
    }
}
