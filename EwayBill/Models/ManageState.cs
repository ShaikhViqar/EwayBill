using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class ManageState
    {
        public int? StateID { get; set; }
        public int? StateCode { get; set; }
        public string State { get; set; }
        public int UserID { get; set; }
    }
}