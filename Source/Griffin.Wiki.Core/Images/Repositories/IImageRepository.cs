using System.Collections.Generic;
using System.IO;
using Griffin.Wiki.Core.Images.DomainModels;

namespace Griffin.Wiki.Core.Images.Repositories
{
    /// <summary>
    /// Used to work with images
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// Create a new image object.
        /// </summary>
        /// <param name="pageName">Page that the image was created for</param>
        /// <param name="fileName">Original filename (name + extension, without path)</param>
        /// <param name="title">Title describing the image </param>
        /// <param name="contentType">Mime type </param>
        /// <param name="content">Image body</param>
        /// <returns>Created image object</returns>
        WikiImage Create(string pageName, string fileName, string title, string contentType, Stream content);

        /// <summary>
        /// Get an image
        /// </summary>
        /// <param name="id">Image id</param>
        /// <returns>Image if found; otherwise null</returns>
        WikiImage Get(int id);

        /// <summary>
        /// Find all images for an page
        /// </summary>
        /// <param name="pageName">Name of page</param>
        /// <returns>Collection of images (or an empty collection)</returns>
        IEnumerable<WikiImage> FindForPage(string pageName);

        /// <summary>
        /// Find all existing images.
        /// </summary>
        /// <returns>Images</returns>
        IEnumerable<WikiImage> FindAll();
    }
}