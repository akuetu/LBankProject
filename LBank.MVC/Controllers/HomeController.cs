using LBank.Business;
using LBank.Domain;
using LBank.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LBank.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServerConfigService _serverConfigService;

        public HomeController(IServerConfigService serverConfigService)
        {
            _serverConfigService = serverConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var serverConfigs = await _serverConfigService.ReadServerConfigs();         
                    
            var serverNames = await _serverConfigService.GetAllServersName();

            ViewBag.ServerNames = new SelectList(serverNames);

            return View(serverConfigs);
        }


        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromBody] ServerConfig serverConfig)
        {
            if (ModelState.IsValid)
            {
                await _serverConfigService.CreateServerConfig(serverConfig);
                var serverConfigs = await _serverConfigService.ReadServerConfigs();               
                var serverNames = await _serverConfigService.GetAllServersName();

                ViewBag.ServerNames = new SelectList(serverNames);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, data = serverConfigs });
                }

                return View(serverConfigs);
            }

            return View(new ServerConfig());
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
