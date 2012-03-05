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
            var page = new WikiPage(repos.Object, 1, "MyTitle", "PageName");
            page.SetBody(1, null);
        }
    }
}