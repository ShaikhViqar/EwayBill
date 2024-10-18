using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class ResponseWrapper
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}