using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonModel.DBModel
{
    public class EmployeeDetails
    {
        public int Id { get; set; }
        public string ?Name { get; set; }
        public string ?Designation { get; set; }
        public int DesignationId { get; set; }
        public string ?ProfilePicture { get; set; }
        public int Salary { get; set; }
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string ?Email { get; set; }
        public string ?Address { get; set; }

        
    }
}
