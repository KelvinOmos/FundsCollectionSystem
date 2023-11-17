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
	public class JoinModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public SavingsGroupDTO savingsGroup { get; set; }

        public JoinModel(ApiProcessor apiCalls, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCalls;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
            {
                string errorMsg = string.Empty;
                var _url = _config.GetSection("apiUrls")["getSavingsGroupByIdUrl"];
                string data = JsonConvert.SerializeObject(new GetSavingsGroupByIdDTO() { Id = (long)id });
                string token = _signInMgm.AccessToken;
                var savingsGroupResp = await _apiCalls.PostDataAsync(data, _url, token);
                if (string.IsNullOrWhiteSpace(savingsGroupResp))
                {
                    errorMsg = "No Response from Get Savings Group By ID Service";
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToPage("/SavingsGroup/Index");
                }
                var _respVal = JsonConvert.DeserializeObject<Response<SavingsGroupDTO>>(savingsGroupResp);
                if (_respVal == null) { _logger.WriteErrorLog($"GET SAVINGS GROUP BY ID RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return RedirectToPage("/SavingsGroup/Index"); }
                if (_respVal.Code != (long)ApiResponseCodes.OK)
                {
                    errorMsg = _respVal.Description;
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToPage("/SavingsGroup/Index");
                }
                savingsGroup = _respVal.Data;
                return Page();
            }
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
                {
                    string errorMsg = string.Empty;
                    var _url = _config.GetSection("apiUrls")["joinSavingsGroupUrl"];
                    string data = JsonConvert.SerializeObject(new JoinSavingsGroupDTO() { SavingsGroupId = savingsGroup.Id, UserId = _signInMgm.UserId });
                    string token = _signInMgm.AccessToken;
                    var joinSavingsGroupResp = await _apiCalls.PostDataAsync(data, _url, token);
                    if (string.IsNullOrWhiteSpace(joinSavingsGroupResp))
                    {
                        errorMsg = "No Response from Join SavingsGroup Status Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<bool>>(joinSavingsGroupResp);
                    if (_respVal == null) { _logger.WriteErrorLog($"JOIN SAVINGS GROUP RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = _respVal.Description;
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    _logger.WriteErrorLog(_respVal.Description);
                    TempData["SuccessMessage"] = _respVal.Description;
                    return RedirectToPage("/SavingsGroup/Index");
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
