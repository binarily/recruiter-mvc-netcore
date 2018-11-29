using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Recruiter.EntityFramework;
using Recruiter.Helpers;
using Recruiter.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recruiter.Controllers
{
    public class JobOfferController : Controller
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        private AppSettings AppSettings { get; set; }

        public JobOfferController(DataContext context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        //Index
        public async Task<IActionResult> Index()
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (isIngroup)
                User.Identities.First().AddClaim(new Claim(ClaimTypes.Role, "Admins"));
            return View(_context.JobOffers.Include(x=>x.Company).ToList());
        }
        //Details
        public async Task<IActionResult> Details(int id)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (isIngroup)
                User.Identities.First().AddClaim(new Claim(ClaimTypes.Role, "Admins"));
            var offer = _context.JobOffers.First(x => x.Id == id);
            if (offer == null) return new StatusCodeResult((int)HttpStatusCode.NotFound);
            _context.Entry(offer).Reference(f => f.Company).Load();
            JobOfferDetailsViewModel model = new JobOfferDetailsViewModel { Offer = offer };
            model.Applications = _context.JobApplications.Where(x => x.OfferId == id).ToList();
            return View(model);
        }
        //Edit
        public async Task<IActionResult> Edit(int? id)
        {

            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobOffer/Edit" });
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
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobOffer/Edit" });
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
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobOffer/Details/"+id });
            if (id == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            _context.JobOffers.Remove(_context.JobOffers.First(j => j.Id == id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //Create
        public async Task<IActionResult> Create()
        {

            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobOffer/Create" });
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
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobOffer/Create" });
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
        private async Task<bool> CheckIfUserIsAnAdmin()
        {
            // AAD usage example
            AADGraph graph = new AADGraph(AppSettings);
            string groupName = "Admins";
            string groupId = AppSettings.AADGroups.FirstOrDefault(g => String.Compare(g.Name, groupName) == 0).Id;
            bool isIngroup = await graph.IsUserInGroup(User.Claims, groupId);
            return isIngroup;
        }

    }
}
