using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Services;
using NHibernate;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Repositories
{
    [Component]
    public class TemplateRepository
    {
        private readonly ISession _session;
        private readonly IContentParser _parser;

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
    }
}
