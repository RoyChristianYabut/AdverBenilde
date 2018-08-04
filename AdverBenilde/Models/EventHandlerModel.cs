using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AdverBenilde.Models
{
    public class EventHandlerModel
    {
        [Key]
        public int EventHandlerID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}