using System.Linq;
using Moq;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Services
{
    public class WikiParserTests
    {
        private Mock<IPageRepository> _repos;
        private WikiParser _parser;

        public WikiParserTests()
        {
            _repos = new Mock<IPageRepository>();
            _parser = new WikiParser(_repos.Object, new WikiParserConfiguration {RootUri = "/root/"});

        }
        [Fact]
        public void ParseSingleNotFound()
        {
            _parser.Parse("home", "Some html [[ALink]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/create/alink?title=ALink&parentName=home"" class=""missing"">ALink</a> with a link",
                _parser.Content);
            Assert.Equal(@"alink", _parser.PageLinks.First());
        }

        [Fact]
        public void ParseSingleNotFoundWithTitle()
        {
            _parser.Parse("home", "Some html [[ALink|Some title]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/create/alink?title=Some%20title&parentName=home"" class=""missing"">Some title</a> with a link",
                _parser.Content);
            Assert.Equal(@"alink", _parser.PageLinks.First());
        }

        [Fact]
        public void ParseSingleFound()
        {
            _repos.Setup(k => k.Exists("alink")).Returns(true);

            _parser.Parse("home", "Some html [[ALink]] with a link");

            Assert.Equal(@"Some html <a href=""/root/page/show/alink"">ALink</a> with a link", _parser.Content);
            Assert.Equal(@"alink", _parser.PageLinks.First());
            _repos.VerifyAll();
        }

        [Fact]
        public void ParseDoubleLink()
        {
            _repos.Setup(k => k.Exists(It.IsAny<string>())).Returns(true);

            _parser.Parse("home", "Some html [[ALink]][[SecondLink|Some Name]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/show/alink"">ALink</a><a href=""/root/page/show/secondlink"">Some Name</a> with a link",
                _parser.Content);
            Assert.Equal(@"alink", _parser.PageLinks.First());
            _repos.VerifyAll();
        }

        [Fact]
        public void ParseSingleFoundWithTitle()
        {
            _repos.Setup(k => k.Exists("alink")).Returns(true);

            _parser.Parse("home", "Some html [[ALink|Some title]] with a link");

            Assert.Equal(@"Some html <a href=""/root/page/show/alink"">Some title</a> with a link", _parser.Content);
            Assert.Equal(@"alink", _parser.PageLinks.First());
            _repos.VerifyAll();
        }

        [Fact]
        public void AdjustHeadings()
        {
            _parser.Parse("home", "Some html <h1>A heading!</h1>klsdfsdkjlsdklds");

            Assert.Equal(@"Some html <a name=""Aheading"" id=""Aheading""></a><h1>A heading!</h1>klsdfsdkjlsdklds", _parser.Content);
        }
    }
}