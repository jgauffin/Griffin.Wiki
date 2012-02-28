using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Griffin.Wiki.Core.Tests.Services
{
    [TestClass]
    public class PageServiceTests
    {
        [TestMethod]
        public void Headings()
        {
            var repos = new Mock<IPageRepository>();
            var service = new PageService(repos.Object);

        }
    }
}