using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.KhipuApiClient
{
    public class PaymentsCreateResponse
    {
        public String Payment_id { get; set; }
        public String Payment_url { get; set; }
        public String Simplified_transfer_url { get; set; }
        public String Transfer_url { get; set; }
        public String Webpay_url { get; set; }
        public String App_url { get; set; }
        public bool Ready_for_terminal { get; set; }
    }
}
