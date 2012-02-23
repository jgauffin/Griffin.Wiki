using System;
using System.IO;
using System.Reflection;

namespace Griffin.Wiki.Core.Tests.Markdown
{
    public class TestDocuments
    {
        public static Stream Get(string nameWithoutExtension)
        {
            var stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (TestDocuments).Namespace + "." +
                                                                          nameWithoutExtension +
                                                                          ".txt");
            if (stream == null)
                throw new InvalidOperationException("Failed to find " + nameWithoutExtension);

            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }

        public static string GetText(string nameWithoutExtension)
        {
            var stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (TestDocuments).Namespace + "." +
                                                                          nameWithoutExtension +
                                                                          ".txt");
            if (stream == null)
                throw new InvalidOperationException("Failed to find " + nameWithoutExtension);

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}