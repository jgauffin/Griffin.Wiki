using System;
using System.Collections.Generic;
using System.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PreProcessors
{
    /// <summary>
    ///   Takes the wiki body as the user entered it and transforms it into usable HTML.
    /// </summary>
    [Component]
    public class PreProcessorService : IPreProcessorService
    {
        private readonly IEnumerable<IHtmlProcessor> _htmlProcessors;
        private readonly IEnumerable<ITextProcessor> _textProcessors;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PreProcessorService" /> class.
        /// </summary>
        /// <param name="textProcessors"> The text processors (text -> html). </param>
        /// <param name="htmlProcessors"> The HTML processors (html -> nicer html). </param>
        public PreProcessorService(IEnumerable<ITextProcessor> textProcessors,
                                   IEnumerable<IHtmlProcessor> htmlProcessors)
        {
            _textProcessors = textProcessors;
            _htmlProcessors = htmlProcessors;
        }

        #region IPreProcessorService Members

        /// <summary>
        ///   Invoke all processors.
        /// </summary>
        /// <param name="context"> Context information used during the processing. </param>
        public void Invoke(PreProcessorContext context)
        {
            var exceptions = new List<Exception>();
            foreach (var textProcessor in _textProcessors)
            {
                try
                {
                    textProcessor.PreProcess(context);
                }
                catch (Exception err)
                {
                    exceptions.Add(err);
                }
            }

            if (exceptions.Any())
                throw new TargetInvocationErrorsException("Failed to transform text into HTML: \r\n" + context.Body,
                                                          exceptions);

            exceptions.Clear();
            foreach (var htmlProcessor in _htmlProcessors)
            {
                try
                {
                    htmlProcessor.PreProcess(context);
                }
                catch (Exception err)
                {
                    exceptions.Add(err);
                }
            }

            if (exceptions.Any())
                throw new TargetInvocationErrorsException("Failed to adjust HTML: \r\n" + context.Body, exceptions);
        }

        #endregion
    }
}