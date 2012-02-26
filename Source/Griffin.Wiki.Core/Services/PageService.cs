﻿using System;
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
            foreach (var linkingPage in linkingPages)
            {
                page.UpdateLinks();
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
}