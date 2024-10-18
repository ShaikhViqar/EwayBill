using EwayBill.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EwayBill.Services
{
    public class EwayBillService
    {
        private readonly string _clientId = ConfigurationManager.AppSettings["ClientId"];
        private readonly string _clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private readonly string _gstin = ConfigurationManager.AppSettings["Gstin"];
        private readonly string _apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        private readonly string _email = ConfigurationManager.AppSettings["UserEmail"];
        private readonly string _ipAddress = ConfigurationManager.AppSettings["IpAddress"];
        private readonly string _username = ConfigurationManager.AppSettings["UserName"];
        private readonly string _password = ConfigurationManager.AppSettings["Password"];
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["EwayBillDbContext"].ConnectionString;
        string tempstatus_cd = "0";
        //int tempID = 0;

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("client_id", _clientId);
                client.DefaultRequestHeaders.Add("client_secret", _clientSecret);
                client.DefaultRequestHeaders.Add("gstin", _gstin);
                client.DefaultRequestHeaders.Add("ip_address", _ipAddress);

                var url = _apiUrl + "/authenticate?email=" + _email + "&username=" + _username + "&password=" + _password;
                var httpResponse = await client.GetAsync(url);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    throw new Exception($"API request failed with status code {httpResponse.StatusCode} and response: {errorContent}");
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                try
                {
                    return JsonConvert.DeserializeObject<AuthenticationResponse>(responseContent);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
                    throw;
                }
            }
        }

        public async Task<EwayBillResponse> GenerateEwayBill(EwayBillRequest request)
        {
            var url = _apiUrl + "/ewayapi/genewaybill?email=shaikhviqar35%40gmail.com";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("client_id", _clientId);
                client.DefaultRequestHeaders.Add("client_secret", _clientSecret);
                client.DefaultRequestHeaders.Add("gstin", _gstin);
                client.DefaultRequestHeaders.Add("email", _email);
                client.DefaultRequestHeaders.Add("ip_address", _ipAddress);
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var json = JsonConvert.SerializeObject(request, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync(url, content);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    throw new Exception($"API request failed with status code {httpResponse.StatusCode} and response: {errorContent}");
                }
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<EwayBillResponse>(responseContent);

                tempstatus_cd = response.status_cd;
                if (tempstatus_cd == "1")
                {
                    //response.Data = new EwayBillData
                    //{
                    //    DocNo = request.DocNo,
                    //    ewayBillNo = response.Data.ewayBillNo,
                    //    EwayBillDate = response.Data.EwayBillDate,
                    //    ValidUpto = response.Data.ValidUpto,

                    //    SupplyType = request.SupplyType,
                    //    SubSupplyType = request.SubSupplyType,
                    //    DocType = request.DocType,
                    //    DocDate = request.DocDate,
                    //    FromGstin = request.FromGstin,
                    //    FromTrdName = request.FromTrdName,
                    //    FromAddr1 = request.FromAddr1,
                    //    FromAddr2 = request.FromAddr2,
                    //    FromPlace = request.FromPlace,
                    //    FromPincode = request.FromPincode,
                    //    FromStateCode = request.FromStateCode,
                    //    ToGstin = request.ToGstin,
                    //    ToTrdName = request.ToTrdName,
                    //    ToAddr1 = request.ToAddr1,
                    //    ToAddr2 = request.ToAddr2,
                    //    ToPlace = request.ToPlace,
                    //    ToPincode = request.ToPincode,
                    //    ToStateCode = request.ToStateCode,
                    //    TransactionType = request.TransactionType,
                    //    TotalValue = request.TotalValue,
                    //    CgstValue = request.CgstValue,
                    //    SgstValue = request.SgstValue,
                    //    IgstValue = request.IgstValue,
                    //    CessValue = request.CessValue,
                    //    CessNonAdvolValue = request.CessNonAdvolValue,
                    //    TotInvValue = request.TotInvValue,
                    //    TransMode = request.TransMode,
                    //    TransDistance = request.TransDistance,
                    //    TransporterId = request.TransporterId,
                    //    TransDocNo = request.TransDocNo,
                    //    VehicleNo = request.VehicleNo,
                    //    VehicleType = request.VehicleType,
                    //    ActFromStateCode = request.ActFromStateCode,
                    //    ActToStateCode = request.ActToStateCode,
                    //    DispatchFromGSTIN = request.DispatchFromGSTIN,
                    //    DispatchFromTradeName = request.DispatchFromTradeName,
                    //    ShipToGSTIN = request.ShipToGSTIN,
                    //    ShipToTradeName = request.ShipToTradeName,

                    //    ItemList = request.ItemList?.Select(item => new Item
                    //    {
                    //        ProductName = item.ProductName,
                    //        ProductDesc = item.ProductDesc,
                    //        HsnCode = item.HsnCode,
                    //        Quantity = item.Quantity,
                    //        QtyUnit = item.QtyUnit,
                    //        TaxableAmount = item.TaxableAmount,
                    //        CgstRate = item.CgstRate,
                    //        SgstRate = item.SgstRate,
                    //        IgstRate = item.IgstRate,
                    //        CessRate = item.CessRate,
                    //    }).ToList()
                    //};

                    //response.Data.ewayBillNo = response.Data.ewayBillNo;
                    //response.Data.EwayBillDate = response.Data.EwayBillDate;
                    //response.Data.ValidUpto = response.Data.ValidUpto;

                    //response.Data.DocNo = request.DocNo;

                    response.RequestData = request;

                    //Insert request data into the database
                    await InsertEwayBillRequestDataAsync(request, json);

                    // Insert response data into the database
                    await InsertEwayBillResponseDataAsync(response, responseContent);
                }

                //return JsonConvert.DeserializeObject<EwayBillResponse>(responseContent);
                return response;
            }
        }

        public async Task<EwayBillResponse> GetEwayBillDetailsAsync(string email, string ewbNo)
        {
            var url = _apiUrl + "/ewayapi/getewaybill?email=" + email + "&ewbNo=" + ewbNo;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("client_id", _clientId);
                client.DefaultRequestHeaders.Add("client_secret", _clientSecret);
                client.DefaultRequestHeaders.Add("gstin", _gstin);
                client.DefaultRequestHeaders.Add("ip_address", _ipAddress);
                var httpResponse = await client.GetAsync(url);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    throw new Exception($"API request failed with status code {httpResponse.StatusCode} and response: {errorContent}");
                }
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "dd/MM/yyyy HH:mm:ss"
                };
                try
                {
                    return JsonConvert.DeserializeObject<EwayBillResponse>(responseContent, settings);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
                    throw;
                }
            }
        }

        private async Task<int> InsertEwayBillRequestDataAsync(EwayBillRequest request, string requestJson)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Define the command and add parameters
                        using (var cmd = new SqlCommand("InsertEwayBillRequestData", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@SupplyType", request.SupplyType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SubSupplyType", request.SubSupplyType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DocType", request.DocType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DocNo", request.DocNo ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DocDate", request.DocDate ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@FromGstin", request.FromGstin ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@FromTrdName", request.FromTrdName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@FromAddr1", request.FromAddr1 ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@FromAddr2", request.FromAddr2 ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@FromPlace", request.FromPlace ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ActFromStateCode", request.ActFromStateCode == 0 ? 0 : request.ActFromStateCode);
                            cmd.Parameters.AddWithValue("@FromPincode", request.FromPincode == 0 ? 0 : request.FromPincode);
                            cmd.Parameters.AddWithValue("@FromStateCode", request.FromStateCode == 0 ? 0 : request.FromStateCode);
                            cmd.Parameters.AddWithValue("@ToGstin", request.ToGstin ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToTrdName", request.ToTrdName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToAddr1", request.ToAddr1 ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToAddr2", request.ToAddr2 ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToPlace", request.ToPlace ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToPincode", request.ToPincode == 0 ? 0 : request.ToPincode);
                            cmd.Parameters.AddWithValue("@ActToStateCode", request.ActToStateCode == 0 ? 0 : request.ActToStateCode);
                            cmd.Parameters.AddWithValue("@ToStateCode", request.ToStateCode == 0 ? 0 : request.ToStateCode);
                            cmd.Parameters.AddWithValue("@TransactionType", request.TransactionType == 0 ? 0 : request.TransactionType);
                            cmd.Parameters.AddWithValue("@DispatchFromGSTIN", request.DispatchFromGSTIN ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DispatchFromTradeName", request.DispatchFromTradeName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipToGSTIN", request.ShipToGSTIN ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipToTradeName", request.ShipToTradeName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TotalValue", request.TotalValue == 0 ? 0 : request.TotalValue);
                            cmd.Parameters.AddWithValue("@CGSTValue", request.CgstValue == 0 ? 0 : request.CgstValue);
                            cmd.Parameters.AddWithValue("@SGSTValue", request.SgstValue == 0 ? 0 : request.SgstValue);
                            cmd.Parameters.AddWithValue("@IGSTValue", request.IgstValue == 0 ? 0 : request.IgstValue);
                            cmd.Parameters.AddWithValue("@CessValue", request.CessValue == 0 ? 0 : request.CessValue);
                            cmd.Parameters.AddWithValue("@CessNonAdvolValue", request.CessNonAdvolValue == 0 ? 0 : request.CessNonAdvolValue);
                            cmd.Parameters.AddWithValue("@TotInvValue", request.TotInvValue == 0 ? 0 : request.TotInvValue);
                            cmd.Parameters.AddWithValue("@TransMode", request.TransMode ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TransDistance", request.TransDistance ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TransporterId", request.TransporterId ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TransDocNo", request.TransDocNo ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@VehicleNo", request.VehicleNo ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@VehicleType", request.VehicleType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@RequestJson", requestJson);

                            // Output parameter to get the inserted ID
                            var requestIdParameter = new SqlParameter("@RequestId", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(requestIdParameter);

                            await cmd.ExecuteNonQueryAsync();

                            // Retrieve the output parameter value
                            var requestId = (int)requestIdParameter.Value;

                            // Insert item data
                            foreach (var item in request.ItemList)
                            {
                                using (var itemCmd = new SqlCommand("InsertEwayBillItemData", conn, transaction))
                                {
                                    itemCmd.CommandType = CommandType.StoredProcedure;
                                    itemCmd.Parameters.AddWithValue("@EwayBillRequestId", requestId);
                                    itemCmd.Parameters.AddWithValue("@ProductName", item.ProductName ?? (object)DBNull.Value);
                                    itemCmd.Parameters.AddWithValue("@ProductDesc", item.ProductDesc ?? (object)DBNull.Value);
                                    itemCmd.Parameters.AddWithValue("@HsnCode", item.HsnCode == 0 ? 0 : item.HsnCode);
                                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity == 0 ? 0 : item.Quantity);
                                    itemCmd.Parameters.AddWithValue("@QtyUnit", item.QtyUnit ?? (object)DBNull.Value);
                                    itemCmd.Parameters.AddWithValue("@TaxableAmount", item.TaxableAmount == 0 ? 0 : item.TaxableAmount);
                                    itemCmd.Parameters.AddWithValue("@CGSTRate", item.CgstRate == 0 ? 0 : item.CgstRate);
                                    itemCmd.Parameters.AddWithValue("@SGSTRate", item.SgstRate == 0 ? 0 : item.SgstRate);
                                    itemCmd.Parameters.AddWithValue("@IGSTRate", item.IgstRate == 0 ? 0 : item.IgstRate);
                                    itemCmd.Parameters.AddWithValue("@CessRate", item.CessRate == 0 ? 0 : item.CessRate);

                                    await itemCmd.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();
                            return requestId; // Return the ID
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task InsertEwayBillResponseDataAsync(EwayBillResponse response, string responseJson)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("InsertEwayBillResponseData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DocNo", response.RequestData?.DocNo);
                    cmd.Parameters.AddWithValue("@ewayBillNo", response.Data?.ewayBillNo ?? 0);
                    cmd.Parameters.AddWithValue("@EwayBillDate", response.Data.EwayBillDate);
                    cmd.Parameters.AddWithValue("@ValidUpto", response.Data.ValidUpto);
                    cmd.Parameters.AddWithValue("@StatusCode", response.status_cd);
                    cmd.Parameters.AddWithValue("@StatusDesc", response.status_desc);
                    cmd.Parameters.AddWithValue("@ResponseJson", responseJson);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<EwayBillViewGenerateData>> GetAllEwayBillDetailsAsync(string DocNo)
        {
            var ewayBillList = new List<EwayBillViewGenerateData>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllEwayBillDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
                    con.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var parentData = new EwayBillRequest
                            {
                                SupplyType= reader["SupplyType"].ToString(),
                                SubSupplyType = reader["SubSupplyType"].ToString(),
                                DocType = reader["DocType"].ToString(),
                                DocNo = reader["DocNo"].ToString(),
                                DocDate = reader["DocDate"].ToString(),
                                FromGstin = reader["FromGstin"].ToString(),
                                FromTrdName = reader["FromTrdName"].ToString(),
                                FromAddr1 = reader["FromAddr1"].ToString(),
                                FromAddr2 = reader["FromAddr2"].ToString(),
                                FromPlace = reader["FromPlace"].ToString(),
                                ActFromStateCode = reader["ActFromStateCode"] != DBNull.Value ? Convert.ToInt32(reader["ActFromStateCode"]) : 0,
                                FromPincode = reader["FromPincode"] != DBNull.Value ? Convert.ToInt32(reader["FromPincode"]) : 0,
                                FromStateCode = reader["FromStateCode"] != DBNull.Value ? Convert.ToInt32(reader["FromStateCode"]) : 0,
                                ToGstin = reader["ToGstin"].ToString(),
                                ToTrdName = reader["ToTrdName"].ToString(),
                                ToAddr1 = reader["ToAddr1"].ToString(),
                                ToAddr2 = reader["ToAddr2"].ToString(),
                                ToPlace = reader["ToPlace"].ToString(),
                                ToPincode = reader["ToPincode"] != DBNull.Value ? Convert.ToInt32(reader["ToPincode"]) : 0,
                                ActToStateCode = reader["ActToStateCode"] != DBNull.Value ? Convert.ToInt32(reader["ActToStateCode"]) : 0,
                                ToStateCode = reader["ToStateCode"] != DBNull.Value ? Convert.ToInt32(reader["ToStateCode"]) : 0,
                                TransactionType = reader["TransactionType"] != DBNull.Value ? Convert.ToInt32(reader["TransactionType"]) : 0,
                                DispatchFromGSTIN = reader["DispatchFromGSTIN"].ToString(),
                                DispatchFromTradeName = reader["DispatchFromTradeName"].ToString(),
                                ShipToGSTIN = reader["ShipToGSTIN"].ToString(),
                                ShipToTradeName = reader["ShipToTradeName"].ToString(),
                                TotalValue = reader["TotalValue"] != DBNull.Value ? Convert.ToInt32(reader["TotalValue"]) : 0,
                                CgstValue = reader["CgstValue"] != DBNull.Value ? Convert.ToDecimal(reader["CgstValue"]) : 0,
                                SgstValue = reader["SgstValue"] != DBNull.Value ? Convert.ToDecimal(reader["SgstValue"]) : 0,
                                IgstValue = reader["IgstValue"] != DBNull.Value ? Convert.ToInt32(reader["IgstValue"]) : 0,
                                CessValue = reader["CessValue"] != DBNull.Value ? Convert.ToDecimal(reader["CessValue"]) : 0,
                                CessNonAdvolValue = reader["CessNonAdvolValue"] != DBNull.Value ? Convert.ToInt32(reader["CessNonAdvolValue"]) : 0,
                                TotInvValue = reader["TotInvValue"] != DBNull.Value ? Convert.ToDecimal(reader["TotInvValue"]) : 0,
                                TransMode = reader["TransMode"].ToString(),
                                TransDistance = reader["TransDistance"].ToString(),
                                TransporterId = reader["TransporterId"].ToString(),
                                TransDocNo = reader["TransDocNo"].ToString(),
                                VehicleNo = reader["VehicleNo"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                            };

                            var itemList = await GetChildItemsAsync(Convert.ToInt32(reader["ID"]));  // Fetch child items

                            var viewModel = new EwayBillViewGenerateData
                            {
                                ParentData = parentData,
                                ItemList = itemList
                            };

                            ewayBillList.Add(viewModel);
                        }
                    }
                }
            }

            return ewayBillList;
        }

        private async Task<List<Item>> GetChildItemsAsync(int ewayBillRequestId)
        {
            var itemList = new List<Item>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetEwayBillItems", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EwayBillRequestId", ewayBillRequestId);
                    con.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new Item
                            {
                                ProductName = reader["ProductName"].ToString(),
                                ProductDesc = reader["ProductDesc"].ToString(),
                                HsnCode = reader["HsnCode"] != DBNull.Value ? Convert.ToInt32(reader["HsnCode"]) : 0,
                                Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToDecimal(reader["Quantity"]) : 0,
                                QtyUnit = reader["QtyUnit"].ToString(),
                                TaxableAmount = reader["TaxableAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TaxableAmount"]) : 0,
                                CgstRate = reader["CgstRate"] != DBNull.Value ? Convert.ToDecimal(reader["CgstRate"]) : 0,
                                SgstRate = reader["SgstRate"] != DBNull.Value ? Convert.ToDecimal(reader["SgstRate"]) : 0,
                                IgstRate = reader["IgstRate"] != DBNull.Value ? Convert.ToDecimal(reader["IgstRate"]) : 0,
                                CessRate = reader["CessRate"] != DBNull.Value ? Convert.ToDecimal(reader["CessRate"]) : 0,
                            };
                            itemList.Add(item);
                        }
                    }
                }
            }

            return itemList;
        }

        public async Task<List<EwayBillResponse>> GetAllEwayBillResponseAsync(int pageNumber, int pageSize)
        {
            var ewayBillList = new List<EwayBillResponse>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetEwayBillResponse", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    con.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var ewayBillData = new EwayBillData
                            {
                                DocNo = reader["DocNo"].ToString(),
                                ewayBillNo = reader["ewayBillNo"] != DBNull.Value ? Convert.ToInt64(reader["ewayBillNo"]) : 0,
                                EwayBillDate = reader["EwayBillDate"].ToString(),
                                ValidUpto = reader["ValidUpto"].ToString(),
                            };

                            // Check if an EwayBillResponse already exists for the current row
                            var existingResponse = ewayBillList.FirstOrDefault(r => r.DataResponse == null);
                            if (existingResponse != null)
                            {
                                // If an existing response is found, add the current EwayBillData to its list
                                existingResponse.DataResponse.Add(ewayBillData);
                            }
                            else
                            {
                                // If no existing response, create a new EwayBillResponse with a new list
                                var ewayBillResponse = new EwayBillResponse
                                {
                                    DataResponse = new List<EwayBillData> { ewayBillData },
                                };
                                ewayBillList.Add(ewayBillResponse);
                            }
                        }
                    }
                }
            }

            return ewayBillList;
        }

        public async Task<List<DocumentType>> GetDocumentTypes()
        {
            var documentTypeList = new List<DocumentType>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetDocumentTypes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var documentType = new DocumentType
                            {
                                DocumentCode = reader["DocumentCode"].ToString(),
                                DocumentName = reader["DocumentName"].ToString(),
                            };
                            documentTypeList.Add(documentType);
                        }
                    }
                }
            }
            return documentTypeList;
        }

        public async Task<int> GetTotalEwayBillCountAsync()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM EwayBillResponseData WHERE IsActive = 1", con))
                {
                    con.Open();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            }
        }
    }
}