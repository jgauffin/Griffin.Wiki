using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Griffin.Wiki.Core.Pages.DomainModels.Events;
using Sogeti.Pattern.DomainEvents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Messaging.Services
{
    [Component]
    class NotifyOfEdits : IAutoSubscriberOf<RevisionModerationRequired>
    {
        private readonly IUriHelper _uriHelper;
        
        public NotifyOfEdits(IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
        }

        /// <summary>
        /// Handle the domain event
        /// </summary>
        /// <param name="e">Domain to process</param>
        public void Handle(RevisionModerationRequired e)
        {
            var msg = new MailMessage("dotnetwiki@ssab.com", "jonas@gauffin.com");
            msg.Subject = "Moderation is required";
            msg.Body = string.Format(@"User {0} have edited the page {1}.

To review it, visit: {2}",
                                     e.Revision.CreatedBy.DisplayName, e.Revision.Page.PagePath,
                                     _uriHelper.CreateLinkFromRoute(
                                         new
                                             {
                                                 controller = "Review",
                                                 action = "Index",
                                                 id = e.Revision.Page.PagePath,
                                                 revision = e.Revision.Id
                                             }));

            new SmtpClient().Send(msg);
        }
    }
}
