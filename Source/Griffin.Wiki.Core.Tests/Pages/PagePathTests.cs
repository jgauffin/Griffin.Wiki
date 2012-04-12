using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.Pages;
using Xunit;

namespace Griffin.Wiki.Core.Tests.Pages
{
    public class PagePathTests
    {
        string LinkRegEx = @"\[\[([.^|\]]+)([|]*)([\w ]*)\]\]";

        [Fact]
        public void Parse()
        {
            
        }
        [Fact]
        public void InitWithRelativePath()
        {
            var path1 = new PagePath("/some/path/");
            var path2 = new RelativePagePath(path1, "../other/path/");

        }

        [Fact]
        public void RootToChild()
        {
            var path1 = new PagePath("/");
            var path2 = new PagePath("/path/");
            var relative = path2.GetPathRelativeTo(path1);

        }

        [Fact]
        public void RelativePathToChild()
        {
            PagePath path1 = new PagePath("/some/");
            var path2 = new PagePath("/some/path/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("path/", relative.ToString());
        }


        [Fact]
        public void RelativePathSameSubDir()
        {
            PagePath path1 = new PagePath("/some/path/");
            var path2 = new PagePath("/some/other/path/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("../other/path/", relative.ToString());
        }

        [Fact]
        public void RelativePathToRoot()
        {
            PagePath path1 = new PagePath("/some/path/");
            var path2 = new PagePath("/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("../..", relative.ToString());
        }

        [Fact]
        public void RelativePathNoCommonFromTwoLevels()
        {
            PagePath path1 = new PagePath("/some/path/");
            var path2 = new PagePath("/other/path/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("../../other/path/", relative.ToString());
        }

        [Fact]
        public void RelativePathFromOneLevelToThree()
        {
            PagePath path1 = new PagePath("/some/");
            var path2 = new PagePath("/other/path/here/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("../other/path/here/", relative.ToString());
        }

        [Fact]
        public void RelativePathFromSameBaseDownOne()
        {
            PagePath path1 = new PagePath("/other/some/");
            var path2 = new PagePath("/other/path/here/");

            var relative = path1.GetPathRelativeTo(path2);

            Assert.Equal("../path/here/", relative.ToString());
        }
    }
}
