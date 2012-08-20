using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Services;
using Moq;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Repositories
{
    /// <summary>
    /// Integration tests
    /// </summary>
    public class CreateRepos
    {
        [Fact]
        public void TestMethod1()
        {
            var session = SessionFactory.Create();
            var repository = new PageRepository(session);

            //var wikiParser = new WikiParser(new Mock<IPageLinkGenerator>().Object);
            //var parser = new TextFormatAndWikiContentParser(new MarkdownParser(), wikiParser);
            //var svc = new PageService(repository, parser, null);

            //var page = svc.CreatePage(0, new PagePath("/somepage/"), "Some page 2", "Hwllo world!", 0);

            //repository.Delete("SomePage2");

            session.Flush();
        }
    }

}
