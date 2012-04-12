namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Processes the body after any <see cref="ITextProcessor"/>. 
    /// </summary>
    /// <remarks>The body is HTML content as this stage</remarks>
    public interface IHtmlProcessor
    {
        /// <summary>
        /// Process body contents
        /// </summary>
        /// <param name="context">Process context</param>
        void PreProcess(PreProcessorContext context);
    }
}