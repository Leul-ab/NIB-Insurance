using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ChapaInitResponse
    {
        public string Status { get; set; }
        public object Message { get; set; }  // <-- use object here
        public ChapaInitData Data { get; set; }
    }

    public class ChapaInitData
    {
        public string Checkout_Url { get; set; }
        public string Reference { get; set; }
    }


}
