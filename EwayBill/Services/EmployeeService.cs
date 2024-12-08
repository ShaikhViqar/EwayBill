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
    public class EmployeeService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["EwayBillDbContext"].ConnectionString;

        public async Task<int> InsertEmployeeDataAsync(Employee employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("usp_InsertEmployee", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@LastName", employee.LastName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Address", employee.Address ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@City", employee.City ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Password", employee.Password ?? (object)DBNull.Value);

                            var employeeIdParameter = new SqlParameter("@EmployeeId", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(employeeIdParameter);

                            await cmd.ExecuteNonQueryAsync();

                            var employeeId = (int)employeeIdParameter.Value;

                            foreach (var childFile in employee.ChildFiles)
                            {
                                using (var itemCmd = new SqlCommand("usp_InsertChildFile", conn, transaction))
                                {
                                    itemCmd.CommandType = CommandType.StoredProcedure;
                                    itemCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                    itemCmd.Parameters.AddWithValue("@FileName", childFile.FileName ?? (object)DBNull.Value);

                                    await itemCmd.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();
                            return employeeId;
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

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var employee = new Employee();
                using (var cmd = new SqlCommand("usp_GetEmployeeById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee.EmployeeID = reader.GetInt32(0);
                            employee.FirstName = reader.GetString(1);
                            employee.LastName = reader.GetString(2);
                            employee.EmployeeName = reader.GetString(3);
                            employee.Address = reader.GetString(4);
                            employee.City = reader.GetString(5);
                            employee.Password = reader.GetString(6);
                        }
                    }
                }

                // Fetch child files
                using (var cmd = new SqlCommand("usp_GetChildFilesByEmployeeId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var childFile = new ChildFile
                            {
                                FileID = reader.GetInt32(0),
                                FileName = reader.GetString(1),
                                EmployeeId = employeeId
                            };
                            employee.ChildFiles.Add(childFile);
                        }
                    }
                }

                return employee;
            }
        }
    }
}