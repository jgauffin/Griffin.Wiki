using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.Services;
using Griffin.Wiki.Core.Tests.Repositories;
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
            var service = new OneDocService(repos, new MarkdownParser(), new ImageRepository(session));
            var sb = service.Generate();
            File.WriteAllText("C:\\temp\\html\\wiki.html", sb);
        }
    }
}
