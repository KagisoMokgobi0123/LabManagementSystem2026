using Microsoft.AspNetCore.Identity;
using NMB_HLabSys.Areas.Manager.Models;
using System.ComponentModel.DataAnnotations;


namespace NMB_HLabSys.Models.Account
{
    public class RegisterUser : IdentityUser
    {
       
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 100 characters")]
        public string FullName { get; set; } = String.Empty;
        public int IDNumber { get; set; }
        public int EmployeeNumber { get; set; }
        public int ContactNumber { get; set; }
        public bool MustChangePassword { get; set; } = true;
        public List<Areas.Manager.Models.TechnicianTestTypeModel> TechnicianTestTypes { get; set; } = new();
    }
}
