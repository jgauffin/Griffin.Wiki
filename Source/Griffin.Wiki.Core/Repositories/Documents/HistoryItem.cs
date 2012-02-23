using System;

namespace ProjectPortal.Core.Repositories.Documents
{
    public class HistoryItem
    {
        public string RawBody { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}