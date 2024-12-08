using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EwayBill.Models;

namespace EwayBill.Services
{
    public class MastersService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["EwayBillDbContext"].ConnectionString;

        #region Manage Role
        public async Task<ManageRole> SaveManageRole(ManageRole manageRole)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
            new SqlParameter{ParameterName = "@Role", DbType = DbType.String, Value = manageRole.Role},
            new SqlParameter{ParameterName = "@UserID", DbType = DbType.Int32, Value = manageRole.UserID},
            new SqlParameter{ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
            new SqlParameter{ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000}
            };
            if (manageRole.RoleID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@RoleID", DbType = DbType.Int32, Value = manageRole.RoleID });
            }
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SaveManageRole", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();

                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }

                    int id = cmd.Parameters["@ID"].Value as int? ?? 0;
                    manageRole.RoleID = id;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageRole;
        }

        public async Task<List<ManageRole>> GetManageRole(int RoleID, int? pageNumber, int? pageSize, string searchQuery)
        {
            #region Declaration
            DataTable dt = new DataTable();
            SqlConnection con = null;
            List<ManageRole> manageRoles = null;
            List<SqlParameter> param = new List<SqlParameter>();

            if (RoleID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@RoleID", DbType = DbType.Int32, Value = RoleID });
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                param.Add(new SqlParameter { ParameterName = "@SearchQuery", DbType = DbType.String, Value = searchQuery });
            }
            #endregion

            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("ShowManageRole", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                #endregion

                #region Wrap data
                if (dt.Rows.Count > 0)
                {
                    manageRoles = new List<ManageRole>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ManageRole manageRole = new ManageRole
                        {
                            RoleID = row["RoleID"] as int? ?? 0,
                            Role = row["Role"] as string ?? string.Empty,
                            UserID = row["UserID"] as int? ?? 0
                        };
                        manageRoles.Add(manageRole);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageRoles;
        }

        public async Task<int> GetTotalRoleCount(string searchQuery)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ManageRole where IsActive = 1", con); // Adjust to your actual roles table
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.CommandText += " AND (Role LIKE @SearchQuery OR CAST(RoleID AS VARCHAR) LIKE @SearchQuery)";
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                }
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public bool IsManageRoleTaken(string role)
        {
            bool isTaken = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckManageRoleExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Role", role);
                    conn.Open();
                    int result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        isTaken = true;
                    }
                }
            }
            return isTaken;
        }

        public async Task<string> DeleteManageRole(int roleID)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@UserID", DbType = DbType.Int32, Value = 1 },
                new SqlParameter { ParameterName = "@RoleID", DbType = DbType.Int32, Value = roleID },
                new SqlParameter { ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
                new SqlParameter { ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000 }
            };
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("DeleteManageRole", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }
                }
                #endregion
                return "Role deleted successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }
        #endregion

        #region Manage Country
        public async Task<List<ManageCountry>> GetManageCountry(int CountryID, int? pageNumber, int? pageSize, string searchQuery)
        {
            #region Declaration
            DataTable dt = new DataTable();
            SqlConnection con = null;
            List<ManageCountry> manageCountries = null;
            List<SqlParameter> param = new List<SqlParameter>();

            if (CountryID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@CountryID", DbType = DbType.Int32, Value = CountryID });
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                param.Add(new SqlParameter { ParameterName = "@SearchQuery", DbType = DbType.String, Value = searchQuery });
            }
            #endregion

            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("ShowManageCountry", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                #endregion

                #region Wrap data
                if (dt.Rows.Count > 0)
                {
                    manageCountries = new List<ManageCountry>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ManageCountry manageCountry = new ManageCountry
                        {
                            CountryID = row["CountryID"] as int? ?? 0,
                            CountryCode = row["CountryCode"] as string ?? string.Empty,
                            CountryName = row["CountryName"] as string ?? string.Empty,
                            UserID = row["UserID"] as int? ?? 0
                        };
                        manageCountries.Add(manageCountry);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageCountries;
        }

        public async Task<int> GetTotalCountryCount(string searchQuery)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ManageCountry where IsActive = 1", con); // Adjust to your actual roles table
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.CommandText += " AND (CountryName LIKE @SearchQuery OR CAST(CountryID AS VARCHAR) LIKE @SearchQuery OR CountryCode LIKE @SearchQuery)";
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                }
                return (int)await cmd.ExecuteScalarAsync();
            }
        }
        #endregion

        #region Manage State
        public async Task<ManageState> SaveManageState(ManageState manageState)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
            new SqlParameter{ParameterName = "@StateCode", DbType = DbType.String, Value = manageState.StateCode},
            new SqlParameter{ParameterName = "@State", DbType = DbType.String, Value = manageState.State},
            new SqlParameter{ParameterName = "@UserID", DbType = DbType.Int32, Value = manageState.UserID},
            new SqlParameter{ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
            new SqlParameter{ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000}
            };
            if (manageState.StateID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@StateID", DbType = DbType.Int32, Value = manageState.StateID });
            }
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SaveManageState", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();

                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }

                    int id = cmd.Parameters["@ID"].Value as int? ?? 0;
                    manageState.StateID = id;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageState;
        }

        public async Task<List<ManageState>> GetManageState(int StateID, int? pageNumber, int? pageSize, string searchQuery, string CountryCode)
        {
            #region Declaration
            DataTable dt = new DataTable();
            SqlConnection con = null;
            List<ManageState> manageStates = null;
            List<SqlParameter> param = new List<SqlParameter>();

            if (StateID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@StateID", DbType = DbType.Int32, Value = StateID });
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                param.Add(new SqlParameter { ParameterName = "@SearchQuery", DbType = DbType.String, Value = searchQuery });
            }
            if (!string.IsNullOrEmpty(CountryCode))
            {
                param.Add(new SqlParameter { ParameterName = "@CountryCode", DbType = DbType.String, Value = CountryCode });
            }
            #endregion

            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("ShowManageState", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                #endregion

                #region Wrap data
                if (dt.Rows.Count > 0)
                {
                    manageStates = new List<ManageState>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ManageState manageState = new ManageState
                        {
                            StateID = row["StateID"] as int? ?? 0,
                            CountryCode = row["CountryCode"] as string ?? string.Empty,
                            StateCode = row["StateCode"] as int? ?? 0,
                            State = row["State"] as string ?? string.Empty,
                            UserID = row["UserID"] as int? ?? 0
                        };
                        manageStates.Add(manageState);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageStates;
        }

        //public async Task<int> GetTotalStateCount(string searchQuery, string CountryCode)
        //{
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ManageState where IsActive = 1", con); // Adjust to your actual roles table
        //        if (!string.IsNullOrEmpty(searchQuery))
        //        {
        //            cmd.CommandText += " AND (State LIKE @SearchQuery OR CAST(StateID AS VARCHAR) LIKE @SearchQuery OR CAST(StateCode AS VARCHAR) LIKE @SearchQuery)";
        //            cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
        //        }
        //        return (int)await cmd.ExecuteScalarAsync();
        //    }
        //}

        public async Task<int> GetTotalStateCount(string searchQuery, string CountryCode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                // Prepare the SQL query with initial conditions
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM ManageState
                    WHERE IsActive = 1", con);

                // Add a filter for searchQuery, if provided
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.CommandText += @"
                     AND (
                         State LIKE @SearchQuery OR 
                         CAST(StateID AS VARCHAR) LIKE @SearchQuery OR 
                         CAST(StateCode AS VARCHAR) LIKE @SearchQuery)";
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                }

                // Add a filter for CountryCode, if provided
                if (!string.IsNullOrEmpty(CountryCode))
                {
                    cmd.CommandText += " AND CountryCode = @CountryCode";
                    cmd.Parameters.AddWithValue("@CountryCode", CountryCode);
                }

                // Execute the query and return the total count
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public bool IsManageStateTaken(string state)
        {
            bool isTaken = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckManageStateExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@State", state);
                    conn.Open();
                    int result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        isTaken = true;
                    }
                }
            }
            return isTaken;
        }

        public bool IsManageStateCodeTaken(int statecode)
        {
            bool isTaken = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckManageStateCodeExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StateCode", statecode);
                    conn.Open();
                    int result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        isTaken = true;
                    }
                }
            }
            return isTaken;
        }

        public async Task<string> DeleteManageState(int stateID)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@UserID", DbType = DbType.Int32, Value = 1 },
                new SqlParameter { ParameterName = "@StateID", DbType = DbType.Int32, Value = stateID },
                new SqlParameter { ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
                new SqlParameter { ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000 }
            };
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("DeleteManageState", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }
                }
                #endregion
                return "State deleted successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }
        #endregion

        #region City
        public async Task<ManageCity> SaveManageCity(ManageCity manageCity)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
            new SqlParameter{ParameterName = "@StateCode", DbType = DbType.String, Value = manageCity.StateCode},
            new SqlParameter{ParameterName = "@City", DbType = DbType.String, Value = manageCity.City},
            new SqlParameter{ParameterName = "@UserID", DbType = DbType.Int32, Value = 1},
            new SqlParameter{ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
            new SqlParameter{ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000}
            };
            if (!string.IsNullOrEmpty(Convert.ToString(manageCity.CityID)))
            {
                param.Add(new SqlParameter { ParameterName = "@CityID", DbType = DbType.Int32, Value = manageCity.CityID });
            }
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SaveManageCity", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();

                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }

                    int id = cmd.Parameters["@ID"].Value as int? ?? 0;
                    manageCity.CityID = id;
                }
                #endregion
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions more specifically
                throw new Exception($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Catch all other exceptions
                throw new Exception($"Error: {ex.Message}");
            }
            return manageCity;
        }

        public async Task<List<ManageCity>> GetManageCity(int CityID, int? pageNumber, int? pageSize, string searchQuery, int? StateCode)
        {
            #region Declaration
            DataTable dt = new DataTable();
            SqlConnection con = null;
            List<ManageCity> manageCities = null;
            List<SqlParameter> param = new List<SqlParameter>();

            if (CityID != 0)
            {
                param.Add(new SqlParameter { ParameterName = "@CityID", DbType = DbType.Int32, Value = CityID });
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                param.Add(new SqlParameter { ParameterName = "@SearchQuery", DbType = DbType.String, Value = searchQuery });
            }
            if (StateCode.HasValue)
            {
                param.Add(new SqlParameter { ParameterName = "@StateCode", DbType = DbType.Int32, Value = StateCode.Value });
            }
            #endregion

            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("ShowManageCity", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                #endregion

                #region Wrap data
                if (dt.Rows.Count > 0)
                {
                    manageCities = new List<ManageCity>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ManageCity manageCity = new ManageCity
                        {
                            CityID = row["CityID"] as int? ?? 0,
                            StateCode = row["StateCode"] as int? ?? 0,
                            State = row["State"] as string ?? string.Empty,
                            City = row["City"] as string ?? string.Empty,
                            UserID = row["UserID"] as int? ?? 0
                        };
                        manageCities.Add(manageCity);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return manageCities;
        }

        public async Task<int> GetTotalCityCount(string searchQuery, int? StateCode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                // Prepare the SQL query with a LEFT JOIN on the ManageState table
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM ManageCity MC
                    LEFT JOIN ManageState MS ON MC.StateCode = MS.StateCode
                    WHERE MC.IsActive = 1", con);

                // If there is a search query, include it in the WHERE clause
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.CommandText += @"
                    AND (
                        MS.State LIKE @SearchQuery OR 
                        MC.City LIKE @SearchQuery OR 
                        CAST(MC.CityID AS VARCHAR) LIKE @SearchQuery)";
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                }

                // If there is a StateCode, include it in the WHERE clause
                if (StateCode.HasValue)
                {
                    cmd.CommandText += " AND MC.StateCode = @StateCode";
                    cmd.Parameters.AddWithValue("@StateCode", StateCode.Value);
                }

                // Execute the query and return the total count
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<string> DeleteManageCity(int cityID)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@UserID", DbType = DbType.Int32, Value = 1 },
                new SqlParameter { ParameterName = "@CityID", DbType = DbType.Int32, Value = cityID },
                new SqlParameter { ParameterName = "@ID", DbType = DbType.Int32, Direction = ParameterDirection.Output},
                new SqlParameter { ParameterName = "@OutputMessage", DbType = DbType.String, Direction = ParameterDirection.Output, Size = 2000 }
            };
            #endregion
            try
            {
                #region Interacting with database
                using (con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("DeleteManageCity", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddRange(param.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                    string outputMessage = cmd.Parameters["@OutputMessage"].Value.ToString();
                    if (!string.Equals(outputMessage, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(outputMessage);
                    }
                }
                #endregion
                return "City deleted successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }
        #endregion
    }
}