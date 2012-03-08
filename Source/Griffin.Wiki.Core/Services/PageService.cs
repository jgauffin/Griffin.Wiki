using System;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    [Component]
    public class PageService
    {
        private readonly IPageRepository _repository;
        private readonly IWikiParser _parser;

        public PageService(IPageRepository repository, IWikiParser parser)
        {
            _repository = repository;
            _parser = parser;
        }

        public void UpdatePage(int changer, string pageName, string title, string content)
        {
            var item = _repository.Get(pageName);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pageName));

            item.Title = title;
            var result = _parser.Parse(pageName, content);
            item.SetBody(changer, result, _repository);
            _repository.Save(item);
        }


        public WikiPage CreatePage(int creator, string title, string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var page = _repository.Create(creator, title, pageName);
            var result = _parser.Parse(pageName, contents);
            page.SetBody(creator, result, _repository);
            _repository.Save(page);

            // Now fix all linking pages.
            var linkingPages = _repository.GetPagesLinkingTo(pageName);
            foreach (var pageToFix in linkingPages)
            {
                var result2 = _parser.Parse(pageName, pageToFix.RawBody);
                pageToFix.UpdateLinks(result2, _repository);
            }

            return page;
        }

        public void DeletePage(string pageName)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");

            var linkingPages = _repository.GetPagesLinkingTo(pageName);
            _repository.Delete(pageName);

            foreach (var linkedPage in linkingPages)
            {
                var result = _parser.Parse(linkedPage.PageName, linkedPage.RawBody);
                linkedPage.UpdateLinks(result, _repository);
            }
        }
    }
}