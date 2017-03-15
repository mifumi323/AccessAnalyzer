﻿using System;
using System.Web;

namespace MifuminLib.AccessAnalyzer
{
    public class RefererAnalyzer
    {
        /// <summary>
        /// 検索サイトの検索結果のページのURLから検索に使われた言葉を抽出します。
        /// </summary>
        /// <param name="urlstring">対象となる検索ページのURL</param>
        /// <returns>抽出した検索フレーズ(見つからなければnull)</returns>
        public static string GetSearchPhrase(string urlstring)
        {
            return new RefererAnalyzer().GetSearchQuery(urlstring);
        }

        /// <summary>
        /// 検索サイトの検索結果のページのURLから検索に使われた言葉を抽出します。
        /// </summary>
        /// <param name="urlstring">対象となる検索ページのURL</param>
        /// <param name="phrase">抽出した検索フレーズを格納する変数</param>
        /// <returns>指定したURLから検索フレーズが抽出できたかどうか</returns>
        public static bool TryGetSearchPhrase(string urlstring, ref string phrase)
        {
            phrase = GetSearchPhrase(urlstring);
            return phrase != null;
        }

        public virtual string GetSearchQuery(string uriString)
        {
            if (!uriString.StartsWith("http://") && !uriString.StartsWith("https://")) return null;
            var uri = new Uri(uriString);
            switch (uri.Host)
            {
                case "www.google.com":
                case "www.google.co.jp":
                case "www.google.co.kr":
                case "www.google.com.br":
                case "www.google.it":
                case "www.google.es":
                case "images.google.co.jp":
                case "cgi.search.biglobe.ne.jp":
                case "search.auone.jp":
                case "sp-web.search.auone.jp":
                case "www.bing.com":
                case "cn.bing.com":
                case "www.msn.com":
                case "search.kensakuplus.com":
                case "eonet.jp":
                case "jwsearch.jword.jp":
                case "mobss.jword.jp":
                case "v-buster.jword.jp":
                case "search.fenrir-inc.com":
                case "search.jiqoo.jp":
                case "websearch.excite.co.jp":
                case "www.search.ask.com":
                case "nortonsafe.search.ask.com":
                case "search.kinza.jp":
                case "search.searchnewsplus.com":
                case "search.dolphin-browser.jp":
                case "search.fooooo.com":
                case "search.freespot.com":
                case "decomailer.awalker.jp":
                case "search.myjcom.jp":
                case "search.crav-ing.com":
                    return GetSearchSimpleQuery(uri, "q");
                case "search.yahoo.com":
                case "search.yahoo.co.jp":
                case "image.search.yahoo.co.jp":
                case "images.search.yahoo.com":
                case "hk.images.search.yahoo.com":
                case "realtime.search.yahoo.co.jp":
                case "cache.yahoofs.jp":
                    return GetSearchSimpleQuery(uri, "p");
                case "search.smt.docomo.ne.jp":
                case "search.goo.ne.jp":
                    return GetSearchSimpleQuery(uri, "MT");
                case "websearch.rakuten.co.jp":
                case "app.websearch.rakuten.co.jp":
                    return GetSearchSimpleQuery(uri, "qt");
                case "wakwakpc.starthome.jp":
                case "pex.jp":
                case "home.kingsoft.jp":
                    return GetSearchSimpleQuery(uri, "keyword");
                case "int.search.tb.ask.com":
                    return GetSearchSimpleQuery(uri, "searchfor");
                case "docomo.ne.jp":
                    return GetSearchSimpleQuery(uri, "key");
                case "ecnavi.jp":
                case "search.kawaii.aswidget.com":
                case "search.plushome.aswidget.com":
                case "gws.cybozu.net":
                    return GetSearchSimpleQuery(uri, "Keywords");
                case "www.so-net.ne.jp":
                case "jp.hao123.com":
                    return GetSearchSimpleQuery(uri, "query");
                case "search.nifty.com":
                case "search.azby.fmworld.net":
                    return GetSearchSimpleQuery(uri, "Text") ?? GetSearchSimpleQuery(uri, "q");
                case "imagesearch.excite.co.jp":
                    return GetSearchSimpleQuery(uri, "q") ?? GetSearchSimpleQuery(uri, "search");
                default:
                    break;
            }
            return null;
        }

        protected virtual string GetSearchSimpleQuery(Uri uri, string key)
        {
            var queries = HttpUtility.ParseQueryString(uri.Query);
            var values = queries.GetValues(key);
            if (values != null && values.Length > 0) return values[0];
            return null;
        }
    }
}
