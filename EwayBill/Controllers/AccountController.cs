using EwayBill.Models;
using EwayBill.Services;
using EwayBill.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EwayBill.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService = new UserService();
        private readonly GenerateToken _generateToken = new GenerateToken();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Login(Login login)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User user = _userService.ValidateUser(login);
        //        if (user != null && HashPassword.VerifyPassword(login.Password, user.Password))
        //        {
        //            Session["UserID"] = user.UserID;
        //            Session["UserName"] = user.UserName;

        //            // Set cookies for client-side storage (optional)
        //            HttpCookie userIdCookie = new HttpCookie("UserID", user.UserID.ToString());
        //            HttpCookie userNameCookie = new HttpCookie("UserName", user.UserName);
        //            userIdCookie.Expires = DateTime.Now.AddHours(1); // Cookie expiration time
        //            userNameCookie.Expires = DateTime.Now.AddHours(1);

        //            Response.Cookies.Add(userIdCookie);
        //            Response.Cookies.Add(userNameCookie);

        //            // Clear the existing AuthToken cookie
        //            if (Request.Cookies["AuthToken"] != null)
        //            {
        //                var cookie = new HttpCookie("AuthToken")
        //                {
        //                    Expires = DateTime.Now.AddDays(-1) // Expire the cookie
        //                };
        //                Response.Cookies.Add(cookie);
        //            }

        //            string token = _generateToken.GenerateJwtToken(user);
        //            HttpCookie authCookie = new HttpCookie("AuthToken", token);
        //            authCookie.HttpOnly = true; // Optional: prevents client-side access
        //            Response.Cookies.Add(authCookie);

        //            return Redirect("/Admin/dist/pages/index.html");

        //            //return RedirectToAction("Index", "EwayBill");
        //        }
        //        ModelState.AddModelError("", "Invalid username or password.");
        //    }
        //    return View(login);
        //}

        [HttpPost]
        public JsonResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                User user = _userService.ValidateUser(login);
                //if (user != null && HashPassword.VerifyPassword(login.Password, user.Password))
                if (user != null)
                {
                    //string encryptionKey = EncryptionUtility.GenerateEncryptionKey();
                    // Decrypt the stored password
                    string decryptedPassword = EncryptionUtility.Decrypt(user.Password);

                    if (login.Password == decryptedPassword)
                    {
                        // Set session and cookies
                        Session["UserID"] = user.UserID;
                        Session["UserName"] = user.UserName;

                        // Set cookies for client-side storage (optional)
                        HttpCookie userIdCookie = new HttpCookie("UserID", user.UserID.ToString());
                        HttpCookie userNameCookie = new HttpCookie("UserName", user.UserName);
                        userIdCookie.Expires = DateTime.Now.AddHours(1);
                        userNameCookie.Expires = DateTime.Now.AddHours(1);

                        Response.Cookies.Add(userIdCookie);
                        Response.Cookies.Add(userNameCookie);

                        // Clear the existing AuthToken cookie
                        if (Request.Cookies["AuthToken"] != null)
                        {
                            var cookie = new HttpCookie("AuthToken")
                            {
                                Expires = DateTime.Now.AddDays(-1)
                            };
                            Response.Cookies.Add(cookie);
                        }

                        string token = _generateToken.GenerateJwtToken(user);
                        HttpCookie authCookie = new HttpCookie("AuthToken", token);
                        authCookie.HttpOnly = true;
                        Response.Cookies.Add(authCookie);

                        // Return JSON response with redirect URL
                        return Json(new { Redirect = "/Admin/dist/pages/index.html" });
                    }
                    else
                    {
                        return Json(new { message = "Invalid username or password." });
                    }
                }
                else
                {
                    return Json(new { message = "Invalid username or password." });
                }
            }

            return Json(new { message = "Invalid data." });
        }

        //public ActionResult Login(Login login = null)
        //{
        //    if (Request.HttpMethod == "GET")
        //    {
        //        // Handle GET request: display the login form
        //        return View();
        //    }
        //    else if (Request.HttpMethod == "POST")
        //    {
        //        // Handle POST request: process the login form
        //        if (ModelState.IsValid)
        //        {
        //            User user = _userService.ValidateUser(login);
        //            if (user != null)
        //            {
        //                // Successful login logic
        //                Session["UserID"] = user.UserID;
        //                Session["UserName"] = user.UserName;

        //                // Set cookies for client-side storage (optional)
        //                HttpCookie userIdCookie = new HttpCookie("UserID", user.UserID.ToString())
        //                {
        //                    Expires = DateTime.Now.AddHours(1)
        //                };
        //                HttpCookie userNameCookie = new HttpCookie("UserName", user.UserName)
        //                {
        //                    Expires = DateTime.Now.AddHours(1)
        //                };

        //                Response.Cookies.Add(userIdCookie);
        //                Response.Cookies.Add(userNameCookie);

        //                // Clear the existing AuthToken cookie
        //                if (Request.Cookies["AuthToken"] != null)
        //                {
        //                    var cookie = new HttpCookie("AuthToken")
        //                    {
        //                        Expires = DateTime.Now.AddDays(-1)
        //                    };
        //                    Response.Cookies.Add(cookie);
        //                }

        //                string token = _generateToken.GenerateJwtToken(user);
        //                HttpCookie authCookie = new HttpCookie("AuthToken", token)
        //                {
        //                    HttpOnly = true
        //                };
        //                Response.Cookies.Add(authCookie);

        //                return RedirectToAction("Index", "EwayBill");
        //            }
        //            ModelState.AddModelError("", "Invalid username or password.");
        //        }
        //    }

        //    // If we reach here, it means the login failed or it was a GET request.
        //    return View(login);
        //}

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //[HttpPost]
        //public JsonResult Register(User user, HttpPostedFileBase userFileUpload, HttpPostedFileBase[] childFileUploads, string previousFileName, List<UserChildFiles> existingChildFiles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //// Hash the password
        //        //user.Password = HashPassword.HashPasswords(user.Password);

        //        // Encrypt the password instead of hashing
        //        user.Password = EncryptionUtility.Encrypt(user.Password);

        //        // Process file upload
        //        if (userFileUpload != null && userFileUpload.ContentLength > 0)
        //        {
        //            // Save the new file if provided
        //            string fileName = Path.GetFileName(userFileUpload.FileName);  // Get the file name
        //            string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
        //            userFileUpload.SaveAs(path);
        //            user.FileName = fileName; // Set the file name in the User object
        //        }
        //        else if (!string.IsNullOrEmpty(previousFileName))
        //        {
        //            // If no new file was uploaded, check if a previous file name exists
        //            user.FileName = previousFileName; // Use the previous file name
        //        }

        //        // Remove existing child files from the database
        //        if (existingChildFiles != null && existingChildFiles.Count > 0)
        //        {
        //            foreach (var existingFile in existingChildFiles)
        //            {
        //                bool isRemoved = _userService.RemoveChildFile(existingFile.FileID).GetAwaiter().GetResult();
        //                if (isRemoved)
        //                {
        //                    // Optionally, you can delete the file from the server as well
        //                    string filePath = Path.Combine(Server.MapPath("~/Uploads"), existingFile.FileName);
        //                    if (System.IO.File.Exists(filePath))
        //                    {
        //                        System.IO.File.Delete(filePath); // Delete the file
        //                    }
        //                }
        //            }
        //        }

        //        // Process child file uploads
        //        if (childFileUploads != null && childFileUploads.Length > 0)
        //        {
        //            foreach (var file in childFileUploads)
        //            {
        //                if (file != null && file.ContentLength > 0)
        //                {
        //                    // Save each child file
        //                    string childFileName = Path.GetFileName(file.FileName);
        //                    string childFilePath = Path.Combine(Server.MapPath("~/Uploads"), childFileName);
        //                    file.SaveAs(childFilePath);
        //                    //user.ChildFileNames.Add(childFileName); // Add to the list of child file names

        //                    UserChildFiles childFile = new UserChildFiles
        //                    {
        //                        FileName = childFileName
        //                    };
        //                    user.ChildFileNames.Add(childFile);
        //                }
        //            }
        //        }


        //        bool isUserRegistered = _userService.RegisterUser(user);
        //        if (isUserRegistered)
        //        {
        //            //string redirectUrl = Url.Action("RegistrationSuccess", "Account", new { userId = user.UserID });
        //            //string redirectUrl = Url.Action("ManageUsers", "Account");
        //            string redirectUrl = Url.Content("~/Admin/dist/pages/Users/ManageUsers.html");
        //            return Json(new { redirectUrl = redirectUrl });
        //        }
        //        else
        //        {
        //            return Json(new { message = "Registration failed." });
        //        }
        //    }
        //    return Json(new { success = false, message = "Invalid data." });
        //}

        [HttpPost]
        public async Task<JsonResult> Register(User user, HttpPostedFileBase userFileUpload, HttpPostedFileBase[] childFileUploads, string previousFileName, List<UserChildFiles> existingChildFiles)
        {
            if (ModelState.IsValid)
            {
                // Encrypt the password instead of hashing
                user.Password = EncryptionUtility.Encrypt(user.Password);

                // Process file upload
                if (userFileUpload != null && userFileUpload.ContentLength > 0)
                {
                    // Save the new file if provided
                    //string fileName = Path.GetFileName(userFileUpload.FileName);  // Get the file name

                    string extension = Path.GetExtension(userFileUpload.FileName);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(userFileUpload.FileName);
                    string fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + extension;


                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    userFileUpload.SaveAs(path);
                    user.FileName = fileName; // Set the file name in the User object
                }
                else if (!string.IsNullOrEmpty(previousFileName))
                {
                    // If no new file was uploaded, check if a previous file name exists
                    user.FileName = previousFileName; // Use the previous file name
                }

                //// Remove existing child files from the database
                //if (existingChildFiles != null && existingChildFiles.Count > 0)
                //{
                //    foreach (var existingFile in existingChildFiles)
                //    {
                //        bool isRemoved = await _userService.RemoveChildFile(existingFile.FileID); // Asynchronous call
                //        if (isRemoved)
                //        {
                //            // Delete the file from the server
                //            string filePath = Path.Combine(Server.MapPath("~/Uploads"), existingFile.FileName);
                //            if (System.IO.File.Exists(filePath))
                //            {
                //                System.IO.File.Delete(filePath); // Delete the file
                //            }
                //        }
                //    }
                //}

                //// Process child file uploads
                //if (childFileUploads != null && childFileUploads.Length > 0)
                //{
                //    for (int i = 0; i < childFileUploads.Length; i++)
                //    {
                //        var file = childFileUploads[i];
                //        if (file != null && file.ContentLength > 0)
                //        {
                //            // Save each child file
                //            string childFileName = Path.GetFileName(file.FileName);
                //            string childFilePath = Path.Combine(Server.MapPath("~/Uploads"), childFileName);
                //            file.SaveAs(childFilePath);

                //            // Create a new UserChildFiles object and add to the user's child files
                //            UserChildFiles childFile = new UserChildFiles
                //            {
                //                FileName = childFileName
                //            };
                //            user.ChildFileNames.Add(childFile);
                //        }
                //    }
                //}

                if(existingChildFiles == null)
                {
                    await _userService.DeleteExistingChildFiles(user.UserID);
                }

                // Add existing child files (pass FileID, FileName, and UserID)
                if (existingChildFiles != null && existingChildFiles.Count > 0)
                {
                    foreach (var existingFile in existingChildFiles)
                    {
                        // Pass FileID, FileName, and UserID for existing files
                        UserChildFiles childFile = new UserChildFiles
                        {
                            FileID = existingFile.FileID,     // Keep existing FileID
                            FileName = existingFile.FileName, // Keep existing FileName
                            UserID = user.UserID              // Assign the UserID
                        };

                        // Add the existing file to user's child file list
                        user.ChildFileNames.Add(childFile);
                    }
                }

                // Process child file uploads (new files)
                if (childFileUploads != null && childFileUploads.Length > 0)
                {
                    foreach (var file in childFileUploads)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            // Save each child file
                            //string childFileName = Path.GetFileName(file.FileName);

                            string extension = Path.GetExtension(file.FileName);
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                            string childFileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + extension;


                            string childFilePath = Path.Combine(Server.MapPath("~/Uploads"), childFileName);
                            file.SaveAs(childFilePath);

                            // Create a new UserChildFiles object for new files
                            UserChildFiles childFile = new UserChildFiles
                            {
                                FileName = childFileName, // Set the FileName for new uploads
                                FileID = 0 // New file, FileID is 0 or handled as per your logic
                            };

                            user.ChildFileNames.Add(childFile); // Add new file to user's child file list
                        }
                    }
                }

                // Register the user
                bool isUserRegistered = await _userService.RegisterUserAsync(user);
                if (isUserRegistered)
                {
                    string redirectUrl = Url.Content("~/Admin/dist/pages/Users/ManageUsers.html");
                    return Json(new { redirectUrl = redirectUrl });
                }
                else
                {
                    return Json(new { message = "Registration failed." });
                }
            }

            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpGet]
        public ActionResult RegistrationSuccess(int? userId, int page = 1, int pageSize = 5, string searchQuery = null)
        {
            // Fetch user data based on userId
            //User registeredUser = _userService.GetUserById(userId);
            List<User> users = _userService.GetUserById(userId, page, pageSize, searchQuery);

            if (users == null)
            {
                // Handle case where user is not found, redirect or show an error message
                return RedirectToAction("Register"); // Or return an error view
            }

            return View(users); // Pass the user object to the view
        }

        [HttpGet]
        public JsonResult CheckUsername(string username)
        {
            bool exists = _userService.IsUsernameTaken(username);
            return Json(!exists, JsonRequestBehavior.AllowGet); // Returns true if the username is available
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<JsonResult> ManageUsers(int? userId, int page = 1, int pageSize = 5, string searchQuery = null)
        {
            try
            {
                List<User> users = _userService.GetUserById(userId, page, pageSize, searchQuery);
                // Decrypt passwords for each user
                foreach (var user in users)
                {
                    user.Password = EncryptionUtility.Decrypt(user.Password);
                }
                int totalItems = await _userService.GetTotalUsersCount(searchQuery);
                return Json(new { users, totalItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteManageUsers(int userId)
        {
            try
            {
                string message = await _userService.DeleteManageUsers(userId);
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(file.FileName));
                file.SaveAs(path);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}