using System.Collections.Generic;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Templates.DomainModels;
using Griffin.Wiki.Core.Templates.Repositories;
using NHibernate;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.NHibernate.Repositories
{
    [Component]
    public class TemplateRepository : ITemplateRepository
    {
        private readonly IContentParser _parser;
        private readonly ISession _session;

        public TemplateRepository(ISession session, IContentParser parser)
        {
            _session = session;
            _parser = parser;
        }

        public PageTemplate Create(string title, string contents)
        {
            var template = new PageTemplate(title, contents);
            _session.Save(template);
            return template;
        }

        public IEnumerable<PageTemplate> Find()
        {
            return _session.QueryOver<PageTemplate>().List();
        }

        public PageTemplate Get(int templateId)
        {
            return _session.Get<PageTemplate>(templateId);
        }
    }
}