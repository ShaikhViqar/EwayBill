using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public List<ChildFile> ChildFiles { get; set; }

        public Employee()
        {
            ChildFiles = new List<ChildFile>();
        }
    }

    public class ChildFile
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public int EmployeeId { get; set; }
    }
}