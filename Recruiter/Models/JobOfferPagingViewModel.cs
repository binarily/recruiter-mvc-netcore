using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recruiter.Models
{
    public class JobOfferPagingViewModel
    {
        public List<JobOffer> Offers { get; set; }
        public int TotalPage { get; set; }
    }
}
