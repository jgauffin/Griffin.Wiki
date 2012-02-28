using System;
using Griffin.Wiki.Core.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    [Component]
    public class PageService
    {
        private readonly IPageRepository _repository;

        public PageService(IPageRepository repository)
        {
            _repository = repository;
        }

        public void UpdatePage(int changer, string pageName, string title, string content)
        {
            var item = _repository.Get(pageName);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pageName));

            item.Title = title;
            item.SetBody(changer, content);
            _repository.Save(item);
        }


        public void CreatePage(int creator, string title, string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var page = _repository.Create(creator, title, pageName);
            page.SetBody(creator, contents);
            _repository.Save(page);

            // Now fix all linking pages.
            var linkingPages = _repository.GetLinkingPages(pageName);
            foreach (var linkedPageName in linkingPages)
            {
                var linkedPage = _repository.Get(linkedPageName);
                linkedPage.UpdateLinks();
            }
        }

        public void DeletePage(string pageName)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");

            var linkingPages = _repository.GetLinkingPages(pageName);
            _repository.Delete(pageName);

            foreach (var linkedPageName in linkingPages)
            {
                var linkedPage = _repository.Get(linkedPageName);
                linkedPage.UpdateLinks();
            }
        }
    }
}