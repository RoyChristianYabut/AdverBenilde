using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace AdverBenilde.Models
{
    public class UserModel
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "User Type")]
        [Required(ErrorMessage = "Required field")]
        public int TypeID { get; set; }

        public List<TypesModel> UserTypes { get; set; }

        public string UserType { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Required")]
        public string FN { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Invalid input")]
        public string LN { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }
    }
}