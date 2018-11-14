using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobApplicationCreateViewModel : JobApplication
    {
        public JobOffer Offer { get; set; }
    }
}
