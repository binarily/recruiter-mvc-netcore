using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Recruiter.Helpers;
using Recruiter.Models;

namespace Recruiter.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private AppSettings AppSettings { get; set; }
        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            AppSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Privacy()
        {
            bool isIngroup = await CheckIfUserIsAnAdmin();
            
            if (!isIngroup)
                return RedirectToAction("SignIn", "Session", new { redirect = "Home/Privacy" });
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
