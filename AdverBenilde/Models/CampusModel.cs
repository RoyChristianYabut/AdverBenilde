using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdverBenilde.Models
{
    public class CampusModel
    {
        [Key]
        public int CampusID { get; set; }

        public string Name { get; set; }
    }
}