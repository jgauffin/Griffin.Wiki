using System;
using System.Linq;
using System.Threading;
using Griffin.Logging;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Templates.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    [Component]
    public class PageService
    {
        private readonly ILogger _logger = LogManager.GetLogger<PageService>();
        private readonly IContentParser _parser;
        private readonly IPageRepository _repository;
        private readonly ITemplateRepository _templateRepository;

        public PageService(IPageRepository repository, IContentParser parser, ITemplateRepository templateRepository)
        {
            _repository = repository;
            _parser = parser;
            _templateRepository = templateRepository;
        }

        public void UpdatePage(string pageName, string title, string content, string comment)
        {
            var item = _repository.Get(pageName);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pageName));

            item.Title = title;
            var result = _parser.Parse(pageName, content);
            item.SetBody(result, comment, _repository);
            _repository.Save(item);
        }


        public WikiPage CreatePage(int parentId, string title, string pageName, string contents, int templateId)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            _logger.Debug("{0} is creating a new page called {1}", Thread.CurrentPrincipal.Identity.Name, pageName);
            var template = _templateRepository.Get(templateId);
            var page = _repository.Create(parentId, title, pageName, template);
            var result = _parser.Parse(pageName, contents);
            page.SetBody(result, "First revision", _repository);

            // Now fix all linking pages.
            var fixMissingLinks = _repository.GetMissingLinks(pageName).Select(x => x.Page);
            var linkingPages = _repository.GetPagesLinkingTo(pageName).Union(fixMissingLinks);
            foreach (var pageToFix in linkingPages)
            {
                _logger.Debug("Fixing link to {0} for {1}.", pageName, pageToFix.PageName);
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