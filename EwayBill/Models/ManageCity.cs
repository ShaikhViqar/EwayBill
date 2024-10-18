using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class ManageCity
    {
        public int? CityID { get; set; }
        public int? StateCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int UserID { get; set; }
    }
}