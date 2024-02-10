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

        [HttpGet]
        public async Task<IActionResult> Update(string serverName)
        {            
            var server = await _serverConfigService.GetOneServerConfigs(serverName) ?? new ServerConfig();           
            return View(server);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ServerConfig serverConfig)
        {
             
            if (ModelState.IsValid)
            {
                var result = await _serverConfigService.UpdateServerConfig(serverConfig);
                var message = new ServerMessage
                {
                    IsSuccess = result,
                    Message = result ? $"The server {serverConfig.ServerName} has been updated successfully." :
                                                $"An error occurred while updating the server name {serverConfig.ServerName}."
                };

                ViewBag.ServerNames = message;

            }

            var server = await _serverConfigService.GetOneServerConfigs(serverConfig.ServerName) ?? new ServerConfig();
            return View(server);
        }


    }
}
