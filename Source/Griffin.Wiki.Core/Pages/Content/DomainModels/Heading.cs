using System;
using System.Collections.Generic;

namespace Griffin.Wiki.Core.Pages.Content.DomainModels
{
    /// <summary>
    /// A heading in a document
    /// </summary>
    public class Heading
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Heading"/> class.
        /// </summary>
        /// <param name="level">Heading size (1 = largest).</param>
        /// <param name="title">The title.</param>
        public Heading(int level, string title)
        {
            if (level < 1 || level > 6)
                throw new ArgumentOutOfRangeException("level", "Level must be a propert heading value (1-6)");
            if (title == null) throw new ArgumentNullException("title");

            Level = level;
            Title = title;
            Children = new List<Heading>();
        }

        /// <summary>
        /// Gets heading size (1-6)
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets heading title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets all child headings (less size)
        /// </summary>
        public List<Heading> Children { get; set; }

        /// <summary>
        /// Gets parent heading
        /// </summary>
        public Heading Parent { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<h{0}>{1}</h{0}>", Level, Title);
        }
    }
}