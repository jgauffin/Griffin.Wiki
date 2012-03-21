using System;
using System.IO;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Images.DomainModels
{
    /// <summary>
    ///   Image stored in the database (or any other location)
    /// </summary>
    public class WikiImage
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="WikiImage" /> class.
        /// </summary>
        /// <param name="page"> The page that the image is for. </param>
        /// <param name="title"> </param>
        /// <param name="fileName"> Name of the file. </param>
        public WikiImage(WikiPage page, string title, string fileName)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (fileName == null) throw new ArgumentNullException("fileName");
            Title = title;
            WikiPage = page;
            Filename = fileName;
            UploadedBy = WikiContext.CurrentUser;
            UploadedAt = DateTime.Now;
        }

        protected WikiImage()
        {
            UploadedAt = DateTime.Now;
        }

        /// <summary>
        ///   Gets database id
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        ///   Gets binary body
        /// </summary>
        protected virtual byte[] Body { get; set; }

        /// <summary>
        ///   Gets mime type
        /// </summary>
        public virtual string ContentType { get; protected set; }

        /// <summary>
        ///   Gets original filename (name + extension)
        /// </summary>
        public virtual string Filename { get; protected set; }

        /// <summary>
        ///   Gets title specified by the user
        /// </summary>
        public virtual string Title { get; protected set; }

        /// <summary>
        ///   Gets when the image was uploaded
        /// </summary>
        public virtual DateTime UploadedAt { get; protected set; }

        /// <summary>
        ///   Gets user that the image was uploaded by
        /// </summary>
        public virtual User UploadedBy { get; protected set; }

        /// <summary>
        ///   Gets page that the image was uploaded for
        /// </summary>
        public virtual WikiPage WikiPage { get; protected set; }

        /// <summary>
        ///   Get stream for file
        /// </summary>
        /// <returns> A stream </returns>
        /// <returns>The caller owns the stream</returns>
        public virtual Stream GetFileStream()
        {
            return new MemoryStream(Body);
        }

        /// <summary>
        ///   Set a new body
        /// </summary>
        /// <param name="contentType"> mime type </param>
        /// <param name="inputStream"> Image stream </param>
        public virtual void SetFile(string contentType, Stream inputStream)
        {
            Body = new byte[inputStream.Length];
            inputStream.Read(Body, 0, Body.Length);
            ContentType = contentType;
        }
    }
}