using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMB_HLabSys.Areas.Administrator.Models
{
    public class LabTechnician
    {
        [Key]
        public int LabTechnicianID { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public int EmployeeNumber { get; set; }

        [Required]
        public int CellPhone { get; set; }

        [Required]
        public string Email { get; set; }


        public string Password { get; set; }

        //[Required]
        // public bool MustChangePassword {  get; set; } not sure if its needed...

    }
}