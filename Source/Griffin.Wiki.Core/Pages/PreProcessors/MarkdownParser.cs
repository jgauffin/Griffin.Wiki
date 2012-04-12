using MarkdownDeep;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Parses markdown
    /// </summary>
    public class MarkdownParser : ITextProcessor
    {
        public void PreProcess(PreProcessorContext context)
        {
            var md = new Markdown { ExtraMode = true };
            context.Body = md.Transform(context.Body);
        }
    }
}