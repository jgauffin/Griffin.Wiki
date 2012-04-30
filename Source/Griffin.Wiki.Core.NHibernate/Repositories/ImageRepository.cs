using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Griffin.Wiki.Core.Images.DomainModels;
using Griffin.Wiki.Core.Images.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.NHibernate.Repositories
{
    /// <summary>
    /// nhibernate implementation of the repository
    /// </summary>
    [Component]
    public class ImageRepository : IImageRepository
    {
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRepository"/> class.
        /// </summary>
        /// <param name="session">Current session.</param>
        public ImageRepository(ISession session)
        {
            _session = session;
        }


        /// <summary>
        /// Create a new image object.
        /// </summary>
        /// <param name="pagePath">Page that the image was created for</param>
        /// <param name="fileName">Original filename (name + extension, without path)</param>
        /// <param name="title">Title describing the image</param>
        /// <param name="contentType">Mime type</param>
        /// <param name="content">Image body</param>
        /// <returns>
        /// Created image object
        /// </returns>
        public WikiImage Create(PagePath pagePath, string fileName, string title, string contentType, Stream content)
        {
            var page = _session.Query<WikiPage>().FirstOrDefault(x => x.PagePath == pagePath);
            if (page == null)
                throw new InvalidOperationException("The specified page " + pagePath + " do not exist.");
            var image = new WikiImage(pagePath, title, fileName);
            image.SetFile(contentType, content);
            _session.Save(image);
            return image;
        }

        /// <summary>
        /// Get an image
        /// </summary>
        /// <param name="id">Image id</param>
        /// <returns>
        /// Image if found; otherwise null
        /// </returns>
        public WikiImage Get(int id)
        {
            return _session.Get<WikiImage>(id);
        }

        /// <summary>
        /// Find all images for an page
        /// </summary>
        /// <param name="pagePath">Name of page</param>
        /// <returns>
        /// Collection of images (or an empty collection)
        /// </returns>
        public IEnumerable<WikiImage> FindForPage(PagePath pagePath)
        {
            return _session.Query<WikiImage>().Where(x => x.Path == pagePath).ToList();
        }

        /// <summary>
        /// Find all existing images.
        /// </summary>
        /// <returns>
        /// Images
        /// </returns>
        public IEnumerable<WikiImage> FindAll()
        {
            return _session.Query<WikiImage>().ToList();

        }
    }
}
