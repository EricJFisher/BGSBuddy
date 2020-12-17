using BGSBuddyWeb.Models;
using Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BGSBuddyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISituationReportsService _situationReportsService;
        private readonly ISolarSystemsService _systemsServices;

        private string faction = string.Empty;
        private List<string> offLimitsList = new List<string>();

        public HomeController(ILogger<HomeController> logger, ISituationReportsService situationReportsService, ISolarSystemsService systemsService)
        {
            _logger = logger;
            _situationReportsService = situationReportsService;
            _systemsServices = systemsService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new SituationReportViewModel();

            faction = GetCookie("faction");
            if (string.IsNullOrEmpty(faction))
                SetCookie("faction", "Alliance Rapid-reaction Corps", 7);
            viewModel.SituationReport.FactionName = faction;

            var offLimits = GetCookie("offlimits");
            if (!string.IsNullOrEmpty(offLimits))
                offLimitsList = offLimits.Split(',').ToList();
            viewModel.SituationReport.OffLimits = offLimitsList;

            viewModel.SituationReport = await _situationReportsService.GenerateReport(viewModel.SituationReport);

            return View(viewModel);
        }

        public async Task<IActionResult> Expansion(string systemName)
        {
            //remove manual set once input field is added
            systemName = "Aasgaa";
            faction = "Alliance Rapid-reaction Corps";
            var viewModel = new ExpansionPlannerViewModel();
            viewModel.SystemName = systemName;
            var expansionTargets = await _systemsServices.GetExpansionTargets(systemName);

            foreach(var system in expansionTargets)
            {
                if (system.SubFactions.Any(e => string.Equals(e.Name, faction, StringComparison.InvariantCultureIgnoreCase)))
                    continue;
                if (system.SubFactions.Count() < 7)
                {
                    viewModel.NeverRetreatedSystems.Add(system);
                    continue;
                }
                if(system.SubFactions.Any(e => !e.Name.Contains(system.Name, StringComparison.InvariantCultureIgnoreCase) && e.Influence < 0.05))
                {
                    viewModel.InvasionSystems.Add(system);
                }
                
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddToOffLimits(string systemName)
        {
            offLimitsList.Add(systemName);
            SetCookie("offlimits", string.Join(',', offLimitsList), 7);
            return RedirectToAction("Index");
        }

        private string GetCookie(string key)
        {
            return Request.Cookies[key];
        }

        private void SetCookie(string key, string value, int? expirationTime)
        {
            CookieOptions option = new CookieOptions();
            if (expirationTime.HasValue)
                option.Expires = DateTime.Now.AddDays(expirationTime.Value);
            else
                option.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Append(key,value,option);
        }
    }
}
