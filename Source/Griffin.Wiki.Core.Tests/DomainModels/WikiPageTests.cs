using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;

namespace Griffin.Wiki.Core.Tests.DomainModels
{
    [TestClass]
    public class WikiPageTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var repos = new Mock<IPageRepository>();
            var page = new WikiPage(repos.Object);
            page.SetBody("/root", null);
        }
    }
}