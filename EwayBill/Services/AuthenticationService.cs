using EwayBill.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace EwayBill.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _gstin;
        private readonly string _base64Key;

        public AuthenticationService(HttpClient httpClient, string clientId, string clientSecret, string gstin, string base64Key)
        {
            _httpClient = httpClient;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _gstin = gstin;
            _base64Key = base64Key;
        }

        //public async Task<string> GetAuthTokenAsync(string username, string password)
        //{
        //    var requestPayload = new
        //    {
        //        Data = EncryptData(new
        //        {
        //            Action = "ACCESSTOKEN",
        //            Password = password,
        //            App_Key = GenerateAppKey(),
        //            UserName = username
        //        })
        //    };

        //    var response = await _httpClient.PostAsJsonAsync("https://api.mastergst.com/v1.03/auth", requestPayload);
        //    response.EnsureSuccessStatusCode();

        //    var responseBody = await response.Content.ReadAsStringAsync();
        //    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

        //    return EncryptionHelper.Decrypt(authResponse.Data.Sek, GenerateAppKey());
        //}

        public async Task<string> GetAuthTokenAsync(string username, string password)
        {
            try
            {
                var requestPayload = new
                {
                    Data = EncryptData(new
                    {
                        Action = "ACCESSTOKEN",
                        Password = password,
                        App_Key = GenerateAppKey(),
                        UserName = username
                    })
                };

                var response = await _httpClient.PostAsJsonAsync("https://api.mastergst.com/ewaybillapi/v1.03/", requestPayload);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Log the response body for debugging
                Console.WriteLine("API Response: " + responseBody);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

                // Check if authResponse or authResponse.Data is null
                if (authResponse == null)
                {
                    throw new Exception("AuthResponse is null.");
                }

                if (authResponse.Data == null)
                {
                    throw new Exception("AuthResponse.Data is null.");
                }

                var sek = authResponse.Data.Sek;

                if (string.IsNullOrEmpty(sek))
                {
                    throw new Exception("SEK is null or empty.");
                }

                return EncryptionHelper.Decrypt(sek, GenerateAppKey());
            }
            catch (Exception ex)
            {
                // Log the exception details here
                Console.WriteLine("Error occurred while getting auth token: " + ex.Message);
                throw;
            }
        }

        private string EncryptData(object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            return EncryptionHelper.Encrypt(jsonData, _base64Key);
        }

        //private string GenerateAppKey()
        //{
        //    using (var aes = Aes.Create())
        //    {
        //        return Convert.ToBase64String(aes.Key);
        //    }
        //}
        private string GenerateAppKey()
        {
            return KeyGenerator.GenerateBase64Key(32); // Generate a 256-bit key (32 bytes)
        }

        private class AuthResponse
        {
            public AuthData Data { get; set; }

            public class AuthData
            {
                public string Sek { get; set; }
            }
        }
    }
}