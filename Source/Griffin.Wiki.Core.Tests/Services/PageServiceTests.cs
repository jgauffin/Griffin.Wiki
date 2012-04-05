﻿using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Pages.Services;
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
            var treeService = new Mock<IPageLinkGenerator>();
            var wikiparser = new WikiParser(treeService.Object);
            var contentParser = new TextFormatAndWikiContentParser(new MarkdownParser(), wikiparser);
            var service = new PageService(repos.Object, contentParser, null);

        }
    }
}