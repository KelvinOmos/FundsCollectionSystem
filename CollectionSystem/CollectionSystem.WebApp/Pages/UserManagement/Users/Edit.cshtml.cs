using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using CollectionSystem.WebApp;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Helpers;
using CollectionSystem.WebApp.Models;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;

namespace CollectionSystem.WebApp.Pages.UserManagement.Users
{
    public class SelectLists
    {
        public SelectList? Roles { get; set; }
        public SelectList? Branches { get; set; }
    }
    public class EditModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public ApplicationUserDto user { get; set; }
        [BindProperty] public SelectLists userSelectLists { get; set; }

        public EditModel(ApiProcessor apiCalls, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCalls;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
            {
                string errorMsg = string.Empty;
                var _url = _config.GetSection("apiUrls")["viewUserUrl"];
                string data = JsonConvert.SerializeObject(new viewUserDTO() { Id = id });
                string token = _signInMgm.AccessToken;
                var userResp = await _apiCalls.PostDataAsync(data, _url, token);
                if (string.IsNullOrWhiteSpace(userResp))
                {
                    errorMsg = "No Response from Get User Service";
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToPage("/UserManagement/Users/Index");
                }
                var _respVal = JsonConvert.DeserializeObject<Response<ApplicationUserDto>>(userResp);
                if (_respVal == null) { _logger.WriteErrorLog($"GET USER RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return RedirectToPage("/UserManagement/Users/Index"); }
                if (_respVal.Code != (long)ApiResponseCodes.OK)
                {
                    errorMsg = _respVal.Description;
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToPage("/UserManagement/Users/Index");
                }
                user = _respVal.Data;
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
                    var _url = _config.GetSection("apiUrls")["updateUserStatusUrl"];
                    string data = JsonConvert.SerializeObject(new UserStatusDTO()
                    {
                        UserId = user.Id,
                        Status = (UserStatuses)user.Status
                        //ReasonForChange = "admin"
                    });
                    string token = _signInMgm.AccessToken;
                    var userResp = await _apiCalls.PostDataAsync(data, _url, token);
                    if (string.IsNullOrWhiteSpace(userResp))
                    {
                        errorMsg = "No Response from Update User Status Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<string>>(userResp);
                    if (_respVal == null) { _logger.WriteErrorLog($"UPDATE USER RESPONSE IS NULL"); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = _respVal.Description;
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    _logger.WriteErrorLog(_respVal.Description);
                    TempData["SuccessMessage"] = _respVal.Description;
                    return RedirectToPage("/UserManagement/Users/Index");
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
