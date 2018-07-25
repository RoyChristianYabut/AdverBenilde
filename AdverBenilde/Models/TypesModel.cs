using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AdverBenilde.Models
{
    public class TypesModel
    {
        [Key]
        public int TypeID { get; set; }

        public string UserType { get; set; }
    }
}