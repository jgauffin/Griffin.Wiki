using System;
using System.Linq;
using System.Threading;
using Griffin.Logging;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Templates.Repositories;
using Sogeti.Pattern.DomainEvents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.Services
{
    [Component]
    public class PageService : IAutoSubscriberOf<EditApproved>
    {
        private readonly ILogger _logger = LogManager.GetLogger<PageService>();
        private readonly IPreProcessorService _preProcessorService;
        private readonly IPageRepository _repository;
        private readonly ITemplateRepository _templateRepository;

        public PageService(IPageRepository repository, IPreProcessorService preProcessorService,
                           ITemplateRepository templateRepository)
        {
            _repository = repository;
            _preProcessorService = preProcessorService;
            _templateRepository = templateRepository;
        }

        #region IAutoSubscriberOf<EditApproved> Members

        /// <summary>
        ///   Handle the domain event
        /// </summary>
        /// <param name="e"> Domain to process </param>
        public void Handle(EditApproved e)
        {
            var page = e.Revision.Page;
            var ctx = new PreProcessorContext(page, page.RawBody);
            _preProcessorService.Invoke(ctx);
            page.SetRevision(_repository, e.Revision, ctx);
            _repository.Save(page);
        }

        #endregion

        public void UpdatePage(PagePath pagePath, string title, string content, string comment)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (title == null) throw new ArgumentNullException("title");
            if (content == null) throw new ArgumentNullException("content");

            var item = _repository.Get(pagePath);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pagePath));

            item.Title = title;
            var ctx = new PreProcessorContext(item, content);
            _preProcessorService.Invoke(ctx);
            item.SetBody(ctx, comment, _repository);
            _repository.Save(item);
        }


        public WikiPage CreatePage(int parentId, PagePath pagePath, string title, string contents, int templateId)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (contents == null) throw new ArgumentNullException("contents");

            // contains path, should always be absolute path
            EnsurePath(pagePath);

            // EnsurePath has probably created the parent
            if (parentId < 1 && pagePath.ParentPath.ToString() != "/")
            {
                var parent = _repository.Get(pagePath.ParentPath);
                if (parent != null)
                    parentId = parent.Id;
            }

            _logger.Debug("{0} is creating a new page called {1}", Thread.CurrentPrincipal.Identity.Name, pagePath);
            var template = _templateRepository.Get(templateId);
            var page = _repository.Create(parentId, pagePath, title, template);
            var ctx = new PreProcessorContext(page, contents);
            _preProcessorService.Invoke(ctx);
            page.SetBody(ctx, "First revision", _repository);

            // Now fix all linking pages.
            var fixMissingLinks = _repository.GetMissingLinks(pagePath).Select(x => x.Page);
            var linkingPages = _repository.GetPagesLinkingTo(pagePath).Union(fixMissingLinks);
            foreach (var pageToFix in linkingPages)
            {
                _logger.Debug("Fixing link to {0} for {1}.", pagePath, pageToFix.PagePath);
                ctx = new PreProcessorContext(pageToFix, pageToFix.RawBody);
                _preProcessorService.Invoke(ctx);
                pageToFix.UpdateLinks(ctx, _repository);
            }

            _repository.RemoveMissingLinks(pagePath);

            return page;
        }

        private void EnsurePath(PagePath pagePath)
        {
            var pagePaths = pagePath.GetPathForParents().ToList();

            // check all pages but the last 
            for (var i = 0; i < pagePaths.Count; i++)
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
                var ctx = new PreProcessorContext(linkedPage, linkedPage.RawBody);
                _preProcessorService.Invoke(ctx);
                linkedPage.UpdateLinks(ctx, _repository);
            }
        }
    }
}