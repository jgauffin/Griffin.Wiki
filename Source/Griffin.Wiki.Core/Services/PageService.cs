using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProjectPortal.Core.DomainModels;
using ProjectPortal.Core.Repositories;

namespace ProjectPortal.Core.Services
{
    public class PageService
    {
        private readonly PageRepository _repository;
        private readonly string _rootUri;

        public PageService(PageRepository repository, string rootUri)
        {
            _repository = repository;
            _rootUri = rootUri;
        }

        public void Save(string pageName, string contents)
        {
            var item = _repository.Get(pageName);
            if (item == null)
            {
                
            }
        }


        public void CreatePage(string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var markdownParser = new MarkdownParser();
            var html = markdownParser.Parse(contents);

            var parser = new WikiParser(_repository, _rootUri);
            parser.Parse(html);

            var page = new WikiPage(_repository);
            page.SetBody(contents, parser);
        }
    }

}
