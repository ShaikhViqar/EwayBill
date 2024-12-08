using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class ManageCountry
    {
        public int? CountryID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int UserID { get; set; }
    }
}