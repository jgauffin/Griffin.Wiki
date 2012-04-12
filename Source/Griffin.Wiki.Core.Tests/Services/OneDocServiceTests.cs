using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.PostLoadProcessors;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Services;
using Griffin.Wiki.Core.SiteMaps.Services;
using Griffin.Wiki.Core.Tests.Repositories;
using Griffin.Wiki.Core.Users.DomainModels;
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
            var pre = new PreProcessorService(
                new ITextProcessor[] {new MarkdownParser(), new WikiLinkProcessor(repos)},
                new IHtmlProcessor[] {new HeadingProcessor()});
            var post =
                new PostLoadProcessService(new IPostLoadProcessor[]
                                               {
                                                   new ChildPageSection(pageRepos)
                                               });

            var service = new OneDocService(repos, pre, new ImageRepository(session), post);
            var sb = service.Generate();
            File.WriteAllText("C:\\temp\\html\\wiki.html", sb);
        }

        [Fact]
        public void ReGeneratePages()
        {
            var session = SessionFactory.Create();
            var repos = new PageTreeRepository(session);
            var pageRepos = new PageRepository(session);
            var pre = new PreProcessorService(
                new ITextProcessor[] { new MarkdownParser(), new WikiLinkProcessor(repos) },
                new IHtmlProcessor[] { new HeadingProcessor() });

            var user = new UserRepository(session).GetOrCreate("BA84194");
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
