using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MifuminLib.AccessAnalyzer;

namespace AccessAnalyzerTests
{
    [TestClass]
    public class RefererAnalyzerTest
    {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
        RefererAnalyzer _refererAnalyzer = new RefererAnalyzer();
#pragma warning restore CS0618 // 型またはメンバーが旧型式です

        public TestContext TestContext { get; set; }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "search.csv", "search#csv", DataAccessMethod.Sequential), DeploymentItem("AccessAnalyzerTests\\search.csv")]
        [TestMethod()]
        public void GetSearchQueryTest()
        {
            var urlString = TestContext.DataRow.Field<string>("input");
            var expectedQuery = TestContext.DataRow.Field<string>("output");
            if (expectedQuery == "")
            {
                expectedQuery = null;
            }
            Assert.AreEqual(expectedQuery, _refererAnalyzer.GetSearchQuery(urlString), urlString);
        }
    }
}
