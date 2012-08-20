using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Search;
using Lucene.Net.Documents;

namespace Griffin.Wiki.Core.Pages
{
    class PageIndex : LuceneAdapter<WikiPage>
    {
        /// <summary>
        /// Invoke <c>Map()</c> in this method for every field that should be searched.
        /// </summary>
        public override void MapFields()
        {
            IndexId(x => x.Id);
            IndexString(x => x.Title);
            IndexText(x => x.RawBody);
        }

        public override string IndexName
        {
            get { return "Pages"; }
        }
    }
}
