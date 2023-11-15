using System;

namespace MifuminLib.AccessAnalyzer
{
    /// <summary>
    /// ログの一行に相当する
    /// オリジナルのテキストは保持されないので注意
    /// </summary>
    public sealed class Log
    {
        public enum EMethod : byte { UNKNOWN, GET, HEAD, POST, PUT, DELETE, CONNECT, OPTIONS, TRACE, LINK, UNLINK }
        public enum EHTTP : byte { HTTP10, HTTP11, HTTP20 }

        /// <summary>ホスト名orIP</summary>
        public string Host { get; set; }
        /// <summary>リモートログ名</summary>
        public string RemoteLog { get; set; }
        /// <summary>ユーザー名</summary>
        public string User { get; set; }
        /// <summary>日付(DateTimeとしないのは検索の高速化のため)</summary>
        public long lDate;
        public string Date => new DateTime(lDate).ToString("yyyy/MM/dd HH:mm:ss");
        /// <summary>メソッド</summary>
        public EMethod Method { get; set; }
        /// <summary>リクエスト先</summary>
        public string Requested { get; set; }
        /// <summary>HTTPのバージョン</summary>
        public EHTTP eHTTP;
        public string HTTP
        {
            get {
                switch (eHTTP)
                {
                    case EHTTP.HTTP10:
                        return "HTTP 1.0";
                    case EHTTP.HTTP11:
                        return "HTTP 1.1";
                    case EHTTP.HTTP20:
                        return "HTTP 2.0";
                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                switch (value)
                {
                    case "HTTP 1.0":
                        eHTTP = EHTTP.HTTP10;
                        break;
                    case "HTTP 1.1":
                        eHTTP = EHTTP.HTTP11;
                        break;
                    case "HTTP 2.0":
                        eHTTP = EHTTP.HTTP20;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                eHTTP = (value == "HTTP 1.0") ? EHTTP.HTTP10 : EHTTP.HTTP11;
            }
        }
        /// <summary>ステータスコード</summary>
        public short Status { get; set; }
        /// <summary>転送量</summary>
        public int SendSize { get; set; }
        /// <summary>参照元</summary>
        public string Referer { get; set; }
        /// <summary>ユーザーエージェント</summary>
        public string UserAgent { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Log log
                && log.Host == Host
                && log.RemoteLog == RemoteLog
                && log.User == User
                && log.lDate == lDate
                && log.Method == Method
                && log.Requested == Requested
                && log.eHTTP == eHTTP
                && log.Status == Status
                && log.SendSize == SendSize
                && log.Referer == Referer
                && log.UserAgent == UserAgent;
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode()
                ^ RemoteLog.GetHashCode()
                ^ User.GetHashCode()
                ^ lDate.GetHashCode()
                ^ Method.GetHashCode()
                ^ Requested.GetHashCode()
                ^ eHTTP.GetHashCode()
                ^ Status.GetHashCode()
                ^ SendSize.GetHashCode()
                ^ Referer.GetHashCode()
                ^ UserAgent.GetHashCode();
        }
    }
}
