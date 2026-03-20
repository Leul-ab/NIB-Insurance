using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Enums
{
    public enum ClaimStatus
    {
        Pending = 0,               // Client submitted
        ReviewedByOperator = 1,    // Operator proposed amount
        ApprovedByManager = 2,     // Manager verified
        Rejected = 3,
        RejectedByManager = 4,
        Paid = 5,
        PayoutFailed = 6

    }


}
