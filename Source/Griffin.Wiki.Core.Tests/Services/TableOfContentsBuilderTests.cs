using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages.Content.Services;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Services
{
    public class TableOfContentsBuilderTests
    {
        TableOfContentsBuilder _builder = new TableOfContentsBuilder();

        [Fact]
        public void TestSimple()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile(@"<h1>Some heading</h1>Some data");
            _builder.GenerateList(writer);

            Assert.Equal(@"<ul>
    <li>Some heading</li>
</ul>
", sb.ToString());
        }

        [Fact]
        public void TwoSubHeadingsOfDiffrentSizes()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile("<h1>hello world</h1>kdkdsksddsk<h3>my h3</h3>lksdlkdlkdskl<h2>some heading</h2>");
            _builder.GenerateList(writer);

            Assert.Equal(@"<ul>
    <li>hello world
        <ul>
            <li>my h3</li>
            <li>some heading</li>
        </ul>
    </li>
</ul>
", sb.ToString());
        }


        [Fact]
        public void ThreeNested()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile("<h1>hello world</h1>kdkdsksddsk<h2>my h2</h2>lksdlkdlkdskl<h3>some heading</h3>");
            _builder.GenerateList(writer);

            Assert.Equal(@"<ul>
    <li>hello world
        <ul>
            <li>my h2
                <ul>
                    <li>some heading</li>
                </ul>
            </li>
        </ul>
    </li>
</ul>
", sb.ToString());
        }

        [Fact]
        public void CompleteTree()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            _builder.Compile("<h1>hello world</h1>kdkdsksddsk<h2>my h2</h2>lksdlkdlkdskl<h3>some heading</h3><h1>Another h1</h1><h3>A h3</h3><h2>H2second</h2>");
            _builder.GenerateList(writer);

            Assert.Equal(@"<ul>
    <li>hello world
        <ul>
            <li>my h2
                <ul>
                    <li>some heading</li>
                </ul>
            </li>
        </ul>
    </li>
    <li>Another h1
        <ul>
            <li>A h3</li>
            <li>H2second</li>
        </ul>
    </li>
</ul>
", sb.ToString());
        }
    }
}
