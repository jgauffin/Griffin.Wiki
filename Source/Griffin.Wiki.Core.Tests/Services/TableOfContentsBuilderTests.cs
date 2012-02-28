using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Griffin.Wiki.Core.Tests.Services
{
    //[TestClass]
    public class TableOfContentsBuilderTests
    {
        TableOfContentsBuilder _builder = new TableOfContentsBuilder();

        //[TestMethod]
        public void TestSimple()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile(@"<h1>Some heading</h1>Some data");
            _builder.GenerateList(writer);

            Assert.AreEqual(@"<ul>
<li>Some heading</li>
</ul>
", sb.ToString());
        }

        [Fact]
        public void Headings()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile("<h1>hello world</h1>kdkdsksddsk<h3>my h3</h3>lksdlkdlkdskl<h2>some heading</h2>");
            _builder.GenerateList(writer);

            Assert.AreEqual(@"<ul>
<li>Hello world
<ul>
<li>my h3</li>
<li>some heading</li>
</ul>
</li>
</ul>
", sb.ToString());
        }
    }
}
