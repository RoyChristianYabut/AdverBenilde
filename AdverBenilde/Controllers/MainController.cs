using AdverBenilde.App_Code;
using AdverBenilde.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace AdverBenilde.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Login()
        {
            var record = new UserModel();
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel record)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT UserID, FirstName, LastName, Email, Password FROM Users
                    WHERE Email = @Email and Password=@Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", record.Email);
                    cmd.Parameters.AddWithValue("@Password", Helper.Hash(record.Password));
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            if (data.HasRows)
                            {
                                Session["userid"] = data["UserID"].ToString();
                                Session["username"] = data["FirstName"].ToString() + " " + data["LastName"].ToString();
                                Session["email"] = data["Email"].ToString();
                            }
                            else
                            {

                            }

                        }

                    }
                    return RedirectToAction("UserDashBoard");
                }
            }

        }




        public ActionResult UserDashBoard()
        {
            if (Session["userid"] != null)
            {
                return RedirectToAction("Index", "Event");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
