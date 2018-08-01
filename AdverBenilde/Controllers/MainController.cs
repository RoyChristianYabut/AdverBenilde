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

        public ActionResult MyProfile()
        {
            Helper.ValidateLogin();
            var record = new UserModel();
            using (SqlConnection con= new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT Email, Password, FirstName, Lastname, Phone From Users WHERE UserID=@UserID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                record.Email = sdr["Email"].ToString();
                                record.FN = sdr["FirstName"].ToString();
                                record.LN = sdr["LastName"].ToString();
                                record.Phone = sdr["Phone"].ToString();
                            }
                            return View(record);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Event");
                        }
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult MyProfile(UserModel record)
        {
            using (SqlConnection con=new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"UPDATE Users SET Email=@Email, 
                                Password=@Password, FirstName=@FirstName, Lastname=@LastName,
                                Phone=@Phone, Status=@Status, DateModified=@DateModified
                                WHERE UserID=@UserID";
                using (SqlCommand cmd=new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", record.Email);
                    cmd.Parameters.AddWithValue("@Password", Helper.Hash(record.Password));
                    cmd.Parameters.AddWithValue("@FirstName", record.FN);
                    cmd.Parameters.AddWithValue("@LastName", record.LN);
                    cmd.Parameters.AddWithValue("@Phone", record.Phone);
                    cmd.Parameters.AddWithValue("@Status", "Updated");
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    cmd.ExecuteNonQuery();
                    ViewBag.Message = "<div class='alert alert-success col-lg-6'>Profile Updated</div>";
                    return View(record);
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
