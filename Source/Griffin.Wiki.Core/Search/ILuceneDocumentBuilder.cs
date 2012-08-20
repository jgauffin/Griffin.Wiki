using Lucene.Net.Documents;

namespace Griffin.Wiki.Core.Search
{
    internal interface ILuceneDocumentBuilder<in TModel>
    {
        Document CreateDocument(TModel model);
    }
}