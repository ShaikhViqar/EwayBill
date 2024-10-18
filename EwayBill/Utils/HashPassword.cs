using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EwayBill.Utils
{
    public class HashPassword
    {
        public static string HashPasswords(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}