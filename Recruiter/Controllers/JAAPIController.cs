using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruiter.EntityFramework;
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

        public JAAPIController(DataContext context)
        {
            _context = context;
        }

        // GET: api/applications/5
        /// <summary>
        /// Get an application with a given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An application with a given ID.</returns>
        [HttpGet("{offerid}")]
        public IActionResult GetJobApplicationByOffer([FromRoute] int offerid)
        {
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
    }
}