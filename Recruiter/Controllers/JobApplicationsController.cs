using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Recruiter.EntityFramework;
using Recruiter.Helpers;
using Recruiter.Models;

namespace Recruiter.Controllers
{
    public class JobApplicationsController : Controller
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        private AppSettings AppSettings { get; set; }

        public JobApplicationsController(DataContext context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Index" });
            return View(await _context.JobApplications.ToListAsync());
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Edit/"+id });
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplications/Create
        public IActionResult Create(int offerId)
        {
            JobOffer offer = _context.JobOffers.First(m => m.Id == offerId);
            if(offer == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            return View(new JobApplicationCreateViewModel { Offer = offer, OfferId=offerId});
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplicationCreateViewModel jobApplication)
        {
            if (ModelState.IsValid)
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=recruitercvs;AccountKey=72dirRppgAhr4ZwbmGOR9UJbdhKADea11VcWTi1uBHcGhlfNNIFvY6mRKFl55XeoU1v2NgHanU9fxTbLZYHMrw==;EndpointSuffix=core.windows.net";

                CloudStorageAccount storageAccount = null;
                if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                    CloudBlobContainer container = cloudBlobClient.GetContainerReference("applications");
                    //await container.CreateIfNotExistsAsync();

                    // Get the reference to the block blob from the container
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(DateTime.Now.ToLongDateString()+jobApplication.CVFile.Name);
                    jobApplication.CvUrl = blockBlob.Uri.AbsoluteUri;

                    // Upload the file
                    var stream = jobApplication.CVFile.OpenReadStream();
                    await blockBlob.UploadFromStreamAsync(stream).ContinueWith(delegate { stream.Close(); });
                }
                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "JobOffer");
            }
            JobOffer offer = _context.JobOffers.First(m => m.Id == jobApplication.OfferId);
            if (offer == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            jobApplication.Offer = offer;
            return View(jobApplication);
        }

        // GET: JobApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Edit/"+id });
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admins")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OfferId,FirstName,LastName,PhoneNumber,EmailAddress,Description,ContractAgreement,CvUrl")] JobApplication jobApplication)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Edit/"+id });
            if (id != jobApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Detail/"+id });
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "JobApplications/Details/"+id });
            var jobApplication = await _context.JobApplications.FindAsync(id);
            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return _context.JobApplications.Any(e => e.Id == id);
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
