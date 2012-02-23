using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjectPortal.Core.DomainModels;
using ProjectPortal.Core.Repositories;

namespace ProjectPortal.Core.Tests.DomainModels
{
    [TestClass]
    public class WikiPageTests
    {
       
        [TestMethod]
        public void TestMethod1()
        {
            var repos = new Mock<IPageRepository>();
            WikiPage page = new WikiPage(repos.Object);
            page.SetBody("/root", null);
        }
    }
}
