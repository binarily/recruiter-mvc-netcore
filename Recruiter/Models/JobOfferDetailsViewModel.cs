using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobOfferDetailsViewModel
    {
        public JobOffer Offer { get; set; }
        public List<JobApplication> Applications { get; set; }
    }
}
