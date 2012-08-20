using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace Griffin.Wiki.Core.Search
{
    internal class IndexBuilder
    {
        protected ILuceneDocumentBuilder<T> CreateDocumentBuilder<T>()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes().Where(x => typeof (ILuceneDocumentBuilder<T>).IsAssignableFrom(x))
                    select (ILuceneDocumentBuilder<T>) Activator.CreateInstance(type)).FirstOrDefault();
        }

        public void BuildIndex<T>(string indexPath, IEnumerable<T> items)
        {
            var directory = FSDirectory.Open(new DirectoryInfo(indexPath));

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);

            var indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            var builder = CreateDocumentBuilder<T>();
            foreach (var product in items)
            {
                indexWriter.AddDocument(builder.CreateDocument(product));
            }

            indexWriter.Optimize();
            indexWriter.Close();
        }

        /*
        public void BuildAllIndexes()
        {
            var genericType = typeof (ILuceneDocumentBuilder<>);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var i in type.GetInterfaces())
                    {
                        if (i.GetGenericTypeDefinition() ==genericType)
                            BuildIndex();
                    }
                }
            }
        }*/
    }
}