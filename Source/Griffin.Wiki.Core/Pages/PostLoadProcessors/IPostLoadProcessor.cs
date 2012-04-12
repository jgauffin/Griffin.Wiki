using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    /// Use to process a page before it's displayed (i.e. after it has been generated and saved)
    /// </summary>
    [Collection]
    public interface IPostLoadProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Processing context</param>
        /// <returns></returns>
        void ProcessHtml(PostLoadProcessorContext context);
    }
}
