using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobOfferCreateViewModel{ 
        public JobOffer Offer { get; set; }
        public IEnumerable<Company> Companies { get; set; }
    }
}
