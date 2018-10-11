using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AdverBenilde.Models
{
    public class AuthorModel
    {
        [Key]
        public int RecordNo { get; set; }
        public string Name { get; set; }
        public int orgID { get; set; }
        public int roomID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; }
        public string org { get; set; }
        public string room { get; set; }
        public List<CampusModel> AllCampus { get; set; }
        public List<LocationModel> AllLocations { get; set; }
        public List<AuthorModel> AllAuthor { get; set; }
        public List<EventHandlerModel> EventHandlers { get; set; }
    }
}