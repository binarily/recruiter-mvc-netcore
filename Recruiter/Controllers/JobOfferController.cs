using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recruiter.EntityFramework;
using Recruiter.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recruiter.Controllers
{
    public class JobOfferController : Controller
    {
        private readonly DataContext _context;

        public JobOfferController(DataContext context)
        {
            _context = context;
        }

        //Index
        public IActionResult Index([FromQuery(Name ="search")] string searchString)
        {
            if (String.IsNullOrEmpty(searchString)) {
                var offers = _context.JobOffers;
                foreach (JobOffer o in offers)
                    _context.Entry(o).Reference(f => f.Company).Load();
                return View(offers.ToList());
            }
            List<JobOffer> searchResult = _context.JobOffers.Where(a => a.JobTitle.Contains(searchString)).ToList();

            foreach (JobOffer o in searchResult)
                _context.Entry(o).Reference(f => f.Company).Load();
            return View(searchResult);
        }
        //Details
        public IActionResult Details(int id)
        {
            var offer = _context.JobOffers.First(x => x.Id == id);
            if (offer == null) return new StatusCodeResult((int)HttpStatusCode.NotFound);
            _context.Entry(offer).Reference(f => f.Company).Load();
            JobOfferDetailsViewModel model = new JobOfferDetailsViewModel { Offer = offer };
            model.Applications = _context.JobApplications.Where(x => x.OfferId == id).ToList();
            return View(model);
        }
        //Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            var offer = _context.JobOffers.First(j => j.Id == id);
            if (offer == null) return new StatusCodeResult((int)HttpStatusCode.NotFound);
            _context.Entry(offer).Reference(f => f.Company).Load();
            JobOfferCreateViewModel createView = new JobOfferCreateViewModel { Offer = offer, Companies = _context.Companies };
            return View(createView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobOfferCreateViewModel model)
        {
            if (!ModelState.IsValid) {
                model.Companies = _context.Companies;
                return View(model);
            }
            var offer = _context.JobOffers.First(j => j.Id == model.Offer.Id);
            offer = model.Offer;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = model.Offer.Id });
        }
        //Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            _context.JobOffers.Remove(_context.JobOffers.First(j => j.Id == id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //Create
        public IActionResult Create()
        {
            var model = new JobOfferCreateViewModel
            {
                Companies = _context.Companies.ToList(),
                Offer = new JobOffer()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobOfferCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = _context.Companies.ToList();
                return View(model);
            }
            model.Offer.Company = _context.Companies.FirstOrDefault(j => j.Id == model.Offer.CompanyId);
            model.Offer.Created = DateTime.Now;
            _context.JobOffers.Add(model.Offer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
