using ExpressiveAnnotations.Attributes;
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
        public int Id { get; set; }
        [Required]
        [DisplayName("Job title")]
        public string JobTitle { get; set; }
        public virtual Company Company { get; set; }
        public virtual int CompanyId { get; set; }
        [DisplayName("Salary from")]
        [AssertThat("SalaryFrom <= SalaryTo")]
        [Range(0, int.MaxValue)]
        public decimal? SalaryFrom { get; set; }
        [DisplayName("Salary to")]
        [Range(0, int.MaxValue)]
        [AssertThat("SalaryFrom <= SalaryTo")]
        public decimal? SalaryTo { get; set; }
        [DisplayName("Posted")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        public string Location { get; set; }
        [Required]
        [MinLength(100)]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString ="{0:yyy-MM-dd}")]
        [DisplayName("Valid until")]
        [AssertThat("ValidUntil >= Now()")]
        public DateTime? ValidUntil { get; set; }
        public List<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}
