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
                    WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", record.Email);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            if (string.Compare(Helper.Hash(record.Password), data["Password"].ToString()) == 0)
                            {
                                Session["UserID"] = data["UserID"].ToString();
                                Session["UserName"] = data["FirstName"].ToString() + " " + data["LastName"].ToString();
                                Session["Email"] = data["Email"].ToString();
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
            if (Session["UserID"] != null)
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
