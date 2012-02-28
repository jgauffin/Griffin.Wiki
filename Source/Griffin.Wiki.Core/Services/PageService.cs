using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public void UpdatePage(int changer, string pageName, string title, string content)
        {
            var item = _repository.Get(pageName);
            if (item == null)
                throw new InvalidOperationException(string.Format("Page '{0}' was not found.", pageName));

            item.Title = title;
            item.SetBody(changer, content, ParseBody(content));
            _repository.Save(item);
        }


        public void CreatePage(int creator, string title, string pageName, string contents)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (contents == null) throw new ArgumentNullException("contents");

            var page = _repository.Create(creator, title, pageName);
            var parser = ParseBody(contents);
            page.SetBody(creator, contents, parser);
            _repository.Save(page);

            // Now fix all linking pages.
            var linkingPages = _repository.GetLinkingPages(pageName);
            foreach (var linkedPageName in linkingPages)
            {
                var linkedPage = _repository.Get(linkedPageName);

                var parserResult = ParseBody(linkedPage.RawBody);
                linkedPage.UpdateLinks(parserResult);
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

                var parserResult = ParseBody(linkedPage.RawBody);
                linkedPage.UpdateLinks(parserResult);
            }
        }

        private IWikiParserResult ParseBody(string contents)
        {
            var html = _textFormatParser.Parse(contents);
            var parser = new WikiParser(_repository, _configuration.RootUri);
            parser.Parse(html);
            return parser;
        }
    }

    public class Heading
    {
        public Heading(int level, string title)
        {
            if (level < 1 || level > 6)
                throw new ArgumentOutOfRangeException("level", "Level must be a propert heading value (1-6)");
            if (title == null) throw new ArgumentNullException("title");

            Level = level;
            Title = title;
            Children=new List<Heading>();
        }

        public int Level { get; private set; }
        public string Title { get; private set; }
        public List<Heading> Children { get; set; }
        public Heading Parent { get; set; }
    }
}