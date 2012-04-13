using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Griffin.Wiki.Core.SiteMaps.DomainModels;

namespace Griffin.Wiki.Mvc3.Helpers
{
    /// <summary>
    ///   HtmlHelper for Sitemap
    /// </summary>
    public static class SitemapHelper
    {
        /// <summary>
        ///   Create a sitemap for a node
        /// </summary>
        /// <typeparam name="TModel"> View model </typeparam>
        /// <param name="htmlHelper"> Html helper </param>
        /// <param name="property"> Propery selection expression </param>
        /// <returns> Generated sitemap (no ul/li for the selected node, but for it's children) </returns>
        public static MvcHtmlString SiteMapFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                       Expression<Func<TModel, SiteMapNode>> property,
                                                       Action<SiteMapNode, TagBuilder> itemVisitor = null)
        {
            /*    return TextBoxHelper(htmlHelper,
                                        ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model,
                                        ExpressionHelper.GetExpressionText(expression),
                                        htmlAttributes);*/

            var sb = new StringBuilder();
            GenerateMap(sb, itemVisitor,
                        (SiteMapNode)ModelMetadata.FromLambdaExpression(property, htmlHelper.ViewData).Model,
                        "    ");

            return new MvcHtmlString(sb.ToString());
        }


        /// <summary>
        ///   Create a sitemap for a ndoe
        /// </summary>
        /// <param name="htmlHelper"> Html helper </param>
        /// <param name="htmlAttributes"> Any html attributes for the root <c>ul</c> tag </param>
        /// <returns> Generated sitemap (no ul/li for the selected node, but for it's children) </returns>
        public static MvcHtmlString SiteMap(this HtmlHelper<SiteMapNode> htmlHelper,
                                            IDictionary<string, object> htmlAttributes = null,
                                            Action<SiteMapNode, TagBuilder> itemVisitor = null)
        {
            /*    return TextBoxHelper(htmlHelper,
                                        ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model,
                                        ExpressionHelper.GetExpressionText(expression),
                                        htmlAttributes);*/


            var sb = new StringBuilder();
            GenerateMap(sb, itemVisitor, htmlHelper.ViewData.Model, "    ");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        ///   Create a sitemap for a ndoe
        /// </summary>
        /// <param name="htmlHelper"> Html helper </param>
        /// <param name="htmlAttributes"> Any html attributes for the root <c>ul</c> tag </param>
        /// <param name="itemVisitor"> Invoked for each generated LI node. </param>
        /// <returns> Generated sitemap (no ul/li for the selected node, but for it's children) </returns>
        public static MvcHtmlString SiteMap(this HtmlHelper<IEnumerable<SiteMapNode>> htmlHelper,
                                            IDictionary<string, object> htmlAttributes = null,
                                            Action<SiteMapNode, TagBuilder> itemVisitor = null,
            string emptyText = "There are no map for the current page.")
        {
            /*    return TextBoxHelper(htmlHelper,
                                        ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model,
                                        ExpressionHelper.GetExpressionText(expression),
                                        htmlAttributes);*/


            if (!htmlHelper.ViewData.Model.Any())
                return MvcHtmlString.Create(emptyText);

            var sb = new StringBuilder();
            GenerateMap(htmlAttributes, itemVisitor, htmlHelper.ViewData.Model, sb);
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        ///   Generate a complete sitemap (including top UL)
        /// </summary>
        /// <typeparam name="TModel"> View model </typeparam>
        /// <param name="htmlHelper"> Html helper </param>
        /// <param name="property"> Property selection expression </param>
        /// <param name="htmlAttributes"> Any html attributes for the root <c>ul</c> tag </param>
        /// <returns> Generated sitemap </returns>
        public static MvcHtmlString SiteMapFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                       Expression<Func<TModel, IEnumerable<SiteMapNode>>> property,
                                                       IDictionary<string, object> htmlAttributes = null,
                                                       Action<SiteMapNode, TagBuilder> itemVisitor = null)
        {
            var sb = new StringBuilder();
            var model =
                (IEnumerable<SiteMapNode>)ModelMetadata.FromLambdaExpression(property, htmlHelper.ViewData).Model;

            GenerateMap(htmlAttributes, itemVisitor, model, sb);

            return new MvcHtmlString(sb.ToString());
        }

        private static void GenerateMap(IDictionary<string, object> htmlAttributes,
                                        Action<SiteMapNode, TagBuilder> itemVisitor, IEnumerable<SiteMapNode> model,
                                        StringBuilder sb)
        {
            var tb = new TagBuilder("ul");
            tb.AddCssClass("sitemap");
            if (htmlAttributes != null)
                tb.MergeAttributes(htmlAttributes);

            var spaces = "    ";
            sb.AppendFormat("{0}{1}\r\n", spaces, tb.ToString(TagRenderMode.StartTag));
            spaces += "    ";
            foreach (var node in model)
            {
                var itemTag = new TagBuilder("li");
                if (node.IsCurrent)
                    itemTag.AddCssClass("current");
                if (itemVisitor != null)
                    itemVisitor(node, itemTag);

                sb.AppendFormat("{0}{1}\r\n", spaces, itemTag.ToString(TagRenderMode.StartTag));
                GenerateMap(sb, itemVisitor, node, spaces);
                sb.AppendFormat("{0}</li>\r\n", spaces);
            }
            spaces = spaces.Remove(spaces.Length - 4, 4);
            sb.AppendFormat("{0}</ul>\r\n", spaces);
        }

        private static void GenerateMap(StringBuilder sb, Action<SiteMapNode, TagBuilder> itemVisitor, SiteMapNode node,
                                        string spaces)
        {
            sb.AppendFormat("{0}{1}\r\n", spaces, node.Link);
            if (!node.Children.Any())
                return;

            spaces += "    ";
            sb.AppendFormat("{0}<ul>\r\n", spaces);
            spaces += "    ";
            foreach (var child in node.Children)
            {
                var itemTag = new TagBuilder("li");
                if (child.IsCurrent)
                    itemTag.AddCssClass("current");
                if (itemVisitor != null)
                    itemVisitor(node, itemTag);


                sb.AppendFormat("{0}{1}\r\n", spaces, itemTag.ToString(TagRenderMode.StartTag));
                GenerateMap(sb, itemVisitor, child, spaces);
                sb.AppendFormat("{0}</li>\r\n", spaces);
            }
            spaces = spaces.Remove(spaces.Length - 4, 4);
            sb.AppendFormat("{0}</ul>\r\n", spaces);
        }
    }
}