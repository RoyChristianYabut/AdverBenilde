using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AdverBenilde.Models
{
    public class LocationModel
    {
        [Key]
        public int LocationCode { get; set; }
        public int CampusID { get; set; }
        public string CampusName { get; set; }
        public string Name { get; set; }
        public int TotalCount { get; set; }
    }
}