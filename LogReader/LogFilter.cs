using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MifuminLib.AccessAnalyzer
{
    /// <summary>LogFilter派生クラスの型</summary>
    public enum ELogFilter
    {
        /// <summary>無条件フィルタ(全部通す)</summary>
        All,
        /// <summary>不明なフィルタ</summary>
        Unknown,

        #region 個別フィルタ
        /// <summary>ホスト名/IP</summary>
        Host,
        /// <summary>リモートログ名</summary>
        RemoteLog,
        /// <summary>ユーザー名</summary>
        User,
        /// <summary>日付</summary>
        Date,
        /// <summary>月のみ</summary>
        Month,
        /// <summary>日のみ</summary>
        Day,
        /// <summary>曜日</summary>
        Weekday,
        /// <summary>時間</summary>
        Hour,
        /// <summary>メソッド</summary>
        Method,
        /// <summary>リクエスト先</summary>
        Requested,
        /// <summary>HTTPのバージョン</summary>
        HTTP,
        /// <summary>ステータスコード</summary>
        StatusCode,
        /// <summary>転送量</summary>
        SendSize,
        /// <summary>リファラ</summary>
        Referer,
        /// <summary>リファラのドメイン</summary>
        Domain,
        /// <summary>検索に使われた語句</summary>
        SearchPhrase,
        /// <summary>ユーザーエージェント</summary>
        UserAgent,
        #endregion

        #region 集合フィルタ
        /// <summary>すべてのサブフィルタにマッチ</summary>
        And,
        /// <summary>少なくともひとつのサブフィルタにマッチ</summary>
        Or,
        /// <summary>全てのサブフィルタにはマッチしない</summary>
        Nand,
        /// <summary>どのサブフィルタにもマッチしない</summary>
        Nor,
        #endregion
    }

    /// <summary>操作対象にするログの抽出をする</summary>
    public abstract class LogFilter
    {
        /// <summary>ログが条件にマッチするか調べる</summary>
        /// <param name="l">対象となるログ</param>
        public abstract bool Match(Log l);
        /// <summary>派生クラスの型を調べる</summary>
        public abstract ELogFilter FilterType();
        public override string ToString() { return "不明なフィルタ"; }

        public void Save(string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName))
            {
                Save(writer);
            }
        }
        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("filter");
            {
                writer.WriteAttributeString("type", FilterType().ToString());
                SaveImpl(writer);
            }
            writer.WriteEndElement();
        }
        protected abstract void SaveImpl(XmlWriter writer);

        protected bool EndFilter(XmlReader reader)
        { return reader.NodeType == XmlNodeType.EndElement && reader.LocalName.ToLower() == "filter"; }
        protected abstract void LoadImpl(XmlReader reader);
        public static LogFilter Create(ELogFilter type)
        {
            switch (type)
            {
                case ELogFilter.All: return new LogFilterAll();
                case ELogFilter.Host: return new LogFilterHost();
                case ELogFilter.RemoteLog: return new LogFilterRemoteLog();
                case ELogFilter.User: return new LogFilterUser();
                case ELogFilter.Date: return new LogFilterDate();
                case ELogFilter.Month: return new LogFilterMonth();
                case ELogFilter.Day: return new LogFilterDay();
                case ELogFilter.Weekday: return new LogFilterWeekday();
                case ELogFilter.Hour: return new LogFilterHour();
                case ELogFilter.Method: return new LogFilterMethod();
                case ELogFilter.Requested: return new LogFilterRequested();
                case ELogFilter.HTTP: return new LogFilterHTTP();
                case ELogFilter.StatusCode: return new LogFilterStatusCode();
                case ELogFilter.SendSize: return new LogFilterSendSize();
                case ELogFilter.Referer: return new LogFilterReferer();
                case ELogFilter.Domain: return new LogFilterDomain();
                case ELogFilter.SearchPhrase: return new LogFilterSearchPhrase();
                case ELogFilter.UserAgent: return new LogFilterUserAgent();
                case ELogFilter.And: return new LogFilterAnd();
                case ELogFilter.Or: return new LogFilterOr();
                case ELogFilter.Nand: return new LogFilterNand();
                case ELogFilter.Nor: return new LogFilterNor();
                default: return new LogFilterUnknown();
            }
        }
        protected static LogFilter Create(XmlReader reader)
        {
            LogFilter filter;
            try
            {
                filter = Create((ELogFilter)Enum.Parse(typeof(ELogFilter), reader.GetAttribute("type"), true));
            }
            catch (Exception)
            {
                filter = new LogFilterUnknown();
            }
            filter.LoadImpl(reader);
            return filter;
        }
        public static LogFilter Load(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "filter")
                    return Create(reader);
            }
            return null;
        }
        public static LogFilter Load(string fileName)
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                LogFilter filter = Load(reader);
                return filter != null ? filter : new LogFilterAll();
            }
        }

        /// <summary>フィルタのXML文字列表現を返す。</summary>
        /// <returns>XML</returns>
        public string ToXmlString()
        {
            var output = new StringBuilder(1024);
            using (var writer = XmlWriter.Create(output))
            {
                Save(writer);
            }
            return output.ToString();
        }
        /// <summary>XML文字列からフィルタを作って返す。</summary>
        /// <param name="xml">XML</param>
        /// <returns>フィルタ</returns>
        public static LogFilter FromXmlString(string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return Load(reader);
            }
        }

        /// <summary>このフィルタにマッチするログを返す。</summary>
        /// <param name="logs">不純物だらけのログ</param>
        /// <returns>フィルタに通してきれいになったログ</returns>
        public virtual Log[] Extract(Log[] logs)
        {
            LinkedList<Log> list = new LinkedList<Log>();
            foreach (Log log in logs)
            {
                if (Match(log)) list.AddLast(log);
            }
            Log[] ret = new Log[list.Count];
            list.CopyTo(ret, 0);
            return ret;
        }

        public LogFilter Clone()
        {
            StringBuilder buffer = new StringBuilder(4096);
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                Save(writer);
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(buffer.ToString())))
            {
                return Load(reader);
            }
        }
    }

    #region 補助フィルタ

    /// <summary>数値を対象としたフィルタ</summary>
    public abstract class LogFilterNumber : LogFilter
    {
        public long Min = long.MinValue, Max = long.MaxValue;

        public LogFilterNumber() { }
        public LogFilterNumber(long min, long max)
        {
            Min = min; Max = max;
        }

        protected bool Match(long value)
        {
            return Min <= Max ?
                (Min <= value && value <= Max) :
                (value <= Max || Min <= value);
        }

        protected bool Match(int value) { return Match((long)value); }
        protected bool Match(short value) { return Match((long)value); }
        protected bool Match(sbyte value) { return Match((long)value); }
        protected bool Match(uint value) { return Match((long)value); }
        protected bool Match(ushort value) { return Match((long)value); }
        protected bool Match(byte value) { return Match((long)value); }

        public virtual long LowerLimit() { return 0; }
        public virtual long UpperLimit() { return int.MaxValue; }

        protected override void SaveImpl(XmlWriter writer)
        {
            if (Min != long.MinValue) writer.WriteElementString("min", Min.ToString());
            if (Max != long.MaxValue) writer.WriteElementString("max", Max.ToString());
        }

        protected override void LoadImpl(XmlReader reader)
        {
            Min = long.MinValue; Max = long.MaxValue;
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (EndFilter(reader)) break;
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "min")
                        Min = long.Parse(reader.ReadString());
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "max")
                        Max = long.Parse(reader.ReadString());
                }
            }
        }

        protected string ToString(string name)
        {
            if (Min == Max) return string.Format("{0}が{1}に等しい", name, ToString(Min));
            if (Min > Max) return string.Format("{0}が{1}以下、または{2}以上の範囲", name, ToString(Max), ToString(Min));
            if (Min == long.MinValue)
            {
                if (Max == long.MaxValue) return string.Format("{0}に関する範囲フィルタ(無条件)", name);
                return string.Format("{0}が{1}以下", name, ToString(Max));
            }
            if (Max == long.MaxValue) return string.Format("{0}が{1}以上", name, ToString(Min));
            return string.Format("{0}が{1}以上、{2}以下の範囲", name, ToString(Min), ToString(Max));
        }
        protected virtual string ToString(long value) { return value.ToString(); }
    }

    /// <summary>文字列を対象としたフィルタ</summary>
    public abstract class LogFilterString : LogFilterNumber
    {
        /// <summary>マッチ条件</summary>
        public enum MatchRule
        {
            /// <summary>文字列の長さ</summary>
            Length,
            /// <summary>どれかひとつに完全一致</summary>
            Match,
            /// <summary>どれにも完全一致しない</summary>
            UnmatchAll,
            /// <summary>どれかひとつを含む</summary>
            Contain,
            /// <summary>どれも含まない</summary>
            UncontainAll,
            /// <summary>すべてを含む</summary>
            ContainAll,
            /// <summary>すべては含まない</summary>
            Uncontain,
            /// <summary>どれかひとつから始まる</summary>
            Start,
            /// <summary>どれからも始まらない</summary>
            NotStartAll,
            /// <summary>どれかひとつで終わる</summary>
            End,
            /// <summary>どれでも終わらない</summary>
            NotEndAll,
        }
        private string[] matches = new string[0];
        public string Matches
        {
            get
            {
                return string.Join("\r\n", matches);
            }
            set
            {
                matches = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public bool IgnoreCase = false;
        public MatchRule matchRule;

        public LogFilterString() { }
        public LogFilterString(string matches, bool ignoreCase, MatchRule matchRule)
        {
            Matches = matches;
            IgnoreCase = ignoreCase;
            this.matchRule = matchRule;
        }
        public LogFilterString(long min, long max)
            : base(min, max)
        {
            matchRule = MatchRule.Length;
        }

        protected bool Match(string text) { return Match(text, matchRule); }
        protected bool Match(string text, MatchRule rule)
        {
            switch (rule)
            {
                case MatchRule.Length:
                    return Match(text.Length);
                case MatchRule.Match:
                    if (IgnoreCase)
                    {
                        text = text.ToLower();
                        foreach (string var in matches)
                        {
                            if (text == var.ToLower()) return true;
                        }
                    }
                    else
                    {
                        foreach (string var in matches)
                        {
                            if (text == var) return true;
                        }
                    }
                    return false;
                case MatchRule.UnmatchAll:
                    return !Match(text, MatchRule.Match);
                case MatchRule.Contain:
                    if (IgnoreCase)
                    {
                        text = text.ToLower();
                        foreach (string var in matches)
                        {
                            if (text.Contains(var.ToLower())) return true;
                        }
                    }
                    else
                    {
                        foreach (string var in matches)
                        {
                            if (text.Contains(var)) return true;
                        }
                    }
                    return false;
                case MatchRule.UncontainAll:
                    return !Match(text, MatchRule.Contain);
                case MatchRule.ContainAll:
                    if (IgnoreCase)
                    {
                        text = text.ToLower();
                        foreach (string var in matches)
                        {
                            if (!text.Contains(var.ToLower())) return false;
                        }
                    }
                    else
                    {
                        foreach (string var in matches)
                        {
                            if (!text.Contains(var)) return false;
                        }
                    }
                    return true;
                case MatchRule.Uncontain:
                    return !Match(text, MatchRule.ContainAll);
                case MatchRule.Start:
                    if (IgnoreCase)
                    {
                        text = text.ToLower();
                        foreach (string var in matches)
                        {
                            if (text.StartsWith(var.ToLower())) return true;
                        }
                    }
                    else
                    {
                        foreach (string var in matches)
                        {
                            if (text.StartsWith(var)) return true;
                        }
                    }
                    return false;
                case MatchRule.NotStartAll:
                    return !Match(text, MatchRule.Start);
                case MatchRule.End:
                    if (IgnoreCase)
                    {
                        text = text.ToLower();
                        foreach (string var in matches)
                        {
                            if (text.EndsWith(var.ToLower())) return true;
                        }
                    }
                    else
                    {
                        foreach (string var in matches)
                        {
                            if (text.EndsWith(var)) return true;
                        }
                    }
                    return false;
                case MatchRule.NotEndAll:
                    return !Match(text, MatchRule.End);
                default: return false;
            }
        }

        protected override void SaveImpl(XmlWriter writer)
        {
            writer.WriteAttributeString("match", matchRule.ToString());
            writer.WriteAttributeString("ignorecase", IgnoreCase.ToString());
            foreach (string var in matches)
            {
                writer.WriteElementString("text", var);
            }
            if (Min != long.MinValue) writer.WriteElementString("min", Min.ToString());
            if (Max != long.MaxValue) writer.WriteElementString("max", Max.ToString());
        }
        protected override void LoadImpl(XmlReader reader)
        {
            Min = long.MinValue; Max = long.MaxValue;
            LinkedList<string> list = new LinkedList<string>();
            try
            {
                matchRule = (MatchRule)Enum.Parse(typeof(MatchRule), reader.GetAttribute("match"), true);
            }
            catch (Exception)
            {
                matchRule = MatchRule.Match;
            }
            try
            {
                IgnoreCase = bool.Parse(reader.GetAttribute("ignorecase"));
            }
            catch (Exception)
            {
                IgnoreCase = false;
            }
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (EndFilter(reader)) break;
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "text")
                        list.AddLast(reader.ReadString());
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "min")
                        Min = long.Parse(reader.ReadString());
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "max")
                        Max = long.Parse(reader.ReadString());
                }
            }
            matches = new string[list.Count];
            list.CopyTo(matches, 0);
        }

        protected new string ToString(string name)
        {
            if (matchRule == MatchRule.Length) return base.ToString(name + "の長さ");
            if (matches.Length >= 2)
            {
                string prefix = string.Format("{0}が「{1}」など{2}個の文字列の", name, matches[0], matches.Length);
                switch (matchRule)
                {
                    case MatchRule.Match:
                        return prefix + "どれかひとつに完全一致";
                    case MatchRule.UnmatchAll:
                        return prefix + "どれにも完全一致しない";
                    case MatchRule.Contain:
                        return prefix + "どれかひとつを含む";
                    case MatchRule.UncontainAll:
                        return prefix + "どれも含まない";
                    case MatchRule.ContainAll:
                        return prefix + "すべてを含む";
                    case MatchRule.Uncontain:
                        return prefix + "すべては含まない";
                    case MatchRule.Start:
                        return prefix + "どれかひとつから始まる";
                    case MatchRule.NotStartAll:
                        return prefix + "どれからも始まらない";
                    case MatchRule.End:
                        return prefix + "どれかひとつで終わる";
                    case MatchRule.NotEndAll:
                        return prefix + "どれでも終わらない";
                    default:
                        return name + "に関する不正なフィルタ";
                }
            }
            else if (matches.Length == 1)
            {
                string prefix = string.Format("{0}が「{1}」", name, matches[0]);
                switch (matchRule)
                {
                    case MatchRule.Match:
                        return prefix + "に完全一致";
                    case MatchRule.UnmatchAll:
                        return prefix + "に完全一致しない";
                    case MatchRule.Contain:
                        return prefix + "を含む";
                    case MatchRule.UncontainAll:
                        return prefix + "を含まない";
                    case MatchRule.ContainAll:
                        return prefix + "を含む";
                    case MatchRule.Uncontain:
                        return prefix + "を含まない";
                    case MatchRule.Start:
                        return prefix + "から始まる";
                    case MatchRule.NotStartAll:
                        return prefix + "から始まらない";
                    case MatchRule.End:
                        return prefix + "で終わる";
                    case MatchRule.NotEndAll:
                        return prefix + "で終わらない";
                    default:
                        return name + "に関する不正なフィルタ";
                }
            }
            else
            {
                string prefix = string.Format("{0}が", name);
                switch (matchRule)
                {
                    case MatchRule.Match:
                        return prefix + "どれかひとつに完全一致(全遮断)";
                    case MatchRule.UnmatchAll:
                        return prefix + "どれにも完全一致しない(無条件)";
                    case MatchRule.Contain:
                        return prefix + "どれかひとつを含む(全遮断)";
                    case MatchRule.UncontainAll:
                        return prefix + "どれも含まない(無条件)";
                    case MatchRule.ContainAll:
                        return prefix + "すべてを含む(無条件)";
                    case MatchRule.Uncontain:
                        return prefix + "すべては含まない(全遮断)";
                    case MatchRule.Start:
                        return prefix + "どれかひとつから始まる(全遮断)";
                    case MatchRule.NotStartAll:
                        return prefix + "どれからも始まらない(無条件)";
                    case MatchRule.End:
                        return prefix + "どれかひとつで終わる(全遮断)";
                    case MatchRule.NotEndAll:
                        return prefix + "どれでも終わらない(無条件)";
                    default:
                        return name + "に関する不正なフィルタ";
                }
            }
        }
        protected override string ToString(long value) { return value.ToString() + "文字"; }
    }

    /// <summary>列挙型を対象としたフィルタ</summary>
    public abstract class LogFilterEnum : LogFilter
    {
        public Dictionary<object, bool> Matches;
        private Type EnumType;

        public LogFilterEnum(Type enumType)
        {
            Matches = new Dictionary<object, bool>();
            EnumType = enumType;
            foreach (object val in Enum.GetValues(EnumType))
            {
                Matches.Add(val, false);
            }
        }

        public bool Match(object value)
        {
            return Matches[value];
        }

        protected override void SaveImpl(XmlWriter writer)
        {
            foreach (object val in Enum.GetValues(EnumType))
            {
                writer.WriteElementString(val.ToString().ToLower(), Matches[val].ToString().ToLower());
            }
        }

        protected override void LoadImpl(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (EndFilter(reader)) break;
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        object e = Enum.Parse(EnumType, reader.LocalName, true);
                        Matches[e] = bool.Parse(reader.ReadString());
                    }
                }
            }
        }

        protected string ToString(string name)
        {
            string ret = name + "に関するフィルタ(全遮断)";
            int truecount = 0;
            foreach (KeyValuePair<object, bool> keyvalue in Matches)
            {
                if (keyvalue.Value)
                {
                    if (truecount == 0) ret = name + "が" + keyvalue.Key.ToString();
                    truecount++;
                }
            }
            if (truecount == 0) return ret;
            if (truecount == 1) return ret + "に一致";
            if (truecount == Matches.Count) return name + "に関するフィルタ(無条件)";
            return ret + "など" + truecount.ToString() + "個の要素のどれかに一致";
        }
    }

    /// <summary>集合フィルタ。複数のフィルタを組み合わせたフィルタです。</summary>
    public abstract class LogFilterCollection : LogFilter
    {
        public LogFilter[] subFilter = new LogFilter[0];

        public LogFilterCollection() { }
        public LogFilterCollection(LogFilter[] subFilter)
        {
            this.subFilter = subFilter;
        }

        protected override void SaveImpl(XmlWriter writer)
        {
            foreach (LogFilter filter in subFilter)
            {
                filter.Save(writer);
            }
        }
        protected override void LoadImpl(XmlReader reader)
        {
            LinkedList<LogFilter> list = new LinkedList<LogFilter>();
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (EndFilter(reader)) break;
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.ToLower() == "filter")
                        list.AddLast(Create(reader));
                }
            }
            subFilter = new LogFilter[list.Count];
            list.CopyTo(subFilter, 0);
        }
    }

    #endregion

    /// <summary>全てのログを通す</summary>
    public class LogFilterAll : LogFilter
    {
        public override bool Match(Log l) { return true; }
        public override ELogFilter FilterType() { return ELogFilter.All; }
        public override string ToString() { return "無条件"; }
        protected override void LoadImpl(XmlReader reader)
        {
            if (reader.IsEmptyElement) return;
            while (reader.Read())
            {
                if (EndFilter(reader)) return;
            }
        }
        protected override void SaveImpl(XmlWriter writer) { }
        public override Log[] Extract(Log[] logs) { return logs; }
    }

    /// <summary>不明なフィルタは何も通さない</summary>
    public class LogFilterUnknown : LogFilter
    {
        public override bool Match(Log l) { return false; }
        public override ELogFilter FilterType() { return ELogFilter.Unknown; }
        public override string ToString() { return "不明なフィルタ"; }
        protected override void LoadImpl(XmlReader reader)
        {
            if (reader.IsEmptyElement) return;
            while (reader.Read())
            {
                if (EndFilter(reader)) return;
            }
        }
        protected override void SaveImpl(XmlWriter writer) { }
        public override Log[] Extract(Log[] logs) { return new Log[0]; }
    }

    #region 個別フィルタ

    /// <summary>ホスト名/IPに対するフィルタ</summary>
    public class LogFilterHost : LogFilterString
    {
        public LogFilterHost() { }
        public LogFilterHost(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterHost(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.Host); }
        public override ELogFilter FilterType() { return ELogFilter.Host; }
        public override string ToString() { return ToString("ホスト名/IP"); }
    }

    /// <summary>リモートログ名に対するフィルタ</summary>
    public class LogFilterRemoteLog : LogFilterString
    {
        public LogFilterRemoteLog() { }
        public LogFilterRemoteLog(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterRemoteLog(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.RemoteLog); }
        public override ELogFilter FilterType() { return ELogFilter.RemoteLog; }
        public override string ToString() { return ToString("リモートログ名"); }
    }

    /// <summary>ユーザー名に対するフィルタ</summary>
    public class LogFilterUser : LogFilterString
    {
        public LogFilterUser() { }
        public LogFilterUser(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterUser(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.User); }
        public override ELogFilter FilterType() { return ELogFilter.User; }
        public override string ToString() { return ToString("ユーザー名"); }
    }

    /// <summary>日付に対するフィルタ</summary>
    public class LogFilterDate : LogFilterNumber
    {
        public LogFilterDate() { }
        public LogFilterDate(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.lDate); }
        public override ELogFilter FilterType() { return ELogFilter.Date; }

        public override long LowerLimit() { return new DateTime(1901, 1, 1, 0, 0, 0).Ticks; }
        public override long UpperLimit() { return new DateTime(2101, 1, 1, 0, 0, 0).Ticks - 1; }
        public override string ToString() { return ToString("日付"); }
        protected override string ToString(long value) { return new DateTime(value).ToString(); }
    }

    /// <summary>月のみに対するフィルタ</summary>
    public class LogFilterMonth : LogFilterNumber
    {
        public LogFilterMonth() { }
        public LogFilterMonth(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(new DateTime(l.lDate).Month); }
        public override ELogFilter FilterType() { return ELogFilter.Month; }

        public override long LowerLimit() { return 1; }
        public override long UpperLimit() { return 12; }
        public override string ToString() { return ToString("月"); }
        protected override string ToString(long value) { return value.ToString() + "月"; }
    }

    /// <summary>日のみに対するフィルタ</summary>
    public class LogFilterDay : LogFilterNumber
    {
        public LogFilterDay() { }
        public LogFilterDay(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(new DateTime(l.lDate).Day); }
        public override ELogFilter FilterType() { return ELogFilter.Day; }

        public override long LowerLimit() { return 1; }
        public override long UpperLimit() { return 31; }
        public override string ToString() { return ToString("日"); }
        protected override string ToString(long value) { return value.ToString() + "日"; }
    }

    /// <summary>曜日に対するフィルタ</summary>
    public class LogFilterWeekday : LogFilterEnum
    {
        public LogFilterWeekday() : base(typeof(DayOfWeek)) { }
        public override bool Match(Log l) { return Match(new DateTime(l.lDate).DayOfWeek); }
        public override ELogFilter FilterType() { return ELogFilter.Weekday; }
        public override string ToString() { return ToString("曜日"); }
    }

    /// <summary>時間に対するフィルタ</summary>
    public class LogFilterHour : LogFilterNumber
    {
        public LogFilterHour() { }
        public LogFilterHour(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(new DateTime(l.lDate).Hour); }
        public override ELogFilter FilterType() { return ELogFilter.Hour; }

        public override long LowerLimit() { return 0; }
        public override long UpperLimit() { return 23; }
        public override string ToString() { return ToString("時間"); }
        protected override string ToString(long value) { return value.ToString() + "時"; }
    }

    /// <summary>メソッドに対するフィルタ</summary>
    public class LogFilterMethod : LogFilterEnum
    {
        public LogFilterMethod() : base(typeof(Log.EMethod)) { }
        public override bool Match(Log l) { return Match(l.Method); }
        public override ELogFilter FilterType() { return ELogFilter.Method; }
        public override string ToString() { return ToString("メソッド"); }
    }

    /// <summary>リクエスト先に対するフィルタ</summary>
    public class LogFilterRequested : LogFilterString
    {
        public LogFilterRequested() { }
        public LogFilterRequested(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterRequested(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.Requested); }
        public override ELogFilter FilterType() { return ELogFilter.Requested; }
        public override string ToString() { return ToString("リクエスト先"); }
    }

    /// <summary>HTTPのバージョンに対するフィルタ</summary>
    public class LogFilterHTTP : LogFilterEnum
    {
        public LogFilterHTTP() : base(typeof(Log.EHTTP)) { }
        public override bool Match(Log l) { return Match(l.eHTTP); }
        public override ELogFilter FilterType() { return ELogFilter.HTTP; }
        public override string ToString() { return ToString("HTTPのバージョン"); }
    }

    /// <summary>ステータスコードに対するフィルタ</summary>
    public class LogFilterStatusCode : LogFilterNumber
    {
        public LogFilterStatusCode() { }
        public LogFilterStatusCode(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.Status); }
        public override ELogFilter FilterType() { return ELogFilter.StatusCode; }

        public override long LowerLimit() { return 100; }
        public override long UpperLimit() { return 999; }
        public override string ToString() { return ToString("ステータスコード"); }
    }

    /// <summary>転送量に対するフィルタ</summary>
    public class LogFilterSendSize : LogFilterNumber
    {
        public LogFilterSendSize() { }
        public LogFilterSendSize(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.SendSize); }
        public override ELogFilter FilterType() { return ELogFilter.SendSize; }

        public override string ToString() { return ToString("時間"); }
        protected override string ToString(long value)
        {
            return
                value == 0 ? "0B" :
                value < 1024 ? string.Format("{0}B", value) :
                value < 10240 ? string.Format("{0:0.00}KB", (1.0 / 1024.0) * value) :
                value < 102400 ? string.Format("{0:00.0}KB", (1.0 / 1024.0) * value) :
                value < 1048576 ? string.Format("{0:000}KB", (1.0 / 1024.0) * value) :
                value < 10485760 ? string.Format("{0:0.00}MB", (1.0 / 1048576.0) * value) :
                value < 104857600 ? string.Format("{0:00.0}MB", (1.0 / 1048576.0) * value) :
                value < 1073741824 ? string.Format("{0:000}MB", (1.0 / 1048576.0) * value) :
                string.Format("{0:0.00}GB", (1.0 / 1073741824.0) * value);
        }
    }

    /// <summary>リファラに対するフィルタ</summary>
    public class LogFilterReferer : LogFilterString
    {
        public LogFilterReferer() { }
        public LogFilterReferer(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterReferer(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.Referer); }
        public override ELogFilter FilterType() { return ELogFilter.Referer; }
        public override string ToString() { return ToString("リファラ"); }
    }

    /// <summary>リファラのドメインに対するフィルタ</summary>
    public class LogFilterDomain : LogFilterString
    {
        public LogFilterDomain() { }
        public LogFilterDomain(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterDomain(long min, long max) : base(min, max) { }

        public override bool Match(Log l)
        {
            if (l.Referer.Contains("://"))
            {
                try { return Match(new Uri(l.Referer).Host); }
                catch (Exception) { }
            }
            return false;
        }
        public override ELogFilter FilterType() { return ELogFilter.Domain; }
        public override string ToString() { return ToString("リファラのドメイン"); }
    }

    /// <summary>検索に使われた語句に対するフィルタ</summary>
    public class LogFilterSearchPhrase : LogFilterString
    {
        RefererAnalyzer refererAnalyzer = new RefererAnalyzer();

        public LogFilterSearchPhrase() { }
        public LogFilterSearchPhrase(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterSearchPhrase(long min, long max) : base(min, max) { }

        public override bool Match(Log l)
        {
            if (l.Referer == null || l.Referer.Length < 8) return false;
            try
            {
                var phrase = refererAnalyzer.GetSearchQuery(l.Referer);
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    return Match(phrase);
                }
            }
            catch (Exception)
            {
                // エラーは無視しちゃおうねー
            }
            return false;
        }
        public override ELogFilter FilterType() { return ELogFilter.SearchPhrase; }
        public override string ToString() { return ToString("検索に使われた語句"); }
    }

    /// <summary>ユーザーエージェントに対するフィルタ</summary>
    public class LogFilterUserAgent : LogFilterString
    {
        public LogFilterUserAgent() { }
        public LogFilterUserAgent(string matches, bool ignoreCase, MatchRule matchRule) : base(matches, ignoreCase, matchRule) { }
        public LogFilterUserAgent(long min, long max) : base(min, max) { }

        public override bool Match(Log l) { return Match(l.UserAgent); }
        public override ELogFilter FilterType() { return ELogFilter.UserAgent; }
        public override string ToString() { return ToString("ユーザーエージェント"); }
    }

    #endregion

    #region 集合フィルタ

    /// <summary>すべてのサブフィルタにマッチするログを通すフィルタ</summary>
    public class LogFilterAnd : LogFilterCollection
    {
        public LogFilterAnd() { }
        public LogFilterAnd(params LogFilter[] subFilter) : base(subFilter) { }

        public override bool Match(Log l)
        {
            foreach (LogFilter filter in subFilter)
            {
                if (!filter.Match(l)) return false;
            }
            return true;
        }

        public override ELogFilter FilterType() { return ELogFilter.And; }
        public override string ToString()
        {
            return subFilter.Length + "個のフィルタにすべてマッチ";
        }
    }

    /// <summary>少なくともひとつのサブフィルタにマッチするログを通すフィルタ</summary>
    public class LogFilterOr : LogFilterCollection
    {
        public LogFilterOr() { }
        public LogFilterOr(params LogFilter[] subFilter) : base(subFilter) { }

        public override bool Match(Log l)
        {
            foreach (LogFilter filter in subFilter)
            {
                if (filter.Match(l)) return true;
            }
            return false;
        }

        public override ELogFilter FilterType() { return ELogFilter.Or; }
        public override string ToString()
        {
            return subFilter.Length + "個のフィルタのどれかにマッチ";
        }
    }

    /// <summary>全てのサブフィルタにはマッチしないログを通すフィルタ</summary>
    public class LogFilterNand : LogFilterCollection
    {
        public LogFilterNand() { }
        public LogFilterNand(params LogFilter[] subFilter) : base(subFilter) { }

        public override bool Match(Log l)
        {
            foreach (LogFilter filter in subFilter)
            {
                if (!filter.Match(l)) return true;
            }
            return false;
        }

        public override ELogFilter FilterType() { return ELogFilter.Nand; }
        public override string ToString()
        {
            return subFilter.Length + "個のフィルタのすべてにはマッチしない";
        }
    }

    /// <summary>どのサブフィルタにもマッチしないログを通すフィルタ</summary>
    public class LogFilterNor : LogFilterCollection
    {
        public LogFilterNor() { }
        public LogFilterNor(params LogFilter[] subFilter) : base(subFilter) { }

        public override bool Match(Log l)
        {
            foreach (LogFilter filter in subFilter)
            {
                if (filter.Match(l)) return false;
            }
            return true;
        }

        public override ELogFilter FilterType() { return ELogFilter.Nor; }
        public override string ToString()
        {
            return subFilter.Length + "個のフィルタのどれにもマッチしない";
        }
    }

    #endregion
}
