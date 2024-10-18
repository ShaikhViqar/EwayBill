using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class CheckBoxOption
    {
        public int Id { get; set; }
        public string OptionName { get; set; }
        public bool IsChecked { get; set; }
    }
}