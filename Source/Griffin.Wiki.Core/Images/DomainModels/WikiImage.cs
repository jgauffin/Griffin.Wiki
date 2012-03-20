using System;
using System.IO;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Users.DomainModels;

namespace Griffin.Wiki.Core.Images.DomainModels
{
    public class WikiImage
    {
        public WikiImage(string pageName, string fileName)
        {
            
        }

        public virtual int Id { get; protected set; }
        public virtual WikiPage WikiPage { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual string Filename { get; protected set; }
        public virtual string Title { get; protected set; }
        public virtual byte[] Body { get; protected set; }
        public virtual DateTime UploadedAt { get; protected set; }
        public virtual string ContentType { get; protected set; }

        public void SetFile(string contentType, Stream inputStream)
        {
            Body = new byte[inputStream.Length];
            inputStream.Write(Body, 0, Body.Length);
            ContentType = contentType;
        }
    }
}