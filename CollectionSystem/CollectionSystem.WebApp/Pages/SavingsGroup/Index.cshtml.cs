using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionSystem.WebApp.Models.SavingsGroup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using CollectionSystem.WebApp;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Helpers;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;

namespace CollectionSystem.WebApp.Pages.SavingsGroup
{
    public class IndexModel : PageModel
    {

        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public List<SavingsGroupDTO> savingsGroups { get; set; }

        public IndexModel(ApiProcessor apiCall, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCall;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
                {
                    string errorMsg = string.Empty;
                    var _url = _config.GetSection("apiUrls")["savingsGroupListUrl"];
                    string token = HttpContext.Session.GetString("accessToken");
                    var savingsGroupListResp = await _apiCalls.GetDataAsync(_url, token);
                    if (string.IsNullOrWhiteSpace(savingsGroupListResp))
                    {
                        errorMsg = "No Response from Get SavingsGroups Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<List<SavingsGroupDTO>>>(savingsGroupListResp);
                    if (_respVal == null) { _logger.WriteErrorLog($"SAVINGS GROUP LIST RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = _respVal.Description;
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    savingsGroups = _respVal.Data;
                    return Page();
                }
                return RedirectToPage("/Index");
            }
            catch (Exception eex)
            {
                _logger.WriteErrorLog($"Exception:: {eex?.ToString()}");
                TempData["ErrorMessage"] = "An error has occured";
                return Page();
            }
        }
    }
}
