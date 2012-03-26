using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Griffin.Logging;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Templates.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.Services
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

        public void UpdatePage(PagePath pagePath, string title, string content, string comment)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (title == null) throw new ArgumentNullException("title");
            if (content == null) throw new ArgumentNullException("content");

            var item = _repository.Get(pagePath);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pagePath));

            item.Title = title;
            var result = _parser.Parse(pagePath, content);
            item.SetBody(result, comment, _repository);
            _repository.Save(item);
        }


        public WikiPage CreatePage(int parentId, PagePath pagePath, string title, string contents, int templateId)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (contents == null) throw new ArgumentNullException("contents");

            // contains path, should always be absolute path
            EnsurePath(pagePath);

            _logger.Debug("{0} is creating a new page called {1}", Thread.CurrentPrincipal.Identity.Name, pagePath);
            var template = _templateRepository.Get(templateId);
            var page = _repository.Create(parentId, pagePath, title, template);
            var result = _parser.Parse(pagePath, contents);
            page.SetBody(result, "First revision", _repository);

            // Now fix all linking pages.
            var fixMissingLinks = _repository.GetMissingLinks(pagePath).Select(x => x.Page);
            var linkingPages = _repository.GetPagesLinkingTo(pagePath).Union(fixMissingLinks);
            foreach (var pageToFix in linkingPages)
            {
                _logger.Debug("Fixing link to {0} for {1}.", pagePath, pageToFix.PagePath);
                var result2 = _parser.Parse(pageToFix.PagePath, pageToFix.RawBody);
                pageToFix.UpdateLinks(result2, _repository);
            }

            _repository.RemoveMissingLinks(pagePath);

            return page;
        }

        private void EnsurePath(PagePath pagePath)
        {
            var pagePaths = pagePath.GetPaths().ToList();

            // check all pages but the last 
            for (int i = 0; i < pagePaths.Count -1; i++)
            {
                var path = pagePaths[i];
                if (!_repository.Exists(path))
                    CreatePage(0, path, path.ToString(), "# Child pages\r\n\r\n[:child-pages].", 0);
            }
        }

        public void DeletePage(PagePath pagePath)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");

            var linkingPages = _repository.GetPagesLinkingTo(pagePath);
            _repository.Delete(pagePath);

            foreach (var linkedPage in linkingPages)
            {
                var result = _parser.Parse(linkedPage.PagePath, linkedPage.RawBody);
                linkedPage.UpdateLinks(result, _repository);
            }
        }
    }
}