using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Container.DomainEvents;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Repositories;

namespace Griffin.Wiki.Core.Pages.Handlers
{
    /// <summary>
    /// Pre processes edits and saves a new revision.
    /// </summary>
    public class EditApprovedHandler : ISubscriberOf<EditApproved>
    {
        private IPreProcessorService _preProcessorService;
        private IPageRepository _repository;

        public EditApprovedHandler(IPreProcessorService preProcessorService, IPageRepository repository)
        {
            _preProcessorService = preProcessorService;
            _repository = repository;
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">The event</param>
        public void Handle(EditApproved e)
        {
            var page = e.Revision.Page;
            var ctx = new PreProcessorContext(page, page.RawBody);
            _preProcessorService.Invoke(ctx);
            page.SetRevision(_repository, e.Revision, ctx);
            _repository.Save(page);
        }
    }
}
