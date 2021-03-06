﻿using System.Threading;
using Griffin.Wiki.Core.Data;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.Users.Repositories;
using Griffin.Container;

namespace Griffin.Wiki.Core.Pages.Services
{
    /// <summary>
    ///   Creates the Home and Help root pages
    /// </summary>
    [Component]
    public class InitialPageCreator : IScopedStartable
    {
        private readonly IPageRepository _pageRepository;
        private readonly IPreProcessorService _parser;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private static bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialPageCreator"/> class.
        /// </summary>
        /// <param name="pageRepository">The page repository.</param>
        /// <param name="parser">The content parser.</param>
        public InitialPageCreator(IPageRepository pageRepository, IPreProcessorService parser, IUserRepository userRepository, IUnitOfWork uow)
        {
            _pageRepository = pageRepository;
            _parser = parser;
            _userRepository = userRepository;
            _uow = uow;
        }

        #region IStartable Members

        /// <summary>
        /// Called during application startup
        /// </summary>
        public void StartScoped()
        {
            if (_started)
                return;
            _started = true;

            var user = _userRepository.GetOrCreate("MasterOfTheUniverse", "The might master of the Universe (and everything beyond)");
            Thread.CurrentPrincipal = new WikiPrinicpal(new WikiIdentity(user));

            if (_pageRepository.Get(new PagePath("/")) == null)
            {
                CreateHomePage();
            }

            if (_pageRepository.Get(new PagePath("/Help/")) == null)
            {
                CreateHelpPage();
            }

            _uow.SaveChanges();
        }

        #endregion

        private void CreateHomePage()
        {
            var page = _pageRepository.Create(0, new PagePath("/"), "Welcome to the Wiki", null);

            var body =
                @"#Welcome to the Wiki!

Congratulations to the choice of the most awsome wiki engine.

## Structure

This wiki forces a structure. No pages (except Home and Help) may exist without a parent page. This allows
us to generate a tree and make the users think before creating pages. It will hopefully lead to a better
organization than a regular wiki.

We also have introduced templates. Each page can define a template that is automatically added as page
content for all child pages.

";

            var ctx = new PreProcessorContext(page, body);
            _parser.Invoke(ctx);
            page.SetBody(ctx, "First release", _pageRepository);
        }

        private void CreateHelpPage()
        {
            var page = _pageRepository.Create(0, new PagePath("/Help/"), "Wiki help", null);

            var body =
                @"#Wiki help

This help is for the default text formatting language, **_Markdown_**.

## Headings

Use one dash `#` to create largest heading. Two for the second and finally three dashes for the third size.

## Paragraphs

Simply use an empty line.

## Italic text

Use underscore, `_italic_` becomes _italic_

## Bold

`**bold**` becomes **bold** ;)

## Qoute

`> hello world`

is

> Hello world

## Lists

### List

Use digit and a dot (`1.`) at the beginning on a line

1. Some point
2. ANother point
 1. Use spaces to create sub lists (one space per indentation level)

Bullet lists starts with a bullet and a space (`* Action point`)

## Code

Include code uses backtick `\`` as in `for (a = 1; a < 10; ++a) {}`.

Regular code uses four spaces at the beginning of each line.,

## Wikilinks

Use `[\[PageName]]` or `[\[PageName|This is a title]]`.

All pages should contain their parent page:s name `Guidelines` -> `GuidelinesCode`.


";
            var ctx = new PreProcessorContext(page, body);
            _parser.Invoke(ctx);
            page.SetBody(ctx, "First release", _pageRepository);
        }
    }
}