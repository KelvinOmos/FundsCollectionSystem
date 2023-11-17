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
    public class CreateModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public newUserDTO newUser { get; set; }        

        public CreateModel(ApiProcessor apiCalls, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCalls;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
            {
                return Page();
            }
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string errorMsg = String.Empty;
            try
            {
                if (_signInMgm.UserName != null && (!string.IsNullOrEmpty(_signInMgm.UserName.ToString())))
                {
                    var _url = _config.GetSection("apiUrls")["addUserUrl"];
                    newUser.Password = "Password@2023";
                    newUser.ConfirmPassword = "Password@2023";
                    if (string.IsNullOrEmpty(newUser.UserName))
                    {
                        newUser.UserName = newUser.FirstName;
                    }
                    string data = JsonConvert.SerializeObject(newUser);
                    string token = _signInMgm.AccessToken;
                    var userResp = await _apiCalls.PostDataAsync(data, _url, token);
                    if (string.IsNullOrWhiteSpace(userResp))
                    {
                        errorMsg = "No Response from Create User Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<bool>>(userResp);
                    if (_respVal == null) { errorMsg = $"CREATE USER RESPONSE IS NULL"; _logger.WriteErrorLog(errorMsg); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = $"{_respVal.Description}";
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
