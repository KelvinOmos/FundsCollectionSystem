using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using CollectionSystem.WebApp;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Helpers;
using CollectionSystem.WebApp.Models;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;

namespace CollectionSystem.WebApp.Pages.UserManagement.Users
{
    public class IndexModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public List<UserModel> users { get; set; }

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
                    var _url = _config.GetSection("apiUrls")["userListUrl"];
                    string token = HttpContext.Session.GetString("accessToken");
                    var userListResp = await _apiCalls.GetDataAsync(_url, token);
                    if (string.IsNullOrWhiteSpace(userListResp))
                    {
                        errorMsg = "No Response from Get Users Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<List<UserModel>>>(userListResp);
                    if (_respVal == null) { _logger.WriteErrorLog($"USER LIST RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return Page(); }
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
