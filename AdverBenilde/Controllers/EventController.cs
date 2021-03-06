﻿using AdverBenilde.App_Code;
using AdverBenilde.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdverBenilde.Controllers
{
    public class EventController : Controller
    {
        public List<LocationModel> GetLocation()
        {
            var list = new List<LocationModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT l.LocationCode, c.Name AS CampusName, l.Name FROM Location l
                    INNER JOIN Campus c ON l.CampusID=c.CampusID
                    ORDER BY l.CampusID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new LocationModel
                            {
                                LocationCode = int.Parse(data["LocationCode"].ToString()),
                                CampusName = data["CampusName"].ToString(),
                                Name = data["CampusName"].ToString() + ", " + data["Name"].ToString()
                            });
                        }
                    }
                }
                con.Close();
            }
            return list;
        }

        public List<EventHandlerModel> GetHandler()
        {
            var list = new List<EventHandlerModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT EventHandlerID, Name FROM EventHandler 
                                 ORDER BY CASE WHEN Name='Other...' THEN 2 ELSE 1 END, Name";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new EventHandlerModel
                            {
                                EventHandlerID = int.Parse(data["EventHandlerID"].ToString()),
                                Name = data["Name"].ToString()
                            });
                        }
                    }
                }
                con.Close();
            }
            return list;
        }

        public List<AuthorModel> GetA()
        {
            var list = new List<AuthorModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.Recordno, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.orgID, e.roomID, e.eventDate 
                    FROM Authorship e
                    LEFT JOIN EventHandler eh on e.orgID=eh.EventHandlerID
                    INNER JOIN Location l on e.roomID=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new AuthorModel
                            {
                                RecordNo = int.Parse(data["Recordno"].ToString()),
                                Name = data["Name"].ToString(),
                                orgID = int.Parse(data["orgID"].ToString()),
                                org = data["HandlerName"].ToString(),
                                roomID = int.Parse(data["roomID"].ToString()),
                                room = data["LocationName"].ToString(),
                                EventDate = DateTime.Parse(data["eventDate"].ToString())

                            });

                        }
                    }
                }
                con.Close();
            }
            return list;
        }

        public ActionResult CreateAuthor()
        {
            var record = new AuthorModel();
            record.AllLocations = GetLocation();
            record.EventHandlers = GetHandler();
            return View(record);
        }


        [HttpPost]
        public ActionResult CreateAuthor(AuthorModel record, HttpPostedFileBase Image)
        {
            int eventid = 0;
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @" INSERT INTO Authorship
                              (Name, orgID, roomID, eventDate)
                              VALUES
                              (@Name, @orgID, @roomID, @eventDate);";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                        cmd.Parameters.AddWithValue("@Name", record.Name);
                        cmd.Parameters.AddWithValue("@orgID", record.orgID);
                        cmd.Parameters.AddWithValue("@roomID", record.roomID);
                        cmd.Parameters.AddWithValue("@eventDate", DateTime.Parse(record.EventDate.ToString()));
                        cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return RedirectToAction("IndexO");
        }


        public ActionResult IndexO()
        {

            var list = new AuthorModel();
            list.AllCampus = GetCampus();
            list.AllAuthor=GetA();
            return View(list);
        }




        public ActionResult Create()
        {
            Helper.ValidateLogin();
            var record = new EventsModel();
            record.AllLocations = GetLocation();
            record.EventHandlers = GetHandler();
            return View(record);
        }


      

        [HttpPost]
        public ActionResult Create(EventsModel record, HttpPostedFileBase Image)
        {
            int eventid = 0;
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @" INSERT INTO Events
                              (EventHandlerID, LocationCode, Name, Description, 
                               Image, Time, Status, DateAdded, IsGoing, Interested, NotGoing)
                              VALUES
                              (@EventHandlerID, @LocationCode, @Name, @Description, 
                              @Image, @Time,@Status,@DateAdded, @IsGoing, @Interested, @NotGoing);";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@EventHandlerID", record.EventHandlerID);
                        cmd.Parameters.AddWithValue("@LocationCode", record.LocationCode);
                        cmd.Parameters.AddWithValue("@Name", record.Name);
                        cmd.Parameters.AddWithValue("@Description", record.Description);
                        cmd.Parameters.AddWithValue("@Image",
                        DateTime.Now.ToString("yyyyMMddHHmmss-") + Image.FileName);
                        Image.SaveAs(Server.MapPath("~/Images/Events/" +
                        DateTime.Now.ToString("yyyyMMddHHmmss-") + Image.FileName));
                        cmd.Parameters.AddWithValue("@Time", record.Time);
                        cmd.Parameters.AddWithValue("@Status", "Pending");
                        cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                        cmd.Parameters.AddWithValue("@IsGoing", 0);
                        cmd.Parameters.AddWithValue("@Interested", 0);
                        cmd.Parameters.AddWithValue("@NotGoing", 0);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = "<div class='alert alert-danger col-lg-6'>Error: " + e.Message + "</div>";
                        return View();
                    }
                }
                con.Close();
            }
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT MAX(EventID) FROM Events";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    eventid = (int)cmd.ExecuteScalar();
                }
                con.Close();
            }
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT MAX(EventID) FROM Events;
                                INSERT INTO EventHost VALUES (@UserID, @EventID);";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    cmd.Parameters.AddWithValue("@EventID", eventid);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return RedirectToAction("Index");
        }

        public ActionResult CreateLocation()
        {
            var record = new LocationModel();
            record.Campuses = GetCampus();
            return View(record);
        }

        [HttpPost]
        public ActionResult CreateLocation(LocationModel record)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @" INSERT INTO [Location] 
                              (CampusID, Name)
                              VALUES
                              (@CampusID, @Name)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CampusID", record.CampusID);
                    cmd.Parameters.AddWithValue("@Name", record.Name);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return RedirectToAction("Create");
        }

        public ActionResult CreateHandler()
        {
            var record = new EventHandlerModel();
            return View(record);
        }

        [HttpPost]
        public ActionResult CreateHandler(EventHandlerModel record)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @" INSERT INTO [EventHandler] 
                              (Name)
                              VALUES
                              (@Name)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", record.Name);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return RedirectToAction("Create");
        }

        // GET: Products
        //public List<LocationModel> GetLocations()
        //{
        //    var list = new List<LocationModel>();
        //    using (SqlConnection con = new SqlConnection(Helper.GetCon()))
        //    {
        //        con.Open();
        //        string query = @"SELECT DISTINCT c.Name AS CampusName,
        //            (SELECT COUNT(e.CampusID) FROM Events e
        //            WHERE e.LocationCode = l.LocationCode) AS TotalCount
        //            FROM Location l
        //            INNER JOIN Campus c ON l.CampusID=c.CampusID
        //            WHERE NOT l.Name='Other...'
        //            ORDER BY c.Name DESC";
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            using (SqlDataReader data = cmd.ExecuteReader())
        //            {
        //                while (data.Read())
        //                {
        //                    list.Add(new LocationModel
        //                    {
        //                        LocationCode = int.Parse(data["LocationCode"].ToString()),
        //                        CampusName = data["CampusName"].ToString(),
        //                        Name = data["Name"].ToString(),
        //                        TotalCount = int.Parse(data["TotalCount"].ToString())
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return list;
        //}

        public List<CampusModel> GetCampus()
        {
            var list = new List<CampusModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT  c.CampusID, c.Name AS CampusName
                              FROM Campus c
                              WHERE NOT c.CampusID=4
					          Order By CampusID ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new CampusModel
                            {
                                CampusID = int.Parse(data["CampusID"].ToString()),
                                Name = data["CampusName"].ToString()
                            });
                        }
                    }
                }
                con.Close();
            }
            return list;
        }


        public List<EventsModel> GetEvent()
        {
            var list = new List<EventsModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    INNER JOIN Location l on e.LocationCode=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID
                    ORDER BY e.DateAdded";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new EventsModel
                            {
                                ID = int.Parse(data["EventID"].ToString()),
                                Name = data["Name"].ToString(),
                                EventHandlerName = data["HandlerName"].ToString(),
                                LocationName = data["LocationName"].ToString(),
                                Description = data["Description"].ToString(),
                                Image = data["Image"].ToString(),
                                Time = DateTime.Parse(data["Time"].ToString()),
                                DateAdded = DateTime.Parse(data["DateAdded"].ToString())

                            });

                        }
                    }
                }
                con.Close();
            }
            return list;
        }



        public List<EventsModel> GetEvent(string locatID)
        {
            var list = new List<EventsModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    INNER JOIN Location l on e.LocationCode=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID
                    WHERE c.CampusID=@CampusID
                    ORDER BY e.DateAdded";

                if (locatID == "100000")
                {
                    query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    INNER JOIN Location l on e.LocationCode=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID
                    ORDER BY e.Time";
                }

                if (locatID == "100001")
                {
                    query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    INNER JOIN Location l on e.LocationCode=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID
                    ORDER BY e.Time DESC";
                }
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!(locatID == "100000" || locatID == "100001"))
                    {
                        cmd.Parameters.AddWithValue("@CampusID", locatID);
                    }
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new EventsModel
                            {
                                ID = int.Parse(data["EventID"].ToString()),
                                Name = data["Name"].ToString(),
                                EventHandlerName = data["HandlerName"].ToString(),
                                LocationName = data["LocationName"].ToString(),
                                Description = data["Description"].ToString(),
                                Image = data["Image"].ToString(),
                                Time = DateTime.Parse(data["Time"].ToString()),
                                DateAdded = DateTime.Parse(data["DateAdded"].ToString())
                            });
                        }
                    }
                }
                con.Close();
            }
            return list;
        }

        public ActionResult Index()
        {
            Helper.ValidateLogin();
            var list = new EventsModel();
            list.AllCampus = GetCampus();
            if (Request.QueryString["c"] == null)
            {
                list.AllEvents = GetEvent();
            }
            else
            {
                list.AllEvents = GetEvent(Request.QueryString["c"].ToString());
            }

            return View(list);
        }

        public ActionResult Details(int? id)
        {
            Helper.ValidateLogin();
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var record = new EventsModel();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"Select e.EventID, h.Name AS EHName, l.Name AS LName, c.Name AS CName,
                            e.Name, e.Description, e.Image, e.Time, e.Status,
                            e.DateAdded, e.IsGoing, e.Interested, e.NotGoing
                            FROM Events e
                            LEFT JOIN EventHandler h ON e.EventHandlerID=h.EventHandlerID
                            LEFT JOIN Location l ON e.LocationCode=l.LocationCode
                            LEFT JOIN Campus c ON l.CampusID=c.CampusID
                            WHERE e.EventID=@EventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EventID", id);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        if (data.HasRows)
                        {
                            while (data.Read())
                            {
                                record.ID = int.Parse(data["EventID"].ToString());
                                record.EventHandlerName = data["EHName"].ToString();
                                record.LocationName = data["CName"].ToString() + ", " + data["LName"].ToString();
                                record.Name = data["Name"].ToString();
                                record.Description = data["Description"].ToString();
                                record.Image = data["Image"].ToString();
                                record.Time = DateTime.Parse(data["Time"].ToString());
                                record.Status = data["Status"].ToString();
                                record.DateAdded = DateTime.Parse(data["DateAdded"].ToString());
                                record.IsGoing = int.Parse(data["IsGoing"].ToString());
                                record.Interested = int.Parse(data["Interested"].ToString());
                                record.NotGoing = int.Parse(data["NotGoing"].ToString());
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                con.Close();
            }

            return View(record);
        }


        public string action = "";
        bool IsInserted(int eid)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT Action FROM Interests WHERE UserID=@UserID AND EventID=@EventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    cmd.Parameters.AddWithValue("@EventID", eid);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        if (data.HasRows)
                        {
                            while (data.Read())
                            {
                                action = data["Action"].ToString();
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

            }
            return false;
        }

        [HttpPost]
        public ActionResult Details(int? id, string Action)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            int val1 = 0;
            int val2 = 0;
            int val3 = 0;
            bool isInsert = false;
            if (IsInserted((int)id))
            {
                isInsert = true;
            }
            string checkValue = "";
            if (action == "IsGoing")
            {
                checkValue = "1";
            }
            else if (action == "Interested")
            {
                checkValue = "2";
            }
            else if (action == "NotGoing")
            {
                checkValue = "3";
            }
            string Iaction = "";

            if (Action == "1" && isInsert == false)
            {
                val1 = 1;
                checkValue = "1";
            }
            else if (Action == "2" && isInsert == false)
            {
                val2 = 1;
                checkValue = "2";
            }
            else if (Action == "3" && isInsert == false)
            {
                val3 = 1;
                checkValue = "3";
            }
            else if (Action == "1" && isInsert == true)
            {
                if (checkValue == "1")
                {
                    val1 = 0;
                }
                else if (checkValue == "2")
                {
                    val1 = 1;
                    val2 = -1;
                }
                else if (checkValue == "3")
                {
                    val1 = 1;
                    val3 = -1;
                }
                checkValue = "1";
            }
            else if (Action == "2" && isInsert == true)
            {
                if (checkValue == "1")
                {
                    val1 = -1;
                    val2 = 1;
                }
                else if (checkValue == "2")
                {
                    val2 = 0;
                }
                else if (checkValue == "3")
                {
                    val2 = 1;
                    val3 = -1;
                }
                checkValue = "2";
            }
            else if (Action == "3" && isInsert == true)
            {
                if (checkValue == "1")
                {
                    val1 = -1;
                    val3 = 1;
                }
                else if (checkValue == "2")
                {
                    val2 = -1;
                    val3 = 1;
                }
                else if (checkValue == "3")
                {
                    val3 = 0;
                }
                checkValue = "3";
            }

            if (checkValue == "1")
            {
                Iaction = "IsGoing";
            }
            else if (checkValue == "2")
            {
                Iaction = "Interested";
            }
            else if (checkValue == "3")
            {
                Iaction = "NotGoing";
            }

            var record = new EventsModel();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"UPDATE Events SET IsGoing=IsGoing+@value1, Interested=Interested+@value2, NotGoing=NotGoing+@value3 WHERE EventID=@EventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@value1", val1);
                    cmd.Parameters.AddWithValue("@value2", val2);
                    cmd.Parameters.AddWithValue("@value3", val3);
                    cmd.Parameters.AddWithValue("@EventID", id);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = "";
                if (isInsert == false)
                {
                    query = @"INSERT INTO Interests VALUES (@UserID, @EventID, @Action)";
                }
                else if (isInsert == true)
                {
                    query = @"UPDATE Interests SET Action=@Action WHERE UserID=@UserID AND EventID=@EventID";
                }
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    cmd.Parameters.AddWithValue("@EventID", id);
                    cmd.Parameters.AddWithValue("@Action", Iaction);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return RedirectToAction("Details");
        }


        public List<EventsModel> GetEditable()
        {
            var list = new List<EventsModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], l.Name AS LocationName, e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    INNER JOIN Location l on e.LocationCode=l.LocationCode
                    INNER JOIN Campus c on c.CampusID=l.CampusID
                    INNER JOIN EventHost EvHo on e.EventID=EvHo.EventID
                    WHERE EvHo.UserID=@UserID
                    ORDER BY e.DateAdded DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", Session["userid"].ToString());
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new EventsModel
                            {
                                ID = int.Parse(data["EventID"].ToString()),
                                Name = data["Name"].ToString(),
                                EventHandlerName = data["HandlerName"].ToString(),
                                LocationName = data["LocationName"].ToString(),
                                Description = data["Description"].ToString(),
                                Image = data["Image"].ToString(),
                                Time = DateTime.Parse(data["Time"].ToString()),
                                DateAdded = DateTime.Parse(data["DateAdded"].ToString())

                            });

                        }
                    }
                }
                con.Close();
            }
            return list;
        }

        public ActionResult IndexOfEditableEvent()
        {
            Helper.ValidateLogin();
            var list = new EventHostModel();
            list.EditableEvents = GetEditable();
            return View(list);
        }

        public ActionResult EditEvent(int? id)
        {
            Helper.ValidateLogin();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var record = new EventsModel();
            record.AllLocations = GetLocation();
            record.EventHandlers = GetHandler();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"Select e.EventID, e.EventHandlerID, h.Name AS EHName, e.LocationCode, 
                            l.Name AS LName, c.Name AS CName,
                            e.Name, e.Description, e.Image, e.Time, e.Status,
                            e.DateAdded, e.IsGoing, e.Interested, e.NotGoing
                            FROM Events e
                            LEFT JOIN EventHandler h ON e.EventHandlerID=h.EventHandlerID
                            LEFT JOIN Location l ON e.LocationCode=l.LocationCode
                            LEFT JOIN Campus c ON l.CampusID=c.CampusID
                            WHERE e.EventID=@EventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EventID", id);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        if (data.HasRows)
                        {
                            while (data.Read())
                            {
                                record.ID = int.Parse(data["EventID"].ToString());
                                record.EventHandlerID = int.Parse(data["EventHandlerID"].ToString());
                                record.EventHandlerName = data["EHName"].ToString();
                                record.LocationCode = int.Parse(data["LocationCode"].ToString());
                                record.LocationName = data["CName"].ToString() + ", " + data["LName"].ToString();
                                record.Name = data["Name"].ToString();
                                record.Description = data["Description"].ToString();
                                record.Image = data["Image"].ToString();
                                record.Time = DateTime.Parse(data["Time"].ToString());
                                record.Status = data["Status"].ToString();
                                record.DateAdded = DateTime.Parse(data["DateAdded"].ToString());
                                record.IsGoing = int.Parse(data["IsGoing"].ToString());
                                record.Interested = int.Parse(data["Interested"].ToString());
                                record.NotGoing = int.Parse(data["NotGoing"].ToString());
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                con.Close();
            }
            return View(record);
        }

        [HttpPost]
        public ActionResult EditEvent(int? id, EventsModel record, HttpPostedFileBase Image)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"UPDATE Events SET 
                    EventHandlerID=@EventHandlerID, LocationCode=@LocationCode, 
                    Name=@Name, Description=@Description, Image=@Image,
                    Time=@Time, Status=@Status, DateModified=@DateModified
                    WHERE EventID=@EventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {

                        cmd.Parameters.AddWithValue("@EventHandlerID", record.EventHandlerID);
                        cmd.Parameters.AddWithValue("@LocationCode", record.LocationCode);
                        cmd.Parameters.AddWithValue("@Name", record.Name);
                        cmd.Parameters.AddWithValue("@Description", record.Description);
                        cmd.Parameters.AddWithValue("@Image",
                            DateTime.Now.ToString("yyyyMMddHHmmss-") + Image.FileName);
                        Image.SaveAs(Server.MapPath("~/Images/Events/" +
                            DateTime.Now.ToString("yyyyMMddHHmmss-") + Image.FileName));
                        cmd.Parameters.AddWithValue("@Time", record.Time);
                        cmd.Parameters.AddWithValue("Status", "Updated");
                        cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EventID", id);
                        cmd.ExecuteNonQuery();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = "<div class='alert alert-danger col-lg-6'>Error: " + e.Message + "</div>";
                        return View();
                    }
                }
            }
        }
    }

}

