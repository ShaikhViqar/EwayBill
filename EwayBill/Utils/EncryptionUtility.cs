using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EwayBill.Utils
{
    public static class EncryptionUtility
    {
        private static readonly string EncryptionKey = "shaikh/wCCQaZWO/wGwcTn4AwzRoU/XQ2W4JRkpHE20WA891U=/viqar"; // Replace with a secure key

        public static string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 16));
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV to ciphertext
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            byte[] cipherBytes = new byte[fullCipher.Length - 16];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 16));
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        //public static string GenerateEncryptionKey(int keySize = 32) // 32 bytes = 256-bit key
        //{
        //    using (var rng = new RNGCryptoServiceProvider())
        //    {
        //        byte[] keyBytes = new byte[keySize];
        //        rng.GetBytes(keyBytes); // Fill the byte array with cryptographically strong random bytes
        //        return Convert.ToBase64String(keyBytes); // Convert the byte array to a base64 string
        //    }
        //}
    }
}