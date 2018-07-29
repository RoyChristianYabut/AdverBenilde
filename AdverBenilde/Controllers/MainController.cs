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
                string query = @"SELECT Email, Password FROM Users
                    WHERE Email = @Email AND Password=@Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", record.Email);
                    cmd.Parameters.AddWithValue("@Password", record.Password);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            Session["UserID"] = data["UserID"].ToString();
                            Session["UserName"] = data["FirstName"].ToString() + " "+data["LastName"].ToString();
                            Session["Email"] = data["Email"].ToString();

                        }
                    }

                }
            }
            return RedirectToAction("UserDashBoard");
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
}