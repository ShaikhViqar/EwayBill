using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Hobbies { get; set; }
        public string FileName { get; set; }
        //public List<string> ChildFileNames { get; set; } = new List<string>();
        public List<UserChildFiles> ChildFileNames { get; set; } = new List<UserChildFiles>();
    }

    public class UserChildFiles
    {
        public int? FileID { get; set; }
        public int? UserID { get; set; }
        public string FileName { get; set; }
    }
}