using System.IO;
using System.Web;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Griffin.Wiki.Core.Search
{
    /*
    public class LuceneSearch
    {
        private readonly string _indexPath;

        public LuceneSearch(string indexPath)
        {
            _indexPath = indexPath;
            _luceneDir = Path.Combine(indexPath, "lucene_index");
        }

        private void _addToLuceneIndex(SampleData sampleData, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", sampleData.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", sampleData.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", sampleData.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", sampleData.Description, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        } 

        private readonly string _luceneDir;

        private FSDirectory GetTempDirectory()
        {
                if (_directoryTemp == null) 
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) 
                    IndexWriter.Unlock(_directoryTemp);

                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) 
                    File.Delete(lockFilePath);

                return _directoryTemp;
            
        }
    }
     * */
}