using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MifuminLib.AccessAnalyzer;

namespace AccessAnalyzerTests
{
    [TestClass]
    public class LogReaderTest
    {
        [TestMethod()]
        [DeploymentItem(@"LogReaderTest.log")]
        public void AccessLogTest()
        {
            var accessLog = new AccessLog();
            accessLog.Read(new[] { "LogReaderTest.log" });
            var logs = accessLog.GetLogs();
            Assert.AreEqual(3, logs.Length);

            Assert.AreEqual(@"msnbot-40-77-167-229.search.msn.com", logs[0].Host);
            Assert.AreEqual(@"-", logs[0].RemoteLog);
            Assert.AreEqual(@"-", logs[0].User);
            Assert.AreEqual(@"2023/02/05 00:00:14", logs[0].Date);
            Assert.AreEqual(Log.EMethod.GET, logs[0].Method);
            Assert.AreEqual(@"/dq10/%e3%83%80%e3%83%bc%e3%82%af%e3%82%a6%e3%82%a4%e3%83%b3%e3%82%b0%e3%83%97%e3%83%aa%e3%82%ba%e3%83%a0", logs[0].Requested); // URLエンコード
            Assert.AreEqual("HTTP 2.0", logs[0].HTTP);
            Assert.AreEqual(200, logs[0].Status);
            Assert.AreEqual(1511, logs[0].SendSize);
            Assert.AreEqual(@"-", logs[0].Referer);
            Assert.AreEqual(@"Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm) Chrome/103.0.5060.134 Safari/537.36", logs[0].UserAgent);

            Assert.AreEqual(@"static.76.68.76.144.clients.your-server.de", logs[1].Host);
            Assert.AreEqual(@"-", logs[1].RemoteLog);
            Assert.AreEqual(@"-", logs[1].User);
            Assert.AreEqual(@"2023/02/05 00:00:16", logs[1].Date);
            Assert.AreEqual(Log.EMethod.GET, logs[1].Method);
            Assert.AreEqual(@"/anpandb/chara-796", logs[1].Requested);
            Assert.AreEqual("HTTP 1.1", logs[1].HTTP);
            Assert.AreEqual(200, logs[1].Status);
            Assert.AreEqual(3628, logs[1].SendSize);
            Assert.AreEqual(@"-", logs[1].Referer);
            Assert.AreEqual(@"serpstatbot/2.1 (advanced backlink tracking bot; https://serpstatbot.com/; abuse@serpstatbot.com)", logs[1].UserAgent);

            Assert.AreEqual(@"127.0.0.1.local", logs[2].Host);
            Assert.AreEqual(@"-", logs[2].RemoteLog);
            Assert.AreEqual(@"-", logs[2].User);
            Assert.AreEqual(@"2023/02/05 18:13:16", logs[2].Date);
            Assert.AreEqual(Log.EMethod.GET, logs[2].Method);
            Assert.AreEqual(@"/anpandb/tv-201a", logs[2].Requested);
            Assert.AreEqual("HTTP 2.0", logs[2].HTTP);
            Assert.AreEqual(200, logs[2].Status);
            Assert.AreEqual(3299, logs[2].SendSize);
            Assert.AreEqual(@"https://tgws.plus/anpandb/?search=%e3%81%82%e3%81%8b%e3%81%a1%e3%82%83%e3%82%93%e3%81%be%e3%82%93&l=anime&sort=", logs[2].Referer);
            Assert.AreEqual(@"Mozilla/5.0 (Linux; Android 8.1.0; CPH1903 Build/OPM1.171019.026; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/109.0.5414.118 Mobile Safari/537.36 YJApp-ANDROID jp.co.yahoo.android.yjtop/3.136.0", logs[2].UserAgent);
        }

        [TestMethod()]
        [DeploymentItem(@"LogReaderTest.log")]
        public void EnumerateAsCombinedTest()
        {
            using (var logReader = new LogReader("LogReaderTest.log"))
            {
                var logs = logReader.EnumerateAsCombined().ToArray();
                Assert.AreEqual(3, logs.Length);

                Assert.AreEqual(@"msnbot-40-77-167-229.search.msn.com", logs[0].Host);
                Assert.AreEqual(@"-", logs[0].RemoteLog);
                Assert.AreEqual(@"-", logs[0].User);
                Assert.AreEqual(@"2023/02/05 00:00:14", logs[0].Date);
                Assert.AreEqual(Log.EMethod.GET, logs[0].Method);
                Assert.AreEqual(@"/dq10/ダークウイングプリズム", logs[0].Requested); // 新方式ではエンコードしないでおこう。
                Assert.AreEqual("HTTP 2.0", logs[0].HTTP);
                Assert.AreEqual(200, logs[0].Status);
                Assert.AreEqual(1511, logs[0].SendSize);
                Assert.AreEqual(@"-", logs[0].Referer);
                Assert.AreEqual(@"Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm) Chrome/103.0.5060.134 Safari/537.36", logs[0].UserAgent);

                Assert.AreEqual(@"static.76.68.76.144.clients.your-server.de", logs[1].Host);
                Assert.AreEqual(@"-", logs[1].RemoteLog);
                Assert.AreEqual(@"-", logs[1].User);
                Assert.AreEqual(@"2023/02/05 00:00:16", logs[1].Date);
                Assert.AreEqual(Log.EMethod.GET, logs[1].Method);
                Assert.AreEqual(@"/anpandb/chara-796", logs[1].Requested);
                Assert.AreEqual("HTTP 1.1", logs[1].HTTP);
                Assert.AreEqual(200, logs[1].Status);
                Assert.AreEqual(3628, logs[1].SendSize);
                Assert.AreEqual(@"-", logs[1].Referer);
                Assert.AreEqual(@"serpstatbot/2.1 (advanced backlink tracking bot; https://serpstatbot.com/; abuse@serpstatbot.com)", logs[1].UserAgent);

                Assert.AreEqual(@"127.0.0.1.local", logs[2].Host);
                Assert.AreEqual(@"-", logs[2].RemoteLog);
                Assert.AreEqual(@"-", logs[2].User);
                Assert.AreEqual(@"2023/02/05 18:13:16", logs[2].Date);
                Assert.AreEqual(Log.EMethod.GET, logs[2].Method);
                Assert.AreEqual(@"/anpandb/tv-201a", logs[2].Requested);
                Assert.AreEqual("HTTP 2.0", logs[2].HTTP);
                Assert.AreEqual(200, logs[2].Status);
                Assert.AreEqual(3299, logs[2].SendSize);
                Assert.AreEqual(@"https://tgws.plus/anpandb/?search=あかちゃんまん&l=anime&sort=", logs[2].Referer); // 新方式ではエンコードしないでおこう。
                Assert.AreEqual(@"Mozilla/5.0 (Linux; Android 8.1.0; CPH1903 Build/OPM1.171019.026; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/109.0.5414.118 Mobile Safari/537.36 YJApp-ANDROID jp.co.yahoo.android.yjtop/3.136.0", logs[2].UserAgent);
            }
        }
    }
}
