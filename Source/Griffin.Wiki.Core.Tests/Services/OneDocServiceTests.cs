using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Griffin.Container;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.PostLoadProcessors;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Services;
using Griffin.Wiki.Core.SiteMaps.Services;
using Griffin.Wiki.Core.Tests.Repositories;
using Griffin.Wiki.Core.Users.DomainModels;
using Moq;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Services
{

    public class OneDocServiceTests
    {

        [Fact]
        public void Generate()
        {
            var session = SessionFactory.Create();
            var repos = new PageTreeRepository(session);
            var pageRepos = new PageRepository(session);
            var locator = new Mock<IServiceLocator>();
            locator.Setup(x => x.ResolveAll<IPostLoadProcessor>()).Returns(new IPostLoadProcessor[] { new ChildPageSection(pageRepos) });

            locator.Setup(x => x.ResolveAll<ITextProcessor>()).Returns(new ITextProcessor[]
                                                                           {
                                                                               new MarkdownParser(),
                                                                               new WikiLinkProcessor(repos)
                                                                           });
            locator.Setup(x => x.ResolveAll<IHtmlProcessor>()).Returns(new IHtmlProcessor[] { new HeadingProcessor() });
            var pre = new PreProcessorService(locator.Object);


            var service = new OneDocService(repos, pre, new ImageRepository(session), new PostLoadProcessService(locator.Object));
            service.GenerateHTML("C:\\temp\\html\\working\\",
                                 new StreamWriter(new FileStream("C:\\temp\\html\\wiki.html", FileMode.Create)));
        }

        [Fact]
        public void ReGeneratePages()
        {
            var session = SessionFactory.Create();
            var repos = new PageTreeRepository(session);
            var pageRepos = new PageRepository(session);
            var locator = new Mock<IServiceLocator>();
            locator.Setup(x => x.ResolveAll<IPostLoadProcessor>()).Returns(new IPostLoadProcessor[] { new ChildPageSection(pageRepos) });

            locator.Setup(x => x.ResolveAll<ITextProcessor>()).Returns(new ITextProcessor[]
                                                                           {
                                                                               new MarkdownParser(),
                                                                               new WikiLinkProcessor(repos)
                                                                           });
            locator.Setup(x => x.ResolveAll<IHtmlProcessor>()).Returns(new IHtmlProcessor[] { new HeadingProcessor() });
            var pre = new PreProcessorService(locator.Object);

            var user = new UserRepository(session).GetOrCreate("BA84194", "Jonas Gauffin");
            var myIdentity = new WikiIdentity(user);
            Thread.CurrentPrincipal = new WikiPrinicpal(myIdentity);

            using (var transaction = session.BeginTransaction())
            {
                foreach (var page in pageRepos.FindAll())
                {
                    var ctx = new PreProcessorContext(page, page.RawBody);
                    pre.Invoke(ctx);
                    page.SetBody(ctx, "Changed to relative links", pageRepos);
                }

                transaction.Commit();
            }
        }
    }
}
