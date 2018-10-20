using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recruiter.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recruiter.Controllers
{
    public class JobOfferController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(_jobOffers);
        }
        public IActionResult Details(int id)
        {
            return View(_jobOffers.Where(x=>x.Id==id).First());
        }
        private static List<JobOffer> _jobOffers = new List<JobOffer>
        {
            new JobOffer{Id=1, JobTitle="Backend Developer", PostingDate = DateTime.Now},
            new JobOffer{Id=2, JobTitle="Frontend Developer", PostingDate = DateTime.Now},
            new JobOffer{Id=3, JobTitle="Manager", PostingDate = DateTime.Now},
            new JobOffer{Id=4, JobTitle="Teacher", PostingDate = DateTime.Now},
            new JobOffer{Id=5, JobTitle="Cook", PostingDate = DateTime.Now},
        };
    }
}
