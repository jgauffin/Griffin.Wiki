﻿using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.Repositories;
using Moq;
using Xunit;

namespace Griffin.Wiki.Core.Tests.DomainModels
{
    public class WikiPageTests
    {
        [Fact]
        public void TestMethod1()
        {
            var repos = new Mock<IPageRepository>();
            var page = new WikiPage(null, new PagePath("MyTitle"), "PageName", null);
            page.SetBody(null, "Some comment", repos.Object);
        }
    }
}