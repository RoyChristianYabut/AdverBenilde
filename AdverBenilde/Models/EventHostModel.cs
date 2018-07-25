using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AdverBenilde.Models
{
    public class EventHostModel
    {
        public int UserID { get; set; }
        public int EventID { get; set; }
    }
}