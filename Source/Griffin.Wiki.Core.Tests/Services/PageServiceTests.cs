using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Moq;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Services
{
    public class PageServiceTests
    {
        [Fact]
        public void Headings()
        {
            var repos = new Mock<IPageRepository>();
            var service = new PageService(repos.Object);

        }
    }
}