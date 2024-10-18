using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace EwayBill.Utils
{
    public static class KeyGenerator
    {
        public static string GenerateBase64Key(int keySizeInBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySizeInBytes * 8; // Convert bytes to bits
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }
    }
}