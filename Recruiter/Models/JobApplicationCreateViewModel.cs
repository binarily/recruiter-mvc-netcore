using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobApplicationCreateViewModel : JobApplication
    {
        public JobOffer Offer { get; set; }
        [Required]
        public IFormFile CVFile { get; set; }
    }
}
