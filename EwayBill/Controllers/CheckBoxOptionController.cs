using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EwayBill.Models;

namespace EwayBill.Controllers
{
    public class CheckBoxOptionController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["EwayBillDbContext"].ConnectionString;

        public ActionResult Index()
        {
            var checkBoxItems = GetCheckBoxOptions();
            return View(checkBoxItems);
        }

        // Function to fetch checkbox options from the database
        private List<CheckBoxOption> GetCheckBoxOptions()
        {
            var options = new List<CheckBoxOption>();

            using (SqlConnection conn = new SqlConnection(_connectionString)) // Replace with your connection string
            {
                SqlCommand cmd = new SqlCommand("GetCheckBoxOptions", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        options.Add(new CheckBoxOption
                        {
                            Id = (int)reader["Id"],
                            OptionName = (string)reader["OptionName"],
                            IsChecked = (bool)reader["IsChecked"]
                        });
                    }
                }
            }

            return options;
        }

        // POST: CheckBoxOption/Submit
        [HttpPost]
        public ActionResult Submit(List<int> selectedIds)
        {
            // Get all options to update
            var allOptions = GetCheckBoxOptions();

            // Check if selectedIds is null; if so, initialize it to an empty list
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            // Update the IsChecked property based on user selection
            foreach (var option in allOptions)
            {
                // Check if the option is selected
                bool isChecked = selectedIds.Contains(option.Id);
                UpdateCheckBoxOption(option.Id, isChecked);
            }

            // Redirect back to Index
            return RedirectToAction("Index");
        }

        // Function to update the checkbox option in the database
        private void UpdateCheckBoxOption(int id, bool isChecked)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateCheckBoxOptions", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@IsChecked", isChecked);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}