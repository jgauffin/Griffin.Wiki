using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;

namespace Griffin.Wiki.Core.Tests.Services
{
    [TestClass]
    public class WikiParserTests
    {
        [TestMethod]
        public void ParseSingleNotFound()
        {
            var repos = new Mock<IPageRepository>();
            var parser = new WikiParser(repos.Object, "/root/");

            parser.Parse("Some html [[ALink]] with a link");

            Assert.AreEqual(
                @"Some html <a href=""/root/page/create/alink?title=ALink"" class=""missing"">ALink</a> with a link",
                parser.Content);
            Assert.AreEqual(@"alink", parser.PageLinks.First());
        }

        [TestMethod]
        public void ParseSingleNotFoundWithTitle()
        {
            var repos = new Mock<IPageRepository>();
            var parser = new WikiParser(repos.Object, "/root/");

            parser.Parse("Some html [[ALink|Some title]] with a link");

            Assert.AreEqual(
                @"Some html <a href=""/root/page/create/alink?title=Some%20title"" class=""missing"">Some title</a> with a link",
                parser.Content);
            Assert.AreEqual(@"alink", parser.PageLinks.First());
        }

        [TestMethod]
        public void ParseSingleFound()
        {
            var repos = new Mock<IPageRepository>();
            repos.Setup(k => k.Exists("alink")).Returns(true);
            var parser = new WikiParser(repos.Object, "/root/");

            parser.Parse("Some html [[ALink]] with a link");

            Assert.AreEqual(@"Some html <a href=""/root/page/view/alink"">ALink</a> with a link", parser.Content);
            Assert.AreEqual(@"alink", parser.PageLinks.First());
            repos.VerifyAll();
        }

        [TestMethod]
        public void ParseDoubleLink()
        {
            var repos = new Mock<IPageRepository>();
            repos.Setup(k => k.Exists(It.IsAny<string>())).Returns(true);
            var parser = new WikiParser(repos.Object, "/root/");

            parser.Parse("Some html [[ALink]][[SecondLink|Some Name]] with a link");

            Assert.AreEqual(
                @"Some html <a href=""/root/page/view/alink"">ALink</a><a href=""/root/page/view/secondlink"">Some Name</a> with a link",
                parser.Content);
            Assert.AreEqual(@"alink", parser.PageLinks.First());
            repos.VerifyAll();
        }

        [TestMethod]
        public void ParseSingleFoundWithTitle()
        {
            var repos = new Mock<IPageRepository>();
            repos.Setup(k => k.Exists("alink")).Returns(true);
            var parser = new WikiParser(repos.Object, "/root/");

            parser.Parse("Some html [[ALink|Some title]] with a link");

            Assert.AreEqual(@"Some html <a href=""/root/page/view/alink"">Some title</a> with a link", parser.Content);
            Assert.AreEqual(@"alink", parser.PageLinks.First());
            repos.VerifyAll();
        }
    }
}