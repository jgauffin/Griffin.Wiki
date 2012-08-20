﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
