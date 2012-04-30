using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Pages.DomainModels;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Pages.PostLoadProcessors
{
    /// <summary>
    ///   Processes the wiki body when it has been loaded from the database
    /// </summary>
    /// <remarks>
    ///   <para>Used to modify the HTML (parse any tags which can't be pre processed, i.e. before the generated body is saved in the DB)</para>
    ///  <para>Uses the IoC container to find all
    ///                                                                                                                                           <see cref="IPostLoadProcessor" />
    ///                                                                                                                                           and let them have a go on the body</para>
    /// </remarks>
    [Component]
    public class PostLoadProcessService : IPostLoadProcessService
    {
        private readonly IEnumerable<IPostLoadProcessor> _processors;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PostLoadProcessService" /> class.
        /// </summary>
        /// <param name="processors"> The processors. </param>
        public PostLoadProcessService(IEnumerable<IPostLoadProcessor> processors)
        {
            _processors = processors;
        }

        #region IPostLoadProcessService Members

        /// <summary>
        ///   Process wiki body
        /// </summary>
        /// <param name="context"> Page to process </param>
        /// <returns> Processed HTML body </returns>
        public void Process(PostLoadProcessorContext context)
        {
            var generationExceptions = new List<Exception>();
            foreach (var postProcessor in _processors)
            {
                try
                {
                    postProcessor.ProcessHtml(context);
                }
                catch (Exception err)
                {
                    generationExceptions.Add(err);
                }
            }

            if (generationExceptions.Any())
            {
                throw new TargetInvocationErrorsException("Failed to post load process context " + context,
                                                          generationExceptions);
            }
        }

        #endregion
    }
}