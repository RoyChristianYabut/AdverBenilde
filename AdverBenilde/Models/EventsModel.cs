using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace AdverBenilde.Models
{
    public class EventsModel
    {
        [Key]
        public int ID { get; set; }

        public int EventHandlerID { get; set; }
        public List<EventHandlerModel> EventHandlers { get; set; }
        public string EventHandlerName { get; set; }
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public List<LocationModel> AllLocations { get; set; }
        public List<EventsModel> AllEvents { get; set; }
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }




        public string Image { get; set; }

        public DateTime Time { get; set; }

        public bool IsFeatured { get; set; }

        public string Status { get; set; }
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public int IsGoing { get; set; }
        public int Interested { get; set; }
        public int NotGoing { get; set; }
    }
}