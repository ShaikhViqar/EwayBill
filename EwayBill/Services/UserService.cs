using EwayBill.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EwayBill.Services
{
    public class UserService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["EwayBillDbContext"].ConnectionString;

        public User ValidateUser(Login login)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ValidateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", login.UserName);
                    //cmd.Parameters.AddWithValue("@Password", login.Password); // Use hashing in production!

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = (int)reader["UserID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Role = reader["Role"].ToString(),
                                Password = reader["Password"].ToString(),
                            };
                        }
                    }
                }
            }

            return user;
        }

        //public bool RegisterUser(User user)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_connectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("RegisterUser", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                // Add parameters to match your User table structure
        //                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
        //                cmd.Parameters.AddWithValue("@LastName", user.LastName);
        //                cmd.Parameters.AddWithValue("@UserName", user.UserName);
        //                cmd.Parameters.AddWithValue("@Password", user.Password);
        //                cmd.Parameters.AddWithValue("@Email", user.Email);
        //                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Role", user.Role ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Gender", user.Gender ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@City", user.City ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@State", user.State ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@PostalCode", user.PostalCode ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Country", user.Country ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Hobbies", user.Hobbies ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@FileName", user.FileName ?? (object)DBNull.Value);

        //                // Add optional UserID parameter
        //                cmd.Parameters.AddWithValue("@UserID", user.UserID > 0 ? (object)user.UserID : DBNull.Value);

        //                // Output parameter to get the inserted ID
        //                var requestIdParameter = new SqlParameter("@Id", SqlDbType.Int)
        //                {
        //                    Direction = ParameterDirection.Output
        //                };
        //                cmd.Parameters.Add(requestIdParameter);

        //                //conn.Open();
        //                //int result = cmd.ExecuteNonQuery();

        //                //// Retrieve the output parameter value
        //                //user.UserID = (int)requestIdParameter.Value;

        //                //return result > 0; // Return true if the insert was successful
        //                conn.Open();
        //                // Execute the command
        //                cmd.ExecuteNonQuery();

        //                // Retrieve the return value
        //                int returnValue = (int)cmd.Parameters["@Id"].Value;

        //                // Retrieve the output parameter value
        //                user.UserID = (int)requestIdParameter.Value;

        //                // Check the return value
        //                return returnValue > 0; // Return true if the insert or update was successful
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // Log the exception (not shown for brevity)
        //        return false;
        //    }
        //}

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("RegisterUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to match your User table structure
                        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", user.LastName);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Role", user.Role ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gender", user.Gender ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", user.City ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@State", user.State ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PostalCode", user.PostalCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Country", user.Country ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Hobbies", user.Hobbies ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FileName", user.FileName ?? (object)DBNull.Value);

                        // Add optional UserID parameter
                        cmd.Parameters.AddWithValue("@UserID", user.UserID > 0 ? (object)user.UserID : DBNull.Value);

                        // Output parameter to get the inserted ID
                        var requestIdParameter = new SqlParameter("@Id", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(requestIdParameter);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        // Retrieve the output parameter value
                        user.UserID = (int)requestIdParameter.Value;

                        // Check if user registration was successful
                        if (user.UserID > 0)
                        {
                            // Insert child file names
                            if (user.ChildFileNames != null && user.ChildFileNames.Count > 0)
                            {
                                for (int i = 0; i < user.ChildFileNames.Count; i++)
                                {
                                    using (SqlCommand childFileCmd = new SqlCommand("RegisterUserChildFiles", conn))
                                    {
                                        childFileCmd.CommandType = CommandType.StoredProcedure;

                                        // Add parameters for child files
                                        childFileCmd.Parameters.AddWithValue("@FileID", user.ChildFileNames[i].FileID);
                                        childFileCmd.Parameters.AddWithValue("@UserID", user.UserID);
                                        childFileCmd.Parameters.AddWithValue("@FileName", user.ChildFileNames[i].FileName);

                                        // Output parameter to get the inserted ID
                                        var childFileIdParameter = new SqlParameter("@Id", SqlDbType.Int)
                                        {
                                            Direction = ParameterDirection.Output
                                        };
                                        childFileCmd.Parameters.Add(childFileIdParameter);

                                        // Add IsFirstInsert parameter: 1 for the first insert, 0 for subsequent inserts
                                        childFileCmd.Parameters.AddWithValue("@IsFirstInsert", i == 0 ? 1 : 0);

                                        // Execute the command asynchronously
                                        await childFileCmd.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                            return true; // Return true if user registration and child files insertions were successful
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown for brevity)
                throw new Exception($"Error registering user: {ex.Message}");
            }

            return false; // Return false if registration failed
        }

        public bool RegisterUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("RegisterUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to match your User table structure
                        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", user.LastName);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Role", user.Role ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gender", user.Gender ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", user.City ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@State", user.State ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PostalCode", user.PostalCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Country", user.Country ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Hobbies", user.Hobbies ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FileName", user.FileName ?? (object)DBNull.Value);

                        // Add optional UserID parameter
                        cmd.Parameters.AddWithValue("@UserID", user.UserID > 0 ? (object)user.UserID : DBNull.Value);

                        // Output parameter to get the inserted ID
                        var requestIdParameter = new SqlParameter("@Id", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(requestIdParameter);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Retrieve the output parameter value
                        user.UserID = (int)requestIdParameter.Value;

                        // Check if user registration was successful
                        if (user.UserID > 0)
                        {
                            // Insert child file names
                            if (user.ChildFileNames != null && user.ChildFileNames.Count > 0)
                            {
                                foreach (var childFileName in user.ChildFileNames)
                                {
                                    using (SqlCommand childFileCmd = new SqlCommand("RegisterUserChildFiles", conn))
                                    {
                                        childFileCmd.CommandType = CommandType.StoredProcedure;

                                        // Add parameters for child files
                                        childFileCmd.Parameters.AddWithValue("@UserID", user.UserID);
                                        childFileCmd.Parameters.AddWithValue("@FileName", childFileName.FileName);

                                        // Output parameter to get the inserted ID
                                        var childFileIdParameter = new SqlParameter("@Id", SqlDbType.Int)
                                        {
                                            Direction = ParameterDirection.Output
                                        };
                                        childFileCmd.Parameters.Add(childFileIdParameter);

                                        // Execute the command
                                        childFileCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            return true; // Return true if user registration and child files insertions were successful
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log the exception (not shown for brevity)
                return false;
            }

            return false; // Return false if registration failed
        }

        public List<User> GetUserById(int? userId, int page, int pageSize, string searchQuery)
        {
            //User user = null;
            List<User> users = new List<User>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId); // Assuming the stored procedure takes UserID as a parameter
                    cmd.Parameters.AddWithValue("@PageNumber", page); // Assuming the stored procedure takes UserID as a parameter
                    cmd.Parameters.AddWithValue("@PageSize", pageSize); // Assuming the stored procedure takes UserID as a parameter
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //if (reader.Read())
                        while (reader.Read())
                        {
                            //user = new User
                            User user = new User
                            {
                                UserID = (int)reader["UserID"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Role = reader["Role"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = reader["DateOfBirth"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                Country = reader["Country"].ToString(),
                                Hobbies = reader["Hobbies"].ToString(),
                                FileName = reader["FileName"].ToString()
                            };
                            user.ChildFileNames = GetUserChildFilesById(user.UserID);
                            users.Add(user);
                        }
                    }
                }
            }

            //return user;
            return users;
        }

        public List<UserChildFiles> GetUserChildFilesById(int? userId)
        {
            List<UserChildFiles> userChildFiles = new List<UserChildFiles>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserChildFilesById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserChildFiles userChildFile = new UserChildFiles
                            {
                                FileID = (int)reader["FileID"],
                                UserID = (int)reader["UserID"],
                                FileName = reader["FileName"].ToString()
                            };
                            userChildFiles.Add(userChildFile);
                        }
                    }
                }
            }
            return userChildFiles;
        }

        public bool IsUsernameTaken(string username)
        {
            bool isTaken = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckUsernameExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", username);

                    conn.Open();
                    int result = (int)cmd.ExecuteScalar(); // Get the count from the stored procedure

                    if (result > 0)
                    {
                        isTaken = true; // Username is already taken
                    }
                }
            }

            return isTaken;
        }

        public async Task<int> GetTotalUsersCount(string searchQuery)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users where IsActive = 1", con); // Adjust to your actual roles table
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.CommandText += " AND (FirstName LIKE @SearchQuery OR LastName LIKE @SearchQuery OR Email LIKE @SearchQuery OR Role LIKE @SearchQuery OR CAST(UserID AS VARCHAR) LIKE @SearchQuery)";
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                }
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<string> DeleteManageUsers(int userId)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@UserID", DbType = DbType.Int32, Value = userId },
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
                    SqlCommand cmd = new SqlCommand("DeleteManageUsers", con)
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
                return "User deleted successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }

        public async Task<bool> RemoveChildFile(int? fileID)
        {
            // Check if the FileID is null and return false if it is
            if (fileID == null)
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                List<SqlParameter> param = new List<SqlParameter>
        {
            new SqlParameter { ParameterName = "@FileID", DbType = DbType.Int32, Value = fileID }
        };

                try
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("DeleteChildFile", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(param.ToArray());

                        // Execute the command and check the number of affected rows
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0; // Return true if any row was deleted
                    }
                }
                catch (Exception ex)
                {
                    // Optionally log the exception here
                    throw new Exception($"Error deleting child file: {ex.Message}");
                }
            }
        }

        public async Task<string> DeleteExistingChildFiles(int userId)
        {
            #region Declaration
            SqlConnection con = null;
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@UserID", DbType = DbType.Int32, Value = userId },
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
                    SqlCommand cmd = new SqlCommand("DeleteExistingChildFiles", con)
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
                return "User deleted successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }
    }
}