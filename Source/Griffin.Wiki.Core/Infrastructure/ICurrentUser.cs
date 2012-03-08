﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Infrastructure
{
    public interface ICurrentUser
    {
        User WikiUser { get; }
    }

    public static class WikiContext
    {
        public static User CurrentUser
        {
            get
            {
                var usr = Thread.CurrentPrincipal.Identity as ICurrentUser;
                if (usr == null)
                    throw new InvalidOperationException(
                        "Thread.CurrentPrincipal.Identity must implement Griffin.Wiki.Core.Infrastructure.ICurrentUser");

                return usr.WikiUser;
            }
        }
    }
}
