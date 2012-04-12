using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    /// Used to parse the body as the user entered it.
    /// </summary>
    /// <returns>Typically implemented to parse a text format like Markdown</returns>
    public interface ITextProcessor
    {
        /// <summary>
        /// Processes the body contents
        /// </summary>
        /// <param name="context">Processing context</param>
        void PreProcess(PreProcessorContext context);
    }
}
