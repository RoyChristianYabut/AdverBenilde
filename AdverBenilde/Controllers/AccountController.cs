using AdverBenilde.App_Code;
using AdverBenilde.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdverBenilde.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// Checks if the email address is already existin
        /// from the users table
        /// </summary>
        /// <param name="email">User input email</param>
        /// <returns>Existing or not existing record</returns>
        public bool IsExisting(string email)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT Email FROM Users
                    WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    return cmd.ExecuteScalar() == null ? false : true;
                }
            }
        }

        [HttpPost]
        public ActionResult Register(UserModel record)
        {
            if (IsExisting(record.Email))
            {
                ViewBag.Message =
                    "<div class='alert alert-danger col-lg-6'>Email already existing.</div>";
                return View(record);
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Helper.GetCon()))
                {
                    con.Open();
                    string query = @"INSERT INTO Users 
                    (TypeID, Email, Password, FirstName, LastName, Phone
                    Status, DateAdded) VALUES
                    (@TypeID, @Email, @Password, @FirstName,
                    @LastName, @Phone, @Status, @DateAdded)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@TypeID", 3);
                        cmd.Parameters.AddWithValue("@Email", record.Email);
                        cmd.Parameters.AddWithValue("@Password", Helper.Hash(record.Password));
                        cmd.Parameters.AddWithValue("@FirstName", record.FN);
                        cmd.Parameters.AddWithValue("@LastName", record.LN);
                        cmd.Parameters.AddWithValue("@Phone", record.Phone);
                        cmd.Parameters.AddWithValue("@Status", "Pending");
                        cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                        cmd.ExecuteNonQuery();

                        string message = "Hello, " + record.FN + " " + record.LN + "!<br/>" +
                            "You have created an account successfully from our website.<br/><br/>" +
                            "Here are you credentials:<br/>" +
                            "Email Address: <strong>" + record.Email + "</strong><br/>" +
                            "Password: <strong>" + record.Password + "</strong><br/><br/>" +
                            "Thank you. 🙂<br/><br/>" +
                            "<h3>The Administrator</h3>";
                        Helper.SendEmail(record.Email, "Account Activation", message);
                        return RedirectToAction("Login", "Main");
                    }
                }
            }

        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
    }
}