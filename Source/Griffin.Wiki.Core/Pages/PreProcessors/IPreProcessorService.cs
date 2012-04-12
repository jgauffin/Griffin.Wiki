namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Transforms text (as the user entered it) into HTML.
    /// </summary>
    public interface IPreProcessorService
    {
        /// <summary>
        /// Invoke all processors.
        /// </summary>
        /// <param name="context">Context information used during the processing.</param>
        void Invoke(PreProcessorContext context);
    }
}