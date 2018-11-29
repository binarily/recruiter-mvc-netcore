using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Recruiter.EntityFramework;
using Recruiter.Helpers;
using Recruiter.Models;

namespace Recruiter.Controllers
{
    [Route("api/applications")]
    [ApiController]
    /// <summary>
    /// Job Applications API
    /// </summary>
    /// <remarks>
    /// This API is used to parse applications within Recruiter.
    /// </remarks>
    public class JAAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        private AppSettings AppSettings { get; set; }
        public JAAPIController(DataContext context, IConfiguration Configuration)
        {
            _context = context;

            _configuration = Configuration;
            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        // GET: api/applications/5
        /// <summary>
        /// Get an application with a given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An application with a given ID.</returns>
        [HttpGet("{offerid}")]
        public async Task<IActionResult> GetJobApplicationByOffer([FromRoute] int offerid)
        {

            bool isIngroup = await CheckIfUserIsAnAdmin();
            if (!isIngroup)
                return Forbid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobApplication = _context.JobApplications.Where(x => x.OfferId == offerid);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return Ok(jobApplication);
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/applications/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/applications/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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