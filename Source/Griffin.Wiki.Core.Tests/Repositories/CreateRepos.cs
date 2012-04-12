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
using Sogeti.Pattern.InversionOfControl;
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
            ServiceResolver.Assign(new SomeResolver());

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

    public class SomeResolver : IServiceResolver
    {
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>
        /// Service if found; otherwise <c>null</c>.
        /// </returns>
        public T Resolve<T>() where T : class
        {
            return null;
        }

        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>
        /// Service if found; otherwise <c>null</c>.
        /// </returns>
        public object Resolve(Type type)
        {
            return null;
        }

        /// <summary>
        /// Resolve all services
        /// </summary>
        /// <typeparam name="T">Type that the services must implement</typeparam>
        /// <returns>
        /// A collection of services (or an empty collection)
        /// </returns>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return new List<T>();
        }

        /// <summary>
        /// Resolve all services
        /// </summary>
        /// <param name="type">Type that the services must implement</param>
        /// <returns>
        /// A collection of services (or an empty collection)
        /// </returns>
        public IEnumerable ResolveAll(Type type)
        {
            var listType = typeof (List<>).MakeGenericType(type);
            return (IEnumerable)Activator.CreateInstance(listType);
        }
    }
}
