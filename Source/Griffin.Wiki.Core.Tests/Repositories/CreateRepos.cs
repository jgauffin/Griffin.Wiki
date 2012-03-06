using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
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
            var parser = new TextFormatAndWikiContentParser(new MarkdownParser(), new WikiParser());
            var svc = new PageService(repository);

            var page = svc.CreatePage(1, "SomePage2", "Some page 2", "Hwllo world!");

            //repository.Delete("SomePage2");

            session.Flush();
        }
    }
}
