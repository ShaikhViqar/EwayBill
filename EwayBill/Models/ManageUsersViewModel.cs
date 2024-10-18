using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class ManageUsersViewModel
    {
        public List<User> Users { get; set; }
        public int TotalItems { get; set; }
    }
}