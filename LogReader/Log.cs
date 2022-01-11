using System;

namespace MifuminLib.AccessAnalyzer
{
    /// <summary>
    /// ログの一行に相当する
    /// オリジナルのテキストは保持されないので注意
    /// </summary>
    public class Log
    {
        public enum EMethod : byte { UNKNOWN, GET, HEAD, POST, PUT, DELETE, CONNECT, OPTIONS, TRACE, LINK, UNLINK }
        public enum EHTTP : byte { HTTP10, HTTP11 }

        /// <summary>ホスト名orIP</summary>
        public string Host { get; set; }
        /// <summary>リモートログ名</summary>
        public string RemoteLog { get; set; }
        /// <summary>ユーザー名</summary>
        public string User { get; set; }
        /// <summary>日付(DateTimeとしないのは検索の高速化のため)</summary>
        public long lDate;
        public string Date => (new DateTime(lDate)).ToString("yyyy/MM/dd HH:mm:ss");
        /// <summary>メソッド</summary>
        public EMethod Method { get; set; }
        /// <summary>リクエスト先</summary>
        public string Requested { get; set; }
        /// <summary>HTTPのバージョン</summary>
        public EHTTP eHTTP;
        public string HTTP { get => (eHTTP == EHTTP.HTTP10) ? "HTTP 1.0" : "HTTP 1.1"; set => eHTTP = (value == "HTTP 1.0") ? EHTTP.HTTP10 : EHTTP.HTTP11; }
        /// <summary>ステータスコード</summary>
        public short Status { get; set; }
        /// <summary>転送量</summary>
        public int SendSize { get; set; }
        /// <summary>参照元</summary>
        public string Referer { get; set; }
        /// <summary>ユーザーエージェント</summary>
        public string UserAgent { get; set; }
    }
}
