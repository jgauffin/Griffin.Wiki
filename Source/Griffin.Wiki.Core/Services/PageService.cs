using System;
using System.Linq;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    [Component]
    public class PageService
    {
        private readonly IPageRepository _repository;
        private readonly IContentParser _parser;

        public PageService(IPageRepository repository, IContentParser parser)
        {
            _repository = repository;
            _parser = parser;
        }

        public void UpdatePage(string pageName, string title, string content)
        {
            var item = _repository.Get(pageName);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pageName));

            item.Title = title;
            var result = _parser.Parse(pageName, content);
            item.SetBody(result, _repository);
            _repository.Save(item);
        }


        public WikiPage CreatePage(string title, string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var page = _repository.Create(title, pageName);
            var result = _parser.Parse(pageName, contents);
            page.SetBody(result, _repository);
            _repository.Save(page);

            // Now fix all linking pages.
            var fixMissingLinks = _repository.GetMissingLinks(pageName).Select(x => x.Page);
            var linkingPages = _repository.GetPagesLinkingTo(pageName).Union(fixMissingLinks);
            foreach (var pageToFix in linkingPages)
            {
                var result2 = _parser.Parse(pageName, pageToFix.RawBody);
                pageToFix.UpdateLinks(result2, _repository);
            }

            _repository.RemoveMissingLinks(pageName);

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