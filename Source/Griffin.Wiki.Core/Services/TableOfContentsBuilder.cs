using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Griffin.Wiki.Core.Services
{
    public class TableOfContentsBuilder
    {
        private List<Heading> _headings = new List<Heading>();

        public void Compile(string html)
        {
            var regex = new Regex(@"<[hH]([1-3])>(.+?)</[hH][1-3]>");

            Heading last = null;
            foreach (Match match in regex.Matches(html))
            {
                var current = new Heading(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
                if (last == null)
                {
                    last = current;
                    _headings.Add(last);
                }
                else if (last.Level < current.Level)
                {
                    current.Parent = last;
                    last.Children.Add(current);
                }
                else if (last.Level == current.Level)
                {
                    if (last.Parent == null)
                        _headings.Add(current);
                    else
                        last.Parent.Children.Add(current);
                }
                else if (last.Level > current.Level)
                {
                    if (last.Parent == null)
                        _headings.Add(current);
                    else
                        last.Parent.Children.Add(current);
                }

                last = current;
            }
        }

        public void GenerateList(TextWriter writer)
        {
            writer.WriteLine("<ul>");

            foreach (var heading in _headings)
            {
                GenerateList(writer, heading);
            }

            writer.WriteLine("</ul>");
        }

        protected virtual void GenerateList(TextWriter writer, Heading heading)
        {
            writer.Write("<li>");
            writer.Write(heading.Title);
           
            if (heading.Children.Any())
            {
                writer.WriteLine();
                writer.WriteLine("<ul>");
                foreach (var child in heading.Children)
                {
                    GenerateList(writer, child);
                }

                writer.WriteLine("</ul>");
            }

            writer.WriteLine("</li>");
        }

    }
}
