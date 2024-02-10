using LBank.Business;
using LBank.Domain;
using LBank.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LBank.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServerConfigParser _serverConfigParser;

        public HomeController(IServerConfigParser serverConfigParser)
        {
            _serverConfigParser = serverConfigParser;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var serverConfigs = await _serverConfigParser.ReadServerConfigs();

            // move this to BLL
            var serverConfigDtos = serverConfigs.Select(sc => new ServerConfigDto
            {
                ServerName = sc.ServerName,
                Url = sc.Url,
                Db = sc.Db,
                IpAddress = sc.IpAddress,
                Domain = sc.Domain,
                CookieDomain = sc.CookieDomain
            }).ToList();

            //Interface BL
            var serverNames = serverConfigs.Select(sc => sc.ServerName).Distinct().Except(new[] { "MRAPPPOOLPORTL01" }).ToList();
            ViewBag.ServerNames = new SelectList(serverNames);

            return View(serverConfigDtos);
        }


        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromBody] ServerConfigDto config)
        {
            if (ModelState.IsValid)
            {          
                _serverConfigParser.Create(new ServerConfig
                {
                    ServerName = config.ServerName,
                    Url = config.Url,
                    Db = config.Db,
                    IpAddress = config.IpAddress,
                    Domain = config.Domain,
                    CookieDomain = config.CookieDomain
                });
                var serverConfigs = await _serverConfigParser.ReadServerConfigs();
                //Goes to BL
                var serverConfigDtos = serverConfigs.Select(sc => new ServerConfigDto
                {
                    ServerName = sc.ServerName,
                    Url = sc.Url,
                    Db = sc.Db,
                    IpAddress = sc.IpAddress,
                    Domain = sc.Domain,
                    CookieDomain = sc.CookieDomain
                }).ToList();

                //Interface BL
                var serverNames = serverConfigs.Select(sc => sc.ServerName).Distinct().Except(new[] { "MRAPPPOOLPORTL01" }).ToList();
                ViewBag.ServerNames = new SelectList(serverNames);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, data = serverConfigDtos });
                }
                return View(serverConfigDtos);
            }

            return View(new ServerConfigDto());
        }

        [HttpPost]
        public IActionResult SaveOrUpdate(ServerConfigDto config)
        {
            //ServerConfig
            if (ModelState.IsValid)
            {
                // Implement your saving or updating logic here
                // For demo purposes, we'll just redirect to the Index action
                return RedirectToAction("Index");
            }

            // If model state is not valid, return to the form with validation messages
            return View("Index", config);
        }


    }
}
