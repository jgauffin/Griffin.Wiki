using System;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Services
{
    [Component]
    public class PageService
    {
        private readonly ITextFormatParser _textFormatParser;
        private readonly IPageRepository _repository;
        private readonly PageServiceConfiguration _configuration;

        public PageService(ITextFormatParser textFormatParser, IPageRepository repository, PageServiceConfiguration configuration)
        {
            _textFormatParser = textFormatParser;
            _repository = repository;
            _configuration = configuration;
        }

        public void Save(string pageName, string contents)
        {
            var item = _repository.Get(pageName);
            if (item == null)
            {
            }
        }


        public void CreatePage(string name, string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var html = _textFormatParser.Parse(contents);

            var parser = new WikiParser(_repository, _configuration.RootUri);
            parser.Parse(html);

            var page = new WikiPage(_repository);
            page.SetBody(contents, parser);
        }
    }
}