using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.SiteMaps.Services;
using Moq;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Services
{
    /*
    public class WikiParserTests
    {
        private readonly Mock<IPageLinkGenerator> _linkGenerator;
        private readonly WikiParser _parser;

        public WikiParserTests()
        {
            _linkGenerator = new Mock<IPageLinkGenerator>();
            _parser = new WikiParser(_linkGenerator.Object);
        }

        [Fact]
        public void ParseSingleNotFound()
        {
            _linkGenerator.Setup(
                k => k.CreateLinks(new PagePath("/home/"), It.Is<IEnumerable<WikiLink>>(x => x.First().PagePath == new PagePath("/ALink/")))).Returns(
                    new List<HtmlLink>
                        {
                            new HtmlLink("ALink", "",
                                         @"<a href=""/root/page/create/alink?title=ALink&parentName=home"" class=""missing"">ALink</a>")
                        });
            var result = _parser.Parse("home", "Some html [[ALink]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/create/alink?title=ALink&parentName=home"" class=""missing"">ALink</a> with a link",
                result.HtmlBody);
            Assert.Equal(@"alink", result.PageLinks.First());
        }

        [Fact]
        public void ParseSingleNotFoundWithTitle()
        {
            _linkGenerator.Setup(
                k => k.CreateLinks("home", It.Is<IEnumerable<WikiLink>>(x => x.First().PageName == "ALink"))).Returns(
                    new List<HtmlLink>
                        {
                            new HtmlLink("ALink", "",
                                         @"<a href=""/root/page/create/alink?title=Some%20title&parentName=home"" class=""missing"">Some title</a>")
                        });

            var result = _parser.Parse("home", "Some html [[ALink|Some title]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/create/alink?title=Some%20title&parentName=home"" class=""missing"">Some title</a> with a link",
                result.HtmlBody);
            Assert.Equal(@"alink", result.PageLinks.First());
        }

        [Fact]
        public void ParseSingleFound()
        {
            _linkGenerator.Setup(
                k => k.CreateLinks("home", It.Is<IEnumerable<WikiLink>>(x => x.First().PageName == "ALink"))).Returns(
                    new List<HtmlLink>
                        {
                            new HtmlLink("ALink", "",
                                         @"<a href=""/root/page/show/alink"">ALink</a>")
                        });

            var result = _parser.Parse("home", "Some html [[ALink]] with a link");

            Assert.Equal(@"Some html <a href=""/root/page/show/alink"">ALink</a> with a link", result.HtmlBody);
            Assert.Equal(@"alink", result.PageLinks.First());
            _linkGenerator.VerifyAll();
        }

        [Fact]
        public void ParseDoubleLink()
        {
            _linkGenerator.Setup(
                k => k.CreateLinks("home", It.Is<IEnumerable<WikiLink>>(x => x.Last().PageName == "SecondLink"))).
                Returns(new[]
                            {
                                new HtmlLink("ALink", "", @"<a href=""/root/page/show/alink"">ALink</a>"),
                                new HtmlLink("SecondLink", "", @"<a href=""/root/page/show/secondlink"">Some Name</a>"),
                            });

            var result = _parser.Parse("home", "Some html [[ALink]][[SecondLink|Some Name]] with a link");

            Assert.Equal(
                @"Some html <a href=""/root/page/show/alink"">ALink</a><a href=""/root/page/show/secondlink"">Some Name</a> with a link",
                result.HtmlBody);
            Assert.Equal(@"alink", result.PageLinks.First());
            _linkGenerator.VerifyAll();
        }

        [Fact]
        public void ParseSingleFoundWithTitle()
        {
            _linkGenerator.Setup(x => x.CreateLinks("home",
                                                    It.IsAny<IEnumerable<WikiLink>>())
                ).Returns(new[]
                              {
                                  new HtmlLink("ALink", "", @"<a href=""/root/page/show/alink"">Some title</a>")
                                  ,
                              });

            var result = _parser.Parse("home", "Some html [[ALink|Some title]] with a link");

            Assert.Equal(@"Some html <a href=""/root/page/show/alink"">Some title</a> with a link", result.HtmlBody);
            Assert.Equal(@"alink", result.PageLinks.First());
            _linkGenerator.VerifyAll();
        }


        [Fact]
        public void AdjustHeadings()
        {
            var result = _parser.Parse("home", "Some html <h1>A heading!</h1>klsdfsdkjlsdklds");

            Assert.Equal(@"Some html <h1 id=""Aheading"">A heading!</h1>klsdfsdkjlsdklds",
                         result.HtmlBody);
        }
    }*/
}