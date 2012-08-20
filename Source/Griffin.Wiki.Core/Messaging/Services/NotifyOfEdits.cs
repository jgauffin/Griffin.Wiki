using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Griffin.Container.DomainEvents;
using Griffin.Wiki.Core.Pages.DomainModels.Events;

using Griffin.Container;

namespace Griffin.Wiki.Core.Messaging.Services
{
    [Component]
    class NotifyOfEdits : ISubscriberOf<RevisionModerationRequired>
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
            var administrator = ConfigurationManager.AppSettings["AdministratorEmail"];
            var domain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;

            var msg = new MailMessage("dotnetwiki@ssab.com", administrator);
            msg.Subject = "Moderation is required";
            msg.Body = string.Format(@"User {0} have edited the page {1}.

To review it, visit: {2}",
                                     e.Revision.CreatedBy.DisplayName, e.Revision.Page.PagePath,
                                     domain + _uriHelper.CreateLinkFromRoute(
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
