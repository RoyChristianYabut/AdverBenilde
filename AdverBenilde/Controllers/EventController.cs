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
                                Name = data["Name"].ToString()
                            });
                        }
                    }
                }

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

            }
            return list;
        }

        public ActionResult Create()
        {
            var record = new EventsModel();
            record.AllLocations = GetLocation();
            record.EventHandlers = GetHandler();
            return View(record);
        }

        [HttpPost]
        public ActionResult Create(EventsModel record, HttpPostedFileBase Image)
        {
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @" INSERT INTO Events
                              (EventHandlerID, LocationCode, Name, Description, 
                               Image, Time ,Status,DateAdded)
                              VALUES
                              (@EventHandlerID, @LocationCode, @Name, @Description, 
                              @Image, @Time,@Status,@DateAdded)";
                using (SqlCommand cmd = new SqlCommand(query, con))
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
                    cmd.ExecuteNonQuery();
                }

            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateLocation()
        {
            var record = new LocationModel();
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
                }

            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateHandler()
        {
            var record = new LocationModel();
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
                              (@ @Name)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", record.Name);
                }

            }

            return RedirectToAction("Index");
        }

        // GET: Products
        public List<LocationModel> GetLocations()
        {
            var list = new List<LocationModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT l.LocationCode, c.Name AS CampusName, l.Name,
                    (SELECT COUNT(e.LocationCode) FROM Events e
                    WHERE e.LocationCode = l.LocationCode) AS TotalCount
                    FROM Location l
                    INNER JOIN Campus c ON l.CampusID=c.CampusID
                    WHERE NOT l.Name='Other...'
                    ORDER BY c.Name DESC, l.Name";
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
                                Name = data["Name"].ToString(),
                                TotalCount = int.Parse(data["TotalCount"].ToString())
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<EventsModel> GetEvent()
        {
            var list = new List<EventsModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
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
                                Description = data["Description"].ToString(),
                                Image = data["Image"].ToString(),
                                Time = DateTime.Parse(data["Time"].ToString()),
                                DateAdded = DateTime.Parse(data["DateAdded"].ToString())

                            });

                        }
                    }
                }

            }
            return list;
        }



        public List<EventsModel> GetEvent(string locatID)
        {
            var list = new List<EventsModel>();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"SELECT e.EventID, e.Name, eh.Name AS [HandlerName], e.Description, e.Image, e.Time,
                    e.DateAdded 
                    FROM Events e
                    LEFT JOIN EventHandler eh on e.EventHandlerID=eh.EventHandlerID
                    WHERE e.LocationCode=@LocationCode
                    ORDER BY v.DateAdded";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@LocationCode", locatID);
                    using (SqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            list.Add(new EventsModel
                            {
                                ID = int.Parse(data["EventID"].ToString()),
                                Name = data["Name"].ToString(),
                                EventHandlerName = data["Handler Name"].ToString(),
                                Description = data["Description"].ToString(),
                                Image = data["Image"].ToString(),
                                Time = DateTime.Parse(data["Time"].ToString()),
                                DateAdded = DateTime.Parse(data["DateAdded"].ToString())
                            });
                        }
                    }
                }
            }
            return list;
        }

        public ActionResult Index()
        {
            var list = new EventsModel();
            list.AllLocations = GetLocations();
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
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var record = new EventsModel();
            using (SqlConnection con = new SqlConnection(Helper.GetCon()))
            {
                con.Open();
                string query = @"Select e.EventID, h.Name AS EHName, l.Name AS LName, c.Name AS CName,
                            e.Name, e.Description, e.Image, e.Time, e.Status
                            e.DateAdded
                            FROM Events e
                            LEFT JOIN EventHandler h ON e.EventHandlerID=h.EventHandlerID
                            LEFT JOIN Location l ON e.LocationCode=v.LocationCode
                            INNER JOIN Campus c ON l.CampusID=c.CampusID
                            WHERE e.EventID=@ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("ID", id);
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
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
            }

            return View(record);
        }
    }
}

