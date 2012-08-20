using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Lucene.Net.Documents;

namespace Griffin.Wiki.Core.Search
{
    abstract class LuceneAdapter<TModel> : ILuceneDocumentBuilder<TModel>
    {
        private List<Mapper> _mappings = new List<Mapper>();

            class Mapper
            {
                private readonly Func<TModel, object> _action;
                public string PropertyName { get; set; }
                public Field.Store Store { get; set; }
                public Field.Index Index { get; set; }

                public Mapper(string propertyName, Func<TModel, object> action, Field.Store store, Field.Index index)
                {
                    _action = action;
                    PropertyName = propertyName;
                    Store = store;
                    Index = index;
                }

                public object GetValue(TModel instance)
                {
                    return _action(instance);
                }
            }


        public void IndexString(Expression<Func<TModel, object>> property)
        {
            Map(property, Field.Store.YES, Field.Index.ANALYZED);    
        }
        public void IndexText(Expression<Func<TModel, object>> property)
        {
            Map(property, Field.Store.COMPRESS, Field.Index.ANALYZED);
        }
        public void IndexId(Expression<Func<TModel, object>> property)
        {
            Map(property, Field.Store.YES, Field.Index.NOT_ANALYZED);
        }
        public void IndexNumber(Expression<Func<TModel, object>> property)
        {
            Map(property, Field.Store.YES, Field.Index.NOT_ANALYZED);
        }



        protected void Map(Expression<Func<TModel, object>> property, Field.Store store, Field.Index index)
        {
            var propertyName = ((MemberExpression) property.Body).Member.Name;
            var action = property.Compile();
            _mappings.Add(new Mapper(propertyName, action, store, index));
        }

        /// <summary>
        /// Invoke <c>Map()</c> in this method for every field that should be searched.
        /// </summary>
        public abstract void MapFields();

        public abstract string IndexName { get; }
        public Document CreateDocument(TModel model)
        {
            var doc = new Document();
            foreach (var mapping in _mappings)
            {
                doc.Add(new Field(mapping.PropertyName, mapping.GetValue(model).ToString(), mapping.Store, mapping.Index));
            }

            return doc;
        }


    }
}
