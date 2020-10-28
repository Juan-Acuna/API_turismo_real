using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.KhipuApiClient
{
    public class KhipuResponse
    {
        public String StatusCode { get; set; }
        public PaymentsCreateResponse PaymentsCreateResponse { get; set; }
    }
}
