using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Images.DomainModels;

namespace Griffin.Wiki.Core.Images.Repositories
{
    public class ImageRepository
    {
        
        public WikiImage Create(string pageName, string fileName)
        {
            var image = new WikiImage(pageName, fileName);
            return image;
        }

        public WikiImage Get(int id)
        {
            return null;
        }

        public IEnumerable<WikiImage> FindForPage(string pageName)
        {
            return new List<WikiImage>();
        }

        public IEnumerable<WikiImage> FindAll()
        {
            return new List<WikiImage>();

        }
    }
}
