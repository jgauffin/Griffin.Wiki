using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    ///   Processes the wiki body when it has been loaded from the database
    /// </summary>
    /// <remarks>
    ///   <para>Used to modify the HTML (parse any tags which can't be pre processed, i.e. before the generated body is saved in the DB)</para>
    /// </remarks>
    public interface IPostLoadProcessService
    {
        /// <summary>
        /// Process wiki body
        /// </summary>
        /// <param name="context">Page to process</param>
        /// <returns>Processed HTML body</returns>
        void Process(PostLoadProcessorContext context);
    }
}