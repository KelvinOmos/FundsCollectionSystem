using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Helpers;
using CollectionSystem.WebApp.Models;
using CollectionSystem.WebApp.Models.SavingsGroup;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CollectionSystem.WebApp.Pages.SavingsGroup
{
	public class MembersModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public List<ApplicationUserDto> users { get; set; }

        public MembersModel(ApiProcessor apiCall, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCall;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(long? Id)
        {
            try
            {
                if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
                {
                    string errorMsg = string.Empty;
                    var _url = _config.GetSection("apiUrls")["fetchSavingGroupMembers"];
                    string data = JsonConvert.SerializeObject(new GetSavingsGroupMemberDTO() { GroupId = (long)Id });
                    string token = _signInMgm.AccessToken;
                    var userListResp = await _apiCalls.PostDataAsync(data, _url, token);
                    if (string.IsNullOrWhiteSpace(userListResp))
                    {
                        errorMsg = "No Response from Get Savings Group Members Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<List<ApplicationUserDto>>>(userListResp);
                    if (_respVal == null) { _logger.WriteErrorLog($"SAVING GROUP USER LIST RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = _respVal.Description;
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    users = _respVal.Data;
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
