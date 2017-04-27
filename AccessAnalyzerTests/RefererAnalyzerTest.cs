using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MifuminLib.AccessAnalyzer;

namespace AccessAnalyzerTests
{
    [TestClass]
    public class RefererAnalyzerTest
    {
        RefererAnalyzer _refererAnalyzer = new RefererAnalyzer();

        private void SearchQueryTest(string urlString, string expectedQuery)
        {
            Assert.AreEqual(expectedQuery, _refererAnalyzer.GetSearchQuery(urlString), urlString);
        }

        [TestMethod]
        public void NoSearchQuery()
        {
            SearchQueryTest(@"https://www.google.co.jp/", null);
            SearchQueryTest(@"http://search.yahoo.co.jp/", null);
            SearchQueryTest(@"http://www.google.co.jp/", null);
            SearchQueryTest(@"https://www.bing.com/", null);
            SearchQueryTest(@"https://www.google.com/", null);
        }

        [TestMethod]
        public void WwwGoogleComSearchQuery()
        {
            SearchQueryTest(
                @"https://www.google.com/search?q=%e4%b8%89%e6%ac%a1%e5%85%83+%e7%b7%9a%e5%88%86+%e8%b7%9d%e9%9b%a2&ie=utf-8&oe=utf-8&client=firefox-b",
                @"三次元 線分 距離");
        }

        [TestMethod]
        public void WwwGoogleCoJpSearchQuery()
        {
            SearchQueryTest(
                @"https://www.google.co.jp/search?safe=active&sclient=tablet-gws&site=&source=hp&q=%e3%83%9c%e3%83%b3%e3%83%90%e3%83%bc%e3%83%9e%e3%83%b3+%e3%81%9b%e3%82%93%e3%81%97%e3%81%8b%e3%81%b6%e3%81%a8&oq=%e3%83%9c%e3%83%b3%e3%83%90%e3%83%bc%e3%83%9e%e3%83%b3+%e3%81%9b%e3%82%93%e3%81%97%e3%81%8b%e3%81%b6%e3%81%a8&gs_l=tablet-gws.3...2895.12324.0.13042.14.14.0.0.0.0.254.1737.0j9j2.11.0....0...1c.1.64.tablet-gws..3.6.911...0j0i4k1j0i131k1j0i4i30k1.ZUJEzHEEts8",
                @"ボンバーマン せんしかぶと");
            SearchQueryTest(
                @"http://www.google.co.jp/search?client=ds&hl=ja&rls=com.nintendo.ds:2+JP&ie=utf8&oe=utf8&q=%e7%88%86%e3%83%9c%e3%83%b3%e3%83%90%e3%83%bc%e3%83%9e%e3%83%b3+%e6%94%bb%e7%95%a5",
                @"爆ボンバーマン 攻略");
        }

        [TestMethod]
        public void ImagesGoogleCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://images.google.co.jp/imgres?imgurl=https://tgws.plus/anpandb/image-289s&imgrefurl=https://tgws.plus/anpandb/chara-15&h=120&w=160&tbnid=6YGWixDFv3fRuM:&vet=1&q=%e3%82%84%e3%81%aa%e3%81%9b%e3%81%9f%e3%81%8b%e3%81%97:%e3%82%aa%e3%83%bc%e3%83%ad%e3%83%a9%e5%a7%ab&tbnh=74&tbnw=98&iact=rc&usg=__xtaeD7JvtPzPjgvg223vfK6Cphs=&hl=ja&ei=hXo1WKjqNYeY8QXrq4KAAw&tbm=isch&client=ds&sa=X&ved=0ahUKEwjowLWm4L7QAhUHTLwKHeuVADA4DhCtAwgfMAE",
                @"やなせたかし:オーロラ姫");
            SearchQueryTest(
                @"http://images.google.co.jp/imgres?imgurl=https://tgws.plus/anpandb/pimg-00114000-00113471&imgrefurl=https://tgws.plus/anpandb/chara-150&h=169&w=120&tbnid=mhWc-PTt5HEXcM:&vet=1&q=%e3%83%aa%e3%83%a3%e3%83%b3%e3%83%a1%e3%83%b3%e3%81%95%e3%82%93&tbnh=99&tbnw=70&iact=rc&usg=__a1iaya7sfsh0sg8X5Rion9IjDFY=&hl=ja&ei=IJ0tWNGQOYWj0gTr75X4BQ&tbm=isch&client=ds&sa=X&ved=0ahUKEwjRoN-W4K_QAhWFkZQKHet3BV8QrQMIMzAL",
                @"リャンメンさん");
        }

        [TestMethod]
        public void WwwGoogleComBrSearchQuery()
        {
            SearchQueryTest(
                @"https://www.google.com.br/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e8%aa%95%e7%94%9f%e3%81%a6%e3%82%93%e3%81%a9%e3%82%93%e3%81%be%e3%82%93&ie=utf-8&oe=utf-8&gws_rd=cr&ei=ebscWN7UJYeuwASH4JiYDA",
                @"アンパンマン誕生てんどんまん");
            SearchQueryTest(
                @"https://www.google.com.br/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3db&ie=utf-8&oe=utf-8&gws_rd=cr&ei=EL8cWOuNDcOewATExriYCg",
                @"アンパンマンdb");
        }

        [TestMethod]
        public void SearchYahooComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.yahoo.com/search?p=%e3%81%97%e3%82%87%e3%81%8f%e3%81%b1%e3%82%93%e3%81%be%e3%82%93+%e5%bc%b1%e3%82%8b&fr=onesearchnew",
                @"しょくぱんまん 弱る");
            SearchQueryTest(
                @"http://search.yahoo.com/search?p=%e3%83%95%e3%83%a9%e3%83%b3%e3%82%b1%e3%83%b3%e3%83%ad%e3%83%9c+%e3%83%91%e3%83%91&fr=onesearchnew",
                @"フランケンロボ パパ");
        }

        [TestMethod]
        public void SearchYahooCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.yahoo.co.jp/search?p=%e3%83%9b%e3%83%a9%e3%83%bc%e3%83%9e%e3%83%b3&ei=UTF-8&fr=applpd",
                @"ホラーマン");
            SearchQueryTest(
                @"http://image.search.yahoo.co.jp/search?rkf=2&ei=UTF-8&p=%e3%83%90%e3%82%a4%e3%82%ad%e3%83%b3%e4%bb%99%e4%ba%ba",
                @"バイキン仙人");
        }

        [TestMethod]
        public void ImageSearchYahooCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://image.search.yahoo.co.jp/search?rkf=2&ei=UTF-8&p=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%81%99%e3%81%aa%e3%81%8a%e3%81%a8%e3%81%93",
                @"アンパンマン すなおとこ");
            SearchQueryTest(
                @"http://image.search.yahoo.co.jp/search?rkf=2&ei=UTF-8&gdr=1&p=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%82%ad%e3%83%a3%e3%83%a9%e3%82%af%e3%82%bf%e3%83%bc+%e3%82%84%e3%81%bf%e3%82%8b%e3%82%93%e3%82%8b%e3%82%93",
                @"アンパンマンキャラクター やみるんるん");
        }

        [TestMethod]
        public void HkImagesSearchYahooComSearchQuery()
        {
            SearchQueryTest(
                @"https://hk.images.search.yahoo.com/search/images;_ylt=A2oKmLUqrTdYTXwASsuzygt.;_ylu=X3oDMTE1cXY4Y2VqBGNvbG8Dc2czBHBvcwMxBHZ0aWQDSEtDMDA4XzEEc2VjA3BpdnM-?p=%e6%9d%9f%e7%b8%9b&fr=yfp-t-117-s-hk&fr2=piv-web",
                @"束縛");
            SearchQueryTest(
                @"https://hk.images.search.yahoo.com/search/images;_ylt=A8tUwYZj8SNYlicA1eSzygt.;_ylu=X3oDMTE0bHJ0ZXU0BGNvbG8DdHcxBHBvcwMxBHZ0aWQDQjI1NDhfMQRzZWMDcGl2cw--?p=%e6%9d%9f%e7%b8%9b&fr=fp-handwrite-405-hk&fr2=piv-web",
                @"束縛");
        }

        [TestMethod]
        public void RealtimeSearchYahooCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://realtime.search.yahoo.co.jp/search?p=%e3%81%8b%e3%81%9c%e3%81%93%e3%82%93%e3%81%93%e3%82%93&ei=UTF-8&fr=top_ga1_sa",
                @"かぜこんこん");
            SearchQueryTest(
                @"http://realtime.search.yahoo.co.jp/search?p=%e5%a4%a7%e5%8f%8b%e9%be%8d%e4%b8%89%e9%83%8e&search.x=1&tid=top_ga1_sa&ei=UTF-8&aq=1&oq=%e5%a4%a7%e5%8f%8b%e3%82%8a%e3%82%85%e3%81%86&fr=top_ga1_sa",
                @"大友龍三郎");
        }

        [TestMethod]
        public void CacheYahoofsJpSearchQuery()
        {
            SearchQueryTest(
                @"http://cache.yahoofs.jp/search/cache?c=pI5Y_9rHgEIJ&p=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%a8%e3%82%b5%e3%83%b3%e3%82%bf%e3%81%ae%e6%89%8b%e7%b4%99&u=https://tgws.plus/anpandb/tv-xmas2014",
                @"アンパンマンとサンタの手紙");
            SearchQueryTest(
                @"http://cache.yahoofs.jp/search/cache?c=XQEWneEhysgJ&p=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e9%a8%92%e3%81%8c%e3%81%97%e3%81%84%e3%81%ae%e8%8b%a6%e6%89%8b&u=https://tgws.plus/anpandb/baikinman",
                @"アンパンマン 騒がしいの苦手");
        }

        [TestMethod]
        public void SearchSmtDocomoNeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.smt.docomo.ne.jp/result?MT=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3DB&SID=000&IND=000&TPLID=&UNIT=&URANK=&SPAGE=&PAGE=1",
                @"アンパンマンDB");
            SearchQueryTest(
                @"http://search.smt.docomo.ne.jp/result?MT=%e3%81%bd%e3%81%a3%e3%81%bd%e3%81%a1%e3%82%83%e3%82%93&SID=000&IND=000&TPLID=&UNIT=&URANK=&SPAGE=&PAGE=1",
                @"ぽっぽちゃん");
        }

        [TestMethod]
        public void DocomoNeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://docomo.ne.jp/cp/as-rslt.cgi?pno=1&key=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%80%81%e6%bf%a1%e3%82%8c%e3%81%9f%e9%a1%94&sid=000",
                @"アンパンマン、濡れた顔");
            SearchQueryTest(
                @"http://docomo.ne.jp/cp/as-rslt.cgi?pno=1&key=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%83%90%e3%83%aa%e3%82%a8%e3%83%bc%e3%82%b7%e3%83%a7%e3%83%b3&sid=B01",
                @"アンパンマン バリエーション");
        }

        [TestMethod]
        public void CgiSearchBiglobeNeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://cgi.search.biglobe.ne.jp/cgi-bin/search_bl_top?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3db&sub=%e6%a4%9c%e7%b4%a2&ie=utf8&num=10&start=0&ocgxa=0610",
                @"アンパンマンdb");
            SearchQueryTest(
                @"http://cgi.search.biglobe.ne.jp/cgi-bin/search2-b?search=%e6%a4%9c%e7%b4%a2&web_s.x=1&q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3DB&bt01=%e6%a4%9c%e7%b4%a2",
                @"アンパンマンDB");
        }

        [TestMethod]
        public void SearchGooNeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.goo.ne.jp/web.jsp?MT=%e3%81%9d%e3%82%8c%e3%81%84%e3%81%91%ef%bc%81%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3tgws&IE=sjis&OE=UTF-8&from=myvaiotop&PT=myvaio&sbd=sony001&MD=PC",
                @"それいけ！アンパンマンtgws");
            SearchQueryTest(
                @"http://search.goo.ne.jp/web.jsp?IE=UTF-8&OE=UTF-8&from=ocn_Default&PT=ocn_Default&sbd=ocn001&MT=%e7%88%86%e3%83%9c%e3%83%b3%e3%83%90%e3%83%bc%e3%83%9e%e3%83%b364",
                @"爆ボンバーマン64");
        }

        [TestMethod]
        public void SearchAuoneJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.auone.jp/?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%81+%e7%94%bb%e5%83%8f&sr=0001&ie=UTF-8&lr=",
                @"アンパンマン アンパンチ 画像");
            SearchQueryTest(
                @"http://search.auone.jp/?q=%e3%83%89%e3%82%ad%e3%83%b3%e3%81%a1%e3%82%83%e3%82%93&client=kddi-auone-suggest&sr=0101&ie=SJIS",
                @"ドキンちゃん");
        }

        [TestMethod]
        public void SpWebSearchAuoneJpSearchQuery()
        {
            SearchQueryTest(
                @"http://sp-web.search.auone.jp/search?client=mobile-kddi&channel=android&sr=0000&q=%e5%85%83%e6%b0%973%e5%80%8d%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3&tr=5354193e1b56b72b",
                @"元気3倍アンパンマン");
            SearchQueryTest(
                @"http://sp-web.search.auone.jp/search?client=mobile-kddi&channel=android&sr=0000&q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e9%b3%a5%e3%82%ad%e3%83%a3%e3%83%a9&tr=4280133cb2782c6a",
                @"アンパンマン鳥キャラ");
        }

        [TestMethod]
        public void WwwBingComSearchQuery()
        {
            SearchQueryTest(
                @"https://www.bing.com/search?q=%e8%b5%a4%e3%81%a1%e3%82%83%e3%82%93%e3%83%9e%e3%83%b3&form=MB10782&mkt=en-US&setlang=en-US",
                @"赤ちゃんマン");
            SearchQueryTest(
                @"http://www.bing.com/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%ef%bd%84%ef%bd%82&src=IE-TopResult&FORM=IETR02&conversationid=&pc=EUPP_",
                @"アンパンマンｄｂ");
        }

        [TestMethod]
        public void CnBingComSearchQuery()
        {
            SearchQueryTest(
                @"http://cn.bing.com/search?pc=cosp&ptag=A91255CDCDA&conlogo=CT3210127&q=%e3%82%b5%e3%82%af%e3%81%95%e3%82%93",
                @"サクさん");
            SearchQueryTest(
                @"http://cn.bing.com/search?q=%e3%82%af%e3%83%aa%e3%81%a1%e3%82%83%e3%82%93&qs=n&form=QBRE&pq=%e3%82%af%e3%83%aa%e3%81%a1%e3%82%83%e3%82%93&sc=0-5&sp=-1&sk=&cvid=D1F4BE2FD494403897440E35F02656F7",
                @"クリちゃん");
        }

        [TestMethod]
        public void WakwakpcStarthomeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://wakwakpc.starthome.jp/type/web/page/1/?area=web-searchbox&keyword=%e3%81%8e%e3%82%93%e3%81%84%e3%82%8d%e3%81%be%e3%82%93",
                @"ぎんいろまん");
            SearchQueryTest(
                @"http://wakwakpc.starthome.jp/type/web/page/1/?area=web-searchbox&keyword=%e3%83%a0%e3%83%bc%e3%83%b3%e5%a7%ab",
                @"ムーン姫");
        }

        [TestMethod]
        public void WebsearchRakutenCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://websearch.rakuten.co.jp/?tool_id=1&pid=1&rid=2000&ref=ff&qt=%e3%82%89%e3%81%8f%e3%81%8c%e3%81%8d%e3%81%93%e3%81%9e%e3%81%86",
                @"らくがきこぞう");
            SearchQueryTest(
                @"http://websearch.rakuten.co.jp/?tool_id=1&pid=1&rid=2000&ref=ff&qt=%e3%82%ad%e3%83%8e%e3%82%b3%e3%83%96%e3%83%a9%e3%82%b6%e3%83%bc%e3%82%ba+%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3",
                @"キノコブラザーズ アンパンマン");
        }

        [TestMethod]
        public void AppWebsearchRakutenCoJpSearchQuery()
        {
            SearchQueryTest(
                @"https://app.websearch.rakuten.co.jp/websearch?ref=searchbox&qt=%e3%81%8b%e3%81%a4%e3%81%b6%e3%81%97%e3%81%be%e3%82%93&client_id=1&col=OW&tool_id=1&filter=1&redirect=1",
                @"かつぶしまん");
            SearchQueryTest(
                @"https://app.websearch.rakuten.co.jp/websearch?ref=searchbox&qt=%e3%82%ab%e3%83%ac%e3%83%bc%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3&client_id=1&col=OW&tool_id=1&filter=1&redirect=11",
                @"カレーパンマン");
        }

        [TestMethod]
        public void IntSearchTbAskComSearchQuery()
        {
            SearchQueryTest(
                @"http://int.search.tb.ask.com/search/GGmain.jhtml?n=781b69f8&p2=^BBQ^xdm328^LAJAJP^jp&pg=GGmain&pn=1&ptb=75C02E40-7A35-4063-8173-27B7BDA39916&qs=&si=CLGon6aNrMYCFYiCvQodCKIM4Q&ss=sub&st=bar&searchfor=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%82%86%e3%81%9a%e5%a7%ab&tpr=jrel2&ots=1479520519080",
                @"アンパンマンゆず姫");
            SearchQueryTest(
                @"http://int.search.tb.ask.com/search/GGmain.jhtml?p2=^BBQ^xdm328^LAJAJP^jp&si=CNTT-M2b7MoCFdcSvQodO_UDyA&ptb=BBB9CF85-D3E1-4771-B419-8A1126BACCEE&ind=2016020922&n=782a09ba&st=bar&searchfor=PVD%e3%81%a8CVD%e3%81%a9%e3%81%a1%e3%82%89%e3%81%8c%e5%a4%9a%e3%81%8f%e4%bd%bf%e3%82%8f%e3%82%8c%e3%81%a6%e3%81%84%e3%82%8b%e3%81%8b%ef%bc%9f",
                @"PVDとCVDどちらが多く使われているか？");
        }

        [TestMethod]
        public void WwwSearchAskComSearchQuery()
        {
            SearchQueryTest(
                @"http://www.search.ask.com/web?apn_dtid=^OSJ000^YY^JP&apn_dbr=ie_11.0.9600.17280&psv=&itbv=12.16.2.54&p2=^BE7^OSJ000^YY^JP&apn_ptnrs=BE7&o=APN11461&gct=hp&pf=V7&tpid=ORJ-ST-SPE&trgb=IE&pt=tb&apn_uid=414833AE-FFBF-4788-B2EA-DAB78211FE78&doi=2014-09-27&ts=1478430289784&tpr=10&q=%e3%83%89%e3%82%ad%e3%83%b3%e3%81%a1%e3%82%83%e3%82%93%e3%81%ae%e3%83%80%e3%83%b3%e3%82%b9%e3%83%91%e3%83%bc%e3%83%86%e3%82%a3%e3%83%bc&page=2&ots=1478430352957",
                @"ドキンちゃんのダンスパーティー");
            SearchQueryTest(
                @"http://www.search.ask.com/web?o=APN11406&p2=^BBE^OSJ000^YY^JP&tpid=ORJ-SPE&gct=bar&apn_uid=53F600D4-A176-49DA-B88D-9FA890C8D8BD&apn_ptnrs=BBE&apn_dtid=^OSJ000^YY^JP&apn_dbr=ie_9.0.8112.16599&itbv=12.23.0.15&doi=2015-01-21&trgb=IE&tbv=12.23.0.15&crxv=135.7&pf=V7&pt=tb&psv=&q=%e3%81%82%e3%82%93%e3%81%b1%e3%82%93%e3%81%be%e3%82%93%e3%81%8a%e3%81%ab%e3%81%8e%e3%82%8a%e3%81%be%e3%82%93",
                @"あんぱんまんおにぎりまん");
        }

        [TestMethod]
        public void NortonsafeSearchAskComSearchQuery()
        {
            SearchQueryTest(
                @"http://nortonsafe.search.ask.com/web?q=%e3%81%be%e3%81%97%e3%82%85%e3%81%be%e3%82%8d%e3%81%95%e3%82%93&chn=retail&doi=&geo=JP&guid=&locale=ja_JP&o=APN10506&prt=360&ver=21&tpr=2&ts=1478750234683",
                @"ましゅまろさん");
            SearchQueryTest(
                @"https://nortonsafe.search.ask.com/web?chn=1000720&doi=2016-09-01&geo=JP&guid=017E2C4B-8AA2-4E1C-A3FD-60F0297723DF&locale=ja_JP&o=APN11915&p2=^ET^ih10jp^&page=1&prt=NS&ver=22&q=PVD%e3%81%a8%e3%81%8bCVD%e3%81%a8%e3%81%8b&tpr=5&ots=1479905361998",
                @"PVDとかCVDとか");
        }

        [TestMethod]
        public void SearchKensakuplusComSearchQuery()
        {
            SearchQueryTest(
                @"https://search.kensakuplus.com/web?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%81%94%e3%82%8d%e3%82%93%e3%81%94%e3%82%8d&secret=false&ref=url_input&theme=cyan",
                @"アンパンマン ごろんごろ");
            SearchQueryTest(
                @"https://search.kensakuplus.com/web?q=%e3%81%8b%e3%81%84%e3%81%98%e3%82%85%e3%81%86%e3%83%90%e3%83%aa%e3%83%b3%e3%82%ac%e3%83%bc&secret=false&ref=url_input&search_count=8&view_count=14&theme=cyan",
                @"かいじゅうバリンガー");
        }

        [TestMethod]
        public void EcnaviJpSearchQuery()
        {
            SearchQueryTest(
                @"http://ecnavi.jp/m/spsearch/?Keywords=%e3%83%91%e3%82%b9%e3%82%bf%e3%81%8a%e3%81%b0%e3%81%95%e3%82%93",
                @"パスタおばさん");
            SearchQueryTest(
                @"http://ecnavi.jp/search/bar/?Keywords=%e3%83%a1%e3%83%ad%e3%83%b3%e3%83%91%e3%83%b3%e3%81%aa+%e3%83%91%e3%83%b3%e3%83%81",
                @"メロンパンな パンチ");
        }

        [TestMethod]
        public void EonetJpSearchQuery()
        {
            SearchQueryTest(
                @"http://eonet.jp/search.html?q=%e3%81%9d%e3%82%8c%e3%81%84%e3%81%91%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e7%84%a1%e6%96%99%e7%94%bb%e5%83%8f",
                @"それいけアンパンマン無料画像");
            SearchQueryTest(
                @"http://eonet.jp/search.html?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e8%a1%a8%e6%83%85",
                @"アンパンマン 表情");
        }

        [TestMethod]
        public void JwsearchJwordJpSearchQuery()
        {
            SearchQueryTest(
                @"http://jwsearch.jword.jp/search?q=%e3%82%ab%e3%83%ac%e3%83%bc%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%a8%e3%81%8f%e3%82%8a%e3%81%8d%e3%82%93%e3%81%a8%e3%82%93&act=image&pvs=e-start-search-box&inec=utf-8&per_page=4",
                @"カレーパンマンとくりきんとん");
            SearchQueryTest(
                @"http://jwsearch.jword.jp/search?q=%e3%82%b3%e3%82%ad%e3%83%b3%e3%81%a1%e3%82%83%e3%82%93+%e7%94%bb%e5%83%8f&act=image&pvs=e-start-search-box&inec=utf-8&per_page=15",
                @"コキンちゃん 画像");
        }

        [TestMethod]
        public void MobssJwordJpSearchQuery()
        {
            SearchQueryTest(
                @"http://mobss.jword.jp/search?q=%e3%81%9e%e3%81%86%e3%81%ae%e3%82%ad%e3%83%a3%e3%83%a9%e3%82%af%e3%82%bf%e3%83%bc&p=simeji&pv=&t=web&sid=",
                @"ぞうのキャラクター");
            SearchQueryTest(
                @"http://mobss.jword.jp/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%82%ab%e3%83%83%e3%83%97%e3%82%b1%e3%83%bc%e3%82%ad%e3%81%a1%e3%82%83%e3%82%93&p=infoseek&pv=&t=web&sid=",
                @"アンパンマン カップケーキちゃん");
        }

        [TestMethod]
        public void PexJpSearchQuery()
        {
            SearchQueryTest(
                @"http://pex.jp/search?utf8=%e2%9c%93&keyword=%e3%82%a2%e3%83%b3%e3%82%b3%e3%83%a9&commit=%e6%a4%9c%e7%b4%a2",
                @"アンコラ");
            SearchQueryTest(
                @"http://pex.jp/search?utf8=%e2%9c%93&keyword=%e3%83%a1%e3%82%b3%e3%82%a4%e3%82%b9%e3%81%a4%e3%81%bc&commit=%e6%a4%9c%e7%b4%a2",
                @"メコイスつぼ");
        }

        [TestMethod]
        public void SearchAzbyFmworldNetSearchQuery()
        {
            SearchQueryTest(
                @"http://search.azby.fmworld.net/imagesearch/search?select=2&cflg=%e6%a4%9c%e7%b4%a2&q=%e3%81%bb%e3%81%9f%e3%82%8b%e5%a7%ab+%e3%81%bb%e3%81%9f%e3%82%8b%e7%8e%8b%e5%ad%90&ss=up",
                @"ほたる姫 ほたる王子");
            SearchQueryTest(
                @"http://search.azby.fmworld.net/imagesearch/search?select=2&cflg=%e6%a4%9c%e7%b4%a2&q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%a8%e3%82%b5%e3%83%b3%e3%82%bf%e3%81%95%e3%82%93%e3%81%b8%e3%81%ae%e6%89%8b%e7%b4%99&ss=up",
                @"アンパンマンとサンタさんへの手紙");
            SearchQueryTest(
                @"http://search.azby.fmworld.net/websearch/search?select=41&ss=up&cflg=%e6%a4%9c%e7%b4%a2&chartype=&Text=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%a8%e3%83%9d%e3%83%83%e3%83%9d%e3%81%a1%e3%82%83%e3%82%93",
                @"アンパンマンとポッポちゃん");
            SearchQueryTest(
                @"http://search.azby.fmworld.net/websearch/search?select=41&ss=up&cflg=%e6%a4%9c%e7%b4%a2&chartype=&Text=%e3%82%ab%e3%83%bc%e3%83%93%e3%82%a3%e3%81%ae%e3%83%9c%e3%82%b9%e3%81%9f%e3%81%a1",
                @"カービィのボスたち");
        }

        [TestMethod]
        public void SearchFenrirIncComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.fenrir-inc.com/?hl=ja&channel=sleipnir_3_w&safe=off&lr=all&q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%ef%bc%a4%ef%bc%a2",
                @"アンパンマンＤＢ");
            SearchQueryTest(
                @"http://search.fenrir-inc.com/?hl=ja&channel=sleipnir_3_w&safe=off&lr=all&q=%e3%81%8a%e3%82%84%e3%81%93%e3%81%a9%e3%82%93%e3%81%a1%e3%82%83%e3%82%93",
                @"おやこどんちゃん");
        }

        [TestMethod]
        public void SearchJiqooJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.jiqoo.jp/search?q=%e3%81%8a%e3%82%80%e3%81%99%e3%81%b3%e3%81%be%e3%82%93%e3%81%a8%e3%81%a9%e3%81%86%e3%81%8f%e3%81%a4%e3%81%8b%e3%81%84%e3%81%98%e3%82%93&act=&ienc=UTF-8&x=0&y=0",
                @"おむすびまんとどうくつかいじん");
            SearchQueryTest(
                @"http://search.jiqoo.jp/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%a8%e3%83%9d%e3%83%83%e3%83%9d%e3%81%a1%e3%82%83%e3%82%93&act=&ienc=UTF-8&x=0&y=0",
                @"アンパンマンとポッポちゃん");
        }

        [TestMethod]
        public void SearchKawaiiAswidgetComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.kawaii.aswidget.com/?ref=snoopy02&Keywords=%e3%83%91%e3%82%b9%e3%82%bf%e3%81%8a%e3%81%b0%e3%81%95%e3%82%93",
                @"パスタおばさん");
            SearchQueryTest(
                @"http://search.kawaii.aswidget.com/?Keywords=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%81%ae%e7%94%bb%e5%83%8f&lang=&page=3&ref=snoopy01",
                @"アンパンマンの画像");
        }

        [TestMethod]
        public void SearchNiftyComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.nifty.com/websearch/search?select=2&ss=nifty_top_tp&cflg=%e6%a4%9c%e7%b4%a2&q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b398+%e3%81%a1%e3%81%b3%e3%81%9e%e3%81%86%e3%81%8f%e3%82%93&otype=web_nifty_1",
                @"アンパンマン98 ちびぞうくん");
            SearchQueryTest(
                @"http://search.nifty.com/websearch/search?select=2&ss=up&cflg=%e6%a4%9c%e7%b4%a2&chartype=&Text=%e3%81%8f%e3%82%89%e3%82%84%e3%81%bf%e3%81%be%e3%82%93db",
                @"くらやみまんdb");
        }

        [TestMethod]
        public void SearchPlushomeAswidgetComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.plushome.aswidget.com/?Keywords=%e3%81%8a%e3%81%8f%e3%82%89%e3%81%a1%e3%82%83%e3%82%93%e7%94%bb%e5%83%8f&type=web&ref=android",
                @"おくらちゃん画像");
            SearchQueryTest(
                @"http://search.plushome.aswidget.com/?Keywords=%e3%83%90%e3%82%a4%e3%82%ad%e3%83%b3%e3%83%9e%e3%83%b3%e3%83%ad%e3%83%9c%e3%83%83%e3%83%88&type=web&ref=android",
                @"バイキンマンロボット");
        }

        [TestMethod]
        public void VBusterJwordJpSearchQuery()
        {
            SearchQueryTest(
                @"http://v-buster.jword.jp/search?q=%e3%81%bf%e3%81%aa%e3%81%bf%e3%81%ae%e5%b3%b6%e3%82%92%e3%81%99%e3%81%8f%e3%81%88&act=&ienc=UTF-8&x=0&y=0",
                @"みなみの島をすくえ");
            SearchQueryTest(
                @"http://v-buster.jword.jp/search?q=%e3%81%bf%e3%81%aa%e3%81%bf%e3%81%ae%e5%b3%b6%e3%82%92%e3%81%99%e3%81%8f%e3%81%88+%e5%8b%95%e7%94%bb&domain=",
                @"みなみの島をすくえ 動画");
        }

        [TestMethod]
        public void WebsearchExciteCoJpSearchQuery()
        {
            SearchQueryTest(
                @"http://websearch.excite.co.jp/?q=%e3%81%8a%e3%82%84%e3%81%93%e3%81%a9%e3%82%93%e3%81%a1%e3%82%83%e3%82%93&look=excite_jp&sstype=&search_suggest=",
                @"おやこどんちゃん");
            SearchQueryTest(
                @"http://websearch.excite.co.jp/?q=%e3%82%84%e3%81%8d%e3%81%9d%e3%81%b0%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3&charset=utf8&look=excite-iphone_jp&searchsubmit=%e6%a4%9c%e7%b4%a2",
                @"やきそばパンマン");
        }

        [TestMethod]
        public void SearchKinzaJpSearchQuery()
        {
            SearchQueryTest(
                @"https://search.kinza.jp/web/?q=%e3%83%89%e3%82%ad%e3%83%b3%e3%81%a1%e3%82%83%e3%82%93&fr=newtab&span=&language=",
                @"ドキンちゃん");
            SearchQueryTest(
                @"https://search.kinza.jp/web/?q=dokinnchann&fr=newtab",
                @"dokinnchann");
        }

        [TestMethod]
        public void SearchSearchnewsplusComSearchQuery()
        {
            SearchQueryTest(
                @"https://search.searchnewsplus.com/web?q=%e3%81%82%e3%82%93%e3%81%b1%e3%82%93%e3%81%be%e3%82%93 %e5%a4%8f%e3%81%be%e3%81%a4%e3%82%8a&secret=false&ref=url_input&search_count=4&view_count=2&theme=vivid_pink",
                @"あんぱんまん 夏まつり");
            SearchQueryTest(
                @"https://search.searchnewsplus.com/web?q=%e3%81%93%e3%81%8d%e3%82%93%e3%81%a1%e3%82%83%e3%82%93&secret=false&ref=url_input&theme=sky",
                @"こきんちゃん");
        }

        [TestMethod]
        public void JpHao123ComSearchQuery()
        {
            SearchQueryTest(
                @"http://jp.hao123.com/yahoo-search-demo-sample?query=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%81%e7%94%bb%e5%83%8f&pn=1",
                @"アンパンチ画像");
            SearchQueryTest(
                @"http://jp.hao123.com/yahoo-search-demo-sample?query=%e3%82%a4%e3%83%a9%e3%82%b9%e3%83%88%e3%83%90%e3%82%a4%e3%81%8d%e3%82%93%e3%81%be%e3%82%93%e3%83%90%e3%82%a4%e3%83%90%e3%82%a4%e3%82%ad%e3%83%bc%e3%83%b3&pn=1",
                @"イラストバイきんまんバイバイキーン");
        }

        [TestMethod]
        public void GwsCybozuNetSearchQuery()
        {
            SearchQueryTest(
                @"http://gws.cybozu.net/?Keywords=%e3%83%8a%e3%83%b3%e3%82%ab%e3%83%98%e3%83%b3%e3%83%80%e3%83%bc",
                @"ナンカヘンダー");
            SearchQueryTest(
                @"http://gws.cybozu.net/?Keywords=%e3%83%8f%e3%83%ad%e3%82%a6%e3%82%a3%e3%83%b3%e3%83%9e%e3%83%b3",
                @"ハロウィンマン");
        }

        [TestMethod]
        public void HomeKingsoftJpSearchQuery()
        {
            SearchQueryTest(
                @"http://home.kingsoft.jp/type/web?area=web-searchbox&page=1&keyword=%e3%82%a2%e3%82%af%e3%82%a2%e3%81%a1%e3%82%83%e3%82%93&area=web-searchbox&page=1",
                @"アクアちゃん");
            SearchQueryTest(
                @"http://home.kingsoft.jp/type/web?area=web-searchbox&page=1&keyword=%e3%82%b9%e3%83%bc%e3%83%91%e3%83%bc%e3%83%90%e3%82%a4%e3%82%ad%e3%83%b3%e3%81%9c%e3%82%93%e3%81%be%e3%81%84%e3%83%ad%e3%83%9c&area=web-searchbox&page=1",
                @"スーパーバイキンぜんまいロボ");
        }

        [TestMethod]
        public void SearchDolphinBrowserJpSearchQuery()
        {
            SearchQueryTest(
                @"http://search.dolphin-browser.jp/?q=%e3%82%af%e3%83%aa%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3 %e5%8b%95%e7%94%bb",
                @"クリアンパンマン 動画");
            SearchQueryTest(
                @"http://search.dolphin-browser.jp/?q=%e3%83%89%e3%83%a9%e3%82%b4%e3%83%b3%e3%82%af%e3%82%a8%e3%82%b9%e3%83%8810 %e3%83%89%e3%83%af%e3%83%bc%e3%83%95 %e7%99%bb%e5%a0%b4%e3%82%ad%e3%83%a3%e3%83%a9%e3%82%af%e3%82%bf%e3%83%bc",
                @"ドラゴンクエスト10 ドワーフ 登場キャラクター");
        }

        [TestMethod]
        public void SearchFoooooComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.fooooo.com/web/?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%81%82%e3%81%8f%e3%81%b3%e3%81%a9%e3%82%8a+%e3%82%ad%e3%83%a3%e3%83%a9%e3%82%af%e3%82%bf%e3%83%bc+%e3%82%a4%e3%83%a9%e3%82%b9%e3%83%88&fr=_default&span=&language=",
                @"アンパンマン あくびどり キャラクター イラスト");
            SearchQueryTest(
                @"http://search.fooooo.com/web/?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%82%ad%e3%83%a3%e3%83%a9%e3%82%af%e3%82%bf%e3%83%bc+%e3%83%94%e3%83%bc%e3%83%9e%e3%83%b3%e3%83%88%e3%83%aa%e3%82%aa+%e3%83%94%e3%83%bc%e3%83%9e%e3%83%b3&fr=_default&span=&language=",
                @"アンパンマン キャラクター ピーマントリオ ピーマン");
        }

        [TestMethod]
        public void SearchFreespotComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.freespot.com/search?q=%e3%81%8a%e3%81%97%e3%82%93%e3%81%93%e3%81%a1%e3%82%83%e3%82%93&type=web&ref=",
                @"おしんこちゃん");
            SearchQueryTest(
                @"http://search.freespot.com/search?q=%e3%81%93%e3%81%8a%e3%82%8a%e3%81%8a%e3%81%ab+%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3&type=web&ref=",
                @"こおりおに アンパンマン");
        }

        [TestMethod]
        public void DecomailerAwalkerJpSearchQuery()
        {
            SearchQueryTest(
                @"http://decomailer.awalker.jp/websearch.php?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%82%ad%e3%83%a3%e3%83%a9",
                @"アンパンマンキャラ");
            SearchQueryTest(
                @"http://decomailer.awalker.jp/websearch.php?q=%e3%82%af%e3%83%aa%e3%83%bc%e3%83%a0%e3%82%b7%e3%83%81%e3%83%a5%e3%83%bc%e3%81%8a%e3%81%b0%e3%81%95%e3%82%93+%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3",
                @"クリームシチューおばさん アンパンマン");
        }

        [TestMethod]
        public void SearchCravingComSearchQuery()
        {
            SearchQueryTest(
                @"http://search.crav-ing.com/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3+%e3%83%9d%e3%83%83%e3%83%9d%e3%83%81%e3%83%a3%e3%83%b3&mt=youtube&ienc=UTF-8&utm_source=ce1.x&utm_campaign=ce&utm_medium=explorer",
                @"アンパンマン ポッポチャン");
            SearchQueryTest(
                @"http://search.crav-ing.com/search?utm_source=ce1.x&utm_campaign=ce&ienc=UTF-8&utm_medium=searchbox&q=%e3%81%b0%e3%81%84%e3%81%8d%e3%82%93%e3%81%be%e3%82%93%e3%81%a8%e3%83%84%e3%83%9c%e3%83%9f%e3%81%a1%e3%82%83%e3%82%93",
                @"ばいきんまんとツボミちゃん");
        }

        [TestMethod]
        public void ImagesSearcBiglobeNeJpSearchQuery()
        {
            SearchQueryTest(
                @"http://images.search.biglobe.ne.jp/cgi-bin/search?start=320&q=%e3%83%8f%e3%83%b3%e3%83%90%e3%83%bc%e3%82%ac%e3%83%bc%e3%82%ad%e3%83%83%e3%83%89",
                @"ハンバーガーキッド");
            SearchQueryTest(
                @"http://images.search.biglobe.ne.jp/cgi-bin/search?q=%e3%82%a2%e3%83%b3%e3%83%91%e3%83%b3%e3%83%9e%e3%83%b3%e3%82%a2%e3%83%ad%e3%83%9e%e3%81%a1%e3%82%83%e3%82%93&searchBtn=%e6%a4%9c%e7%b4%a2&ie=utf8&o_sf=1&o_cr=all&o_si=&o_ft=all&o_sz=all",
                @"アンパンマンアロマちゃん");
        }
    }
}
