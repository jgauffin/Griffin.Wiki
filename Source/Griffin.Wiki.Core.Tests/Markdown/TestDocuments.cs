using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProjectPortal.Core.Tests.Markdown
{
    public class TestDocuments
    {
        static public Stream Get(string nameWithoutExtension)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (TestDocuments).Namespace + "." + nameWithoutExtension +
                                                                      ".txt");
            if (stream == null)
                throw new InvalidOperationException("Failed to find " + nameWithoutExtension);

            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }

        static public string GetText(string nameWithoutExtension)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(TestDocuments).Namespace + "." + nameWithoutExtension +
                                                                      ".txt");
            if (stream == null)
                throw new InvalidOperationException("Failed to find " + nameWithoutExtension);

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
