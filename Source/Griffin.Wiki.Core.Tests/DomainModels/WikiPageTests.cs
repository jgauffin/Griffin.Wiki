using Moq;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Xunit;

namespace Griffin.Wiki.Core.Tests.DomainModels
{
    public class WikiPageTests
    {
        [Fact]
        public void TestMethod1()
        {
            var repos = new Mock<IPageRepository>();
            var page = new WikiPage(null, "MyTitle", "PageName", null);
            page.SetBody(null, repos.Object);
        }
    }
}