using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recruiter.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recruiter.Controllers
{
    public class JobOfferController : Controller
    {
        private static List<Company> _companies = new List<Company>
        {
            new Company{Id=1, Name="Microsoft" },
            new Company{Id=2, Name="Predica" },
            new Company{Id=3, Name="Github" },
        };
        private static List<JobOffer> _jobOffers = new List<JobOffer>
        {
            new JobOffer{
                Id =1,
                JobTitle = "Backend Developer",
                Company = _companies.FirstOrDefault(c => c.Name =="Predica"),
                Created = DateTime.Now.AddDays(-2),
                Description = "Backend C# developer with intrests about IoT solutions. The main task would be building API which expose data from phisical devices. Description need to have at least 100 characters so I am adding some. In test case I reccomend you to use Lorem Impsum.",
                Location = "Poland",
                SalaryFrom = 2000,
                SalaryTo = 10000,
                ValidUntil = DateTime.Now.AddDays(20)
            },
            new JobOffer{
                Id =2,
                JobTitle = "Frontend Developer",
                Company = _companies.FirstOrDefault(c => c.Name =="Microsoft"),
                Created = DateTime.Now.AddDays(-2),
                Description = "Developing Office 365 front end interface. Working with SharePoint and graph API. Connecting with AAD and building ML for Mailbox smart assistant. Description need to have at least 100 characters so I am adding some. In test case I reccomend you to use Lorem Impsum.",
                Location = "Poland",
                SalaryFrom = 2000,
                SalaryTo = 10000,
                ValidUntil = DateTime.Now.AddDays(20)
            }
        };


        //Index
        public IActionResult Index([FromQuery(Name ="search")] string searchString)
        {
            if(String.IsNullOrEmpty(searchString))
                return View(_jobOffers);
            List<JobOffer> searchResult = _jobOffers.FindAll(a => a.JobTitle.Contains(searchString));
            return View(searchResult);
        }
        //Details
        public IActionResult Details(int id)
        {
            return View(_jobOffers.Where(x=>x.Id==id).First());
        }
        //Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            var offer = _jobOffers.Find(j => j.Id == id);
            if (offer == null) return new StatusCodeResult((int)HttpStatusCode.NotFound);
            return View(offer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobOffer model)
        {
            if (!ModelState.IsValid) return View();
            var offer = _jobOffers.Find(j => j.Id == model.Id);
            offer.JobTitle = model.JobTitle;
            return RedirectToAction("Details", new { id = model.Id });
        }
        //Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            _jobOffers.RemoveAll(j => j.Id == id);
            return RedirectToAction("Index");
        }
        //Create
        public async Task<IActionResult> Create()
        {
            var model = new JobOfferCreateView
            {
                Companies = _companies
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobOfferCreateView model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = _companies;
                return View(model);
            }
            var id = _jobOffers.Max(j => j.Id) + 1;
            _jobOffers.Add(new JobOffer
            {
                Id = id,
                CompanyId = model.CompanyId,
                Company = _companies.FirstOrDefault(j => j.Id == model.CompanyId),
                Description = model.Description,
                JobTitle = model.JobTitle,
                Location = model.Location,
                SalaryFrom = model.SalaryFrom,
                SalaryTo = model.SalaryTo,
                ValidUntil = model.ValidUntil,
                Created = DateTime.Now
            });
            return RedirectToAction("Index");
        }


    }
}
