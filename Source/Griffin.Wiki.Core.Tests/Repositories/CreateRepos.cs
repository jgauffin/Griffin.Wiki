using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Repositories;
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
            var repository = new PageRepository(SessionFactory.Create());
            var page = repository.Create(1, "SomePage2", "Some page 2");

            repository.Delete("SomePage2");
        }
    }
}
