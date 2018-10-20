using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobOffer
    {
        [DisplayName("ID")]
        [Key]
        public int Id { get; set; }
        [DisplayName("Job title")]
        public string JobTitle { get; set; }
        [DisplayName("Posted")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime PostingDate { get; set; }
        [DisplayName("Type of employment")]
        public string EmploymentType { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Responsibilities")]
        public string ResponsibilitiesDescription { get; set; }
        [DisplayName("Qualifications")]
        public string Qualifications { get; set; }
    }
}
