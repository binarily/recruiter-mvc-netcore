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
    [Route("api/offers")]
    [ApiController]
    public class JOAPIController : ControllerBase
    {
        private readonly DataContext _context;

        public JOAPIController(DataContext context)
        {
            _context = context;
        }

        // GET: api/offers/5

        /// <summary>
        /// Get an offer with a given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An offer with a given ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobOffer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobOffer = await _context.JobOffers.FindAsync(id);

            if (jobOffer == null)
            {
                return NotFound();
            }

            return Ok(jobOffer);
        }

        /// <summary>
        /// Get all offers available or with name matching query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>A list of offers.</returns>
        [HttpGet]
        public IActionResult GetJobOffers([FromQuery(Name = "query")] string query, [FromQuery(Name ="page")] string page="1")
        {
            int pageSize = 10;
            int currentPage;
            int totalPage;
            JobOfferPagingViewModel result;
            Int32.TryParse(page, out currentPage);
            if (String.IsNullOrEmpty(query))
            {
                totalPage = (_context.JobOffers.Count() / pageSize) + ((_context.JobOffers.Count() % pageSize) > 0 ? 1 : 0);
                var offers = _context.JobOffers.Skip((currentPage - 1) * pageSize).Take(pageSize).Include(x => x.Company);
                result = new JobOfferPagingViewModel { Offers = offers.ToList(), TotalPage = totalPage };
                return Ok(result);
            }

            var jobOffers = _context.JobOffers.Where(x => x.JobTitle.Contains(query));

            if (jobOffers == null)
            {
                return NotFound();
            }
            totalPage = (jobOffers.Count() / pageSize) + ((jobOffers.Count() % pageSize) > 0 ? 1 : 0);
            jobOffers = jobOffers.Skip((currentPage - 1) * pageSize).Take(pageSize).Include(x => x.Company);
            result = new JobOfferPagingViewModel { Offers = jobOffers.ToList(), TotalPage = totalPage };
            return Ok(result);
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/offers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/offers/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool JobOfferExists(int id)
        {
            return _context.JobOffers.Any(e => e.Id == id);
        }
    }
}