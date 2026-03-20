using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class Announcement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Content { get; set; }
        public bool SystemWide { get; set; } = true; // If true, visible to all logged-in users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; } // Manager User Id
        public string TargetGroup { get; set; }
    }

}
