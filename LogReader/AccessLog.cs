﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MifuminLib.AccessAnalyzer
{
    /// <summary>一ファイル内の全てのログを格納する</summary>
    public class LogFile
    {
        public string FileName;
        public Log[] LogList = new Log[0];
        private volatile bool canceled = false;

        /// <summary>特定のファイルを開く</summary>
        /// <param name="filename">ファイル名</param>
        /// <param name="option">読み込みオプション</param>
        /// <returns>最後まで読み込んだかどうか</returns>
        public bool Read(string filename, LogReadOption option)
        {
            canceled = false;
            FileName = filename;
            var list = new LinkedList<Log>();
            var buffer = new byte[GetBufferSize(option)];
            if (File.Exists(filename))
            {
                var binReader = new BinaryReader(
                    new BufferedStream(File.Open(filename, FileMode.Open), 1024 * 1024)
                    );
                try
                {
                    // LogFormat "%h %l %u %t \"%r\" %>s %b \"%{Referer}i\" \"%{User-Agent}i\"" combined
                    // %s %s %s [%02d/%3s/%04d:%02d:%02d:%02d %1s%04d] \"%s %s %s\" %3d %d \"%s\" \"%s\"
                    while (true)
                    {
                        if (canceled)
                        {
                            break;
                        }

                        var l = ReadLine(binReader, option, buffer);
                        if (l != null && option.filter.Match(l))
                        {
                            list.AddLast(l);
                        }
                    }
                }
                catch (EndOfStreamException) { }
                finally { binReader.Close(); }
                LogList = new Log[list.Count];
                var num = 0;
                foreach (var l in list)
                {
                    LogList[num++] = l;
                }
            }
            return !canceled;
        }

        private Log ReadLine(BinaryReader binReader, LogReadOption option, byte[] buffer)
        {
            int size, buffersize;
            int day, month, year, hour, minute, second;
            byte buf, old;

            var l = new Log();

            // ホスト/IP
            size = 0;
            buffersize = option.hostBuffer;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == ' ') { l.Host = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }
            }

            // リモートログ名
            size = 0;
            buffersize = option.remoteLogBuffer;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == ' ') { l.RemoteLog = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }
            }

            // ユーザー名
            size = 0;
            buffersize = option.userBuffer;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == ' ') { l.User = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }
            }

            // 時刻(手抜きでエラーチェックしてない)
            binReader.ReadByte();   // '['
            day = (binReader.ReadByte() - '0') * 10 + (binReader.ReadByte() - '0'); // 日
            binReader.ReadByte();   // '/'

            // 月(Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec)
            buffer[0] = binReader.ReadByte();
            buffer[1] = binReader.ReadByte();
            buffer[2] = binReader.ReadByte();
            if (buffer[0] == 'D') { month = 12; }        // Dec
            else if (buffer[0] == 'F') { month = 2; }    // Feb
            else if (buffer[0] == 'N') { month = 11; }   // Nov
            else if (buffer[0] == 'O') { month = 10; }   // Oct
            else if (buffer[0] == 'S') { month = 9; }    // Sep
            else if (buffer[1] == 'p') { month = 4; }    // Apr
            else if (buffer[2] == 'g') { month = 8; }    // Aug
            else if (buffer[2] == 'l') { month = 7; }    // Jul
            else if (buffer[2] == 'r') { month = 3; }    // Mar
            else if (buffer[2] == 'y') { month = 5; }    // May
            else if (buffer[1] == 'a') { month = 1; }    // Jan
            else if (buffer[1] == 'u') { month = 6; }    // Jun
            else { month = 0; }
            binReader.ReadByte();   // '/'
            year = (binReader.ReadByte() - '0') * 1000 + (binReader.ReadByte() - '0') * 100
                + (binReader.ReadByte() - '0') * 10 + (binReader.ReadByte() - '0'); // 年
            binReader.ReadByte();   // ':'
            hour = (binReader.ReadByte() - '0') * 10 + (binReader.ReadByte() - '0'); // 時
            binReader.ReadByte();   // ':'
            minute = (binReader.ReadByte() - '0') * 10 + (binReader.ReadByte() - '0'); // 分
            binReader.ReadByte();   // ':'
            second = (binReader.ReadByte() - '0') * 10 + (binReader.ReadByte() - '0'); // 秒
            binReader.ReadBytes(9);   // ' +0900] "'
            try { l.lDate = (new DateTime(year, month, day, hour, minute, second)).Ticks; } catch (ArgumentOutOfRangeException) { l.lDate = 0; }

            // メソッド
            buf = binReader.ReadByte();
            if (buf == 'G') { l.Method = Log.EMethod.GET; }
            else if (buf == 'P')
            {
                buf = binReader.ReadByte();
                if (buf == 'O') { l.Method = Log.EMethod.POST; } else if (buf == 'U') { l.Method = Log.EMethod.PUT; } else { l.Method = Log.EMethod.UNKNOWN; }
            }
            else if (buf == 'H') { l.Method = Log.EMethod.HEAD; } else if (buf == 'C') { l.Method = Log.EMethod.CONNECT; } else if (buf == 'D') { l.Method = Log.EMethod.DELETE; } else if (buf == 'L') { l.Method = Log.EMethod.LINK; } else if (buf == 'O') { l.Method = Log.EMethod.OPTIONS; } else if (buf == 'T') { l.Method = Log.EMethod.TRACE; } else if (buf == 'U') { l.Method = Log.EMethod.UNLINK; } else { l.Method = Log.EMethod.UNKNOWN; }
            while (binReader.ReadByte() != ' ') { }

            // リクエスト先
            size = 0;
            buffersize = option.requestedBuffer;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == ' ') { l.Requested = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }
            }

            // HTTP(やっぱり手抜きでエラーチェックなし)
            binReader.ReadBytes(5);   // 'HTTP/'
            var httpL = binReader.ReadByte();
            binReader.ReadByte();   // '.'
            var httpR = binReader.ReadByte();
            l.eHTTP = httpL == '2' ? Log.EHTTP.HTTP20 : (httpR == '0' ? Log.EHTTP.HTTP10 : Log.EHTTP.HTTP11);
            binReader.ReadBytes(2);   // '" '

            // ステータスコード
            while (true)
            {
                buf = binReader.ReadByte();
                if ('0' <= buf && buf <= '9') { l.Status = (short)(l.Status * 10 + buf - '0'); } else if (buf == ' ') { break; } else if (buf == '\n' || buf == '\r') { return null; }
            }

            // 転送量
            while (true)
            {
                buf = binReader.ReadByte();
                if ('0' <= buf && buf <= '9') { l.SendSize = l.SendSize * 10 + buf - '0'; }
                else if (buf == ' ') { break; }
                else if (buf == '\n' || buf == '\r')
                {
                    // ここでの失敗のみ特別な処理をする
                    // (combinedとしては失敗だがcommonとしては成功)
                    l.Referer = "";
                    l.UserAgent = "";
                    return l;
                }
            }

            // リファラ
            binReader.ReadByte();   // '"'
            size = 0;
            buffersize = option.refererBuffer;
            old = 0;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == '"' && old != '\\') { l.Referer = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }

                old = buf;
            }
            binReader.ReadByte();   // ' '

            // ユーザーエージェント
            binReader.ReadByte();   // '"'
            size = 0;
            buffersize = option.userAgentBuffer;
            old = 0;
            while (true)
            {
                buf = binReader.ReadByte();
                if (buf == '"' && old != '\\') { l.UserAgent = GetString(buffer, 0, size); break; } else if (buf == '\n' || buf == '\r') { return null; }
                if (size < buffersize)
                {
                    buffer[size++] = buf;
                }

                old = buf;
            }
            binReader.ReadByte();   // '\n'

            return l;
        }

        private string GetString(byte[] array, int offset, int size)
        {
            var sb = new StringBuilder();
            for (var i = offset; i < offset + size; i++)
            {
                var b = array[i];
                if (0x20 <= b && b < 0x7f)
                {
                    sb.Append((char)b);
                }
                else
                {
                    sb.AppendFormat("%{0:x}", b);
                }
            }
            return sb.ToString();
        }

        /// <summary>すでに開いたファイルを設定を変えて読み直す</summary>
        /// <param name="option">読み込みオプション</param>
        /// <returns>最後まで読み込んだかどうか</returns>
        public bool Reload(LogReadOption option) { return Read(FileName, option); }

        public void Cancel() { canceled = true; }

        private int GetBufferSize(LogReadOption option)
        {
            var size = 3;   // 最低でも3バイトのバッファを使用する
            if (size < option.hostBuffer)
            {
                size = option.hostBuffer;
            }

            if (size < option.remoteLogBuffer)
            {
                size = option.remoteLogBuffer;
            }

            if (size < option.userBuffer)
            {
                size = option.userBuffer;
            }

            if (size < option.requestedBuffer)
            {
                size = option.requestedBuffer;
            }

            if (size < option.refererBuffer)
            {
                size = option.refererBuffer;
            }

            if (size < option.userAgentBuffer)
            {
                size = option.userAgentBuffer;
            }

            return size;
        }
    }

    public class LogReadOption
    {
        public int hostBuffer = 255;        // 有効なホスト名は255文字以下
        public int remoteLogBuffer = 256;   // わからんけど念のため256用意しとく
        public int userBuffer = 256;        // わからんけど念のため256用意しとく
        public int requestedBuffer = 1024;  // 1024も読み込めれば充分でしょう
        public int refererBuffer = 1024;    // 1024も読み込めれば充分でしょう
        public int userAgentBuffer = 1024;  // 1024も読み込めれば充分でしょう
        public LogFilter filter = new LogFilterAll();
    }

    public delegate void UpdateFunc(Log[] logs);

    /// <summary>ログ統括クラス</summary>
    public class AccessLog
    {
        private LogFile[] AllLog = new LogFile[0];  // ファイルごとのログの内容
        private Log[] Target = new Log[0];          // 基本的に表示や詳しい解析などはこのTargetに対して行う
        private LogFilter analyzeFilter = new LogFilterAll();
        private UpdateFunc UpdateFuncs;
#if DOT_NET_FRAMEWORK
        private readonly System.Windows.Forms.Form owner;
#endif
        private LogFile NowLoading = null;          // 読み込み中のログ
        public LogReadOption ReadOption { get; } = new LogReadOption();

        public AccessLog() { }

#if DOT_NET_FRAMEWORK
        public AccessLog(System.Windows.Forms.Form owner)
        {
            this.owner = owner;
        }
#endif

        /// <summary>ファイルを読み込んでログに追加する</summary>
        /// <param name="filenames">ファイル名の配列</param>
        public void Read(string[] filenames)
        {
            var nOldLength = AllLog.Length;
            var continueload = true;
            Array.Resize(ref AllLog, nOldLength + filenames.Length);
            for (var i = 0; i < filenames.Length; i++)
            {
                NowLoading = AllLog[nOldLength + i] = new LogFile();
                if (continueload)
                {
                    continueload = NowLoading.Read(filenames[i], ReadOption);
                }
                else
                {
                    NowLoading.FileName = filenames[i];
                }
            }
            lock (this) { NowLoading = null; }
            Update();
        }
        public void Release(LogFile file)
        {
            var newLog = new LogFile[AllLog.Length - 1];
            var i = 0;
            foreach (var lf in AllLog)
            {
                if (lf != file)
                {
                    newLog[i++] = lf;
                }
            }
            if (i == newLog.Length)
            {
                AllLog = newLog;
                Update();
            }
        }
        public void Clear()
        {
            AllLog = new LogFile[0];
            Update();
        }
        public LogFilter ReadFilter
        {
            set => ReadOption.filter = value;
            get => ReadOption.filter;
        }
        public LogFilter AnalyzeFilter
        {
            set { analyzeFilter = value; Target = GetLogs(); Update(); }
            get => analyzeFilter;
        }
        public void Update()
        {
            Target = GetLogs();
            if (UpdateFuncs != null)
            {
#if DOT_NET_FRAMEWORK
                if (owner != null)
                {
                    owner.Invoke(UpdateFuncs, new object[] { Target });
                }
                else
                {
                    UpdateFuncs(Target);
                }
#else
#pragma warning disable IDE1005 // デリゲート呼び出しを簡略化できます。
                UpdateFuncs(Target);
#pragma warning restore IDE1005 // デリゲート呼び出しを簡略化できます。
#endif
            }
        }
        public void AddUpdateFunc(UpdateFunc update) { UpdateFuncs += update; update(Target); }
        public void RemoveUpdateFunc(UpdateFunc update) { UpdateFuncs -= update; }
        public Log[] GetLogs()
        {
            var list = new LinkedList<Log>();
            foreach (var file in AllLog)
            {
                foreach (var log in file.LogList)
                {
                    if (analyzeFilter.Match(log))
                    {
                        list.AddLast(log);
                    }
                }
            }

            var ret = new Log[list.Count];
            var num = 0;
            foreach (var log in list)
            {
                ret[num++] = log;
            }

            return ret;
        }
        public Log[] GetTarget() { return Target; }
        public LogFile[] GetFile() { return AllLog; }
        public void Reload(LogFile file)
        {
            (NowLoading = file).Reload(ReadOption);
            lock (this) { NowLoading = null; }
            Update();
        }
        public void ReloadAll()
        {
            foreach (var file in AllLog)
            {
                if (!(NowLoading = file).Reload(ReadOption))
                {
                    break;
                }
            }
            lock (this) { NowLoading = null; }
            Update();
        }

        public void CancelRead()
        {
            lock (this)
            {
                if (NowLoading != null)
                {
                    NowLoading.Cancel();
                }
            }
        }
    }
}
