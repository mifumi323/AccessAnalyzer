using System;
using System.Text;

namespace MifuminLib.AccessAnalyzer
{
    public class RefererAnalyzer
    {
        private enum CodePage : int
        {
            Shift_JIS = 932,
            EUC_JP = 51932,
            UTF8 = 65001,
        }

        /// <summary>
        /// 検索サイトの検索結果のページのURLから検索に使われた言葉を抽出します。
        /// </summary>
        /// <param name="urlstring">対象となる検索ページのURL</param>
        /// <returns>抽出した検索フレーズ(見つからなければnull)</returns>
        public static string GetSearchPhrase(string urlstring)
        {
            string phrase = null;
            if (TryGetSearchPhrase(urlstring, ref phrase))
            {
                return phrase;
            }
            return null;
        }

        /// <summary>
        /// 検索サイトの検索結果のページのURLから検索に使われた言葉を抽出します。
        /// </summary>
        /// <param name="urlstring">対象となる検索ページのURL</param>
        /// <param name="phrase">抽出した検索フレーズを格納する変数</param>
        /// <returns>指定したURLから検索フレーズが抽出できたかどうか</returns>
        public static bool TryGetSearchPhrase(string urlstring, ref string phrase)
        {
            if (!Uri.IsWellFormedUriString(urlstring, UriKind.Absolute)) return false;
            try
            {
                Uri uri = new Uri(urlstring);
                if (string.IsNullOrEmpty(uri.Query)) return false;
                string domain = uri.Host;
                if (domain.Contains("google.co")) return TryGetGoogleSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("yahoo.co")) return TryGetYahooSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("msn.co") || domain.Contains("live.co") || domain.Contains("bing.co")) return TryGetMSNSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("biglobe.ne")) return TryGetBiglobeSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("goo.ne")) return TryGetGooSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("ezsch.ezweb.ne")) return TryGetEzSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("docomo.ne")) return TryGetDocomoSearchPhrase(uri.Query, ref phrase);
                if (domain.Contains("search.rakuten.co")) return TryGetRakutenSearchPhrase(uri.Query, ref phrase);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryGetGoogleSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            //Encoding encoding = Encoding.UTF8;
            CodePage codepage = CodePage.UTF8;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "q" || key == "as_q" || key == "as_epq" || key == "as_oq")
                        phrase = s[1];
                    //if (key == "as_eq") phrase = s[1];  // as_eqはマイナス検索に相当
                    if (key == "ie")    // oeは表示用の文字コードなので無視する
                    {
                        string value = s[1].ToLower();
                        if (value == "sjis" || value == "shift_jis") codepage = CodePage.Shift_JIS; // encoding = Encoding.GetEncoding("Shift_JIS");
                        if (value == "euc-jp") codepage = CodePage.EUC_JP; // Encoding.GetEncoding("EUC-JP");
                    }
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                if (phrase.Contains("\\x"))
                    phrase = DecodeUrlWithBackSlash(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetYahooSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.EUC_JP;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "p" || key == "va" || key == "vo" || key == "vp")
                        phrase = s[1];
                    //if (key == "ve") phrase = s[1];  // veはNOT検索に相当
                    if (key == "ei")
                    {
                        string value = s[1].ToLower();
                        if (value == "utf-8") codepage = CodePage.UTF8;
                        if (value == "sjis" || value == "shift_jis") codepage = CodePage.Shift_JIS;
                        if (value == "euc-jp") codepage = CodePage.EUC_JP;
                    }
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetMSNSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.UTF8;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "q")
                        phrase = s[1];
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetBiglobeSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.EUC_JP;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "q" || key == "search")
                        phrase = s[1];
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetGooSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.EUC_JP;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "mt")
                        phrase = s[1];
                    if (key == "ie")
                    {
                        string value = s[1].ToLower();
                        if (value == "utf-8") codepage = CodePage.UTF8;
                        if (value == "sjis" || value == "shift_jis") codepage = CodePage.Shift_JIS;
                    }
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetEzSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.Shift_JIS;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "query")
                        phrase = s[1];
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetDocomoSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.Shift_JIS;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "key")
                        phrase = s[1];
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static bool TryGetRakutenSearchPhrase(string query, ref string phrase)
        {
            string[] array = query.Substring(1).Split('&');
            phrase = "";
            CodePage codepage = CodePage.UTF8;
            foreach (string var in array)
            {
                string[] s = var.Split('=');
                if (s.Length >= 2)
                {
                    string key = s[0].ToLower();
                    if (key == "qt")
                        phrase = s[1];
                }
            }
            if (phrase.Length > 0)
            {
                if (phrase.Contains("%"))
                    phrase = System.Web.HttpUtility.UrlDecode(phrase, GetEncoding(codepage));
                return true;
            }
            return false;
        }

        private static string DecodeUrlWithBackSlash(string encoded, Encoding encoding)
        {
            char[] chars = encoded.ToCharArray();
            byte[] ret = new byte[chars.Length];
            int j = 0;
            for (int i = 0; i < chars.Length; )
            {
                if (i < chars.Length - 3 && chars[i] == '\\' && chars[i + 1] == 'x')
                {
                    bool ok = true;
                    byte code = 0;
                    char c = chars[i + 2];
                    if (char.IsDigit(c)) code = (byte)((c - '0') * 16);
                    else if ('a' <= c && c <= 'f') code = (byte)((c + 10 - 'a') * 16);
                    else if ('A' <= c && c <= 'F') code = (byte)((c + 10 - 'A') * 16);
                    c = chars[i + 3];
                    if (char.IsDigit(c)) code += (byte)(c - '0');
                    else if ('a' <= c && c <= 'f') code += (byte)(c + 10 - 'a');
                    else if ('A' <= c && c <= 'F') code += (byte)(c + 10 - 'A');
                    if (ok)
                    {
                        ret[j++] = code;
                        i += 4;
                        continue;
                    }
                }
                ret[j++] = (byte)chars[i++];
            }
            return encoding.GetString(ret, 0, j);
        }

        private static Encoding GetEncoding(CodePage codepage)
        {
            return Encoding.GetEncoding((int)codepage, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback);
        }
    }
}
