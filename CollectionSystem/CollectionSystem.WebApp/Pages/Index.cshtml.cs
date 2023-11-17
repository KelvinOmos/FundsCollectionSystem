using CollectionSystem.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Models;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;

namespace CollectionSystem.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApiProcessor _apiCall;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public loginRequest userprofile { get; set; }

        public IndexModel(ApiProcessor apiCall, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCall = apiCall;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                string errorMsg = string.Empty;
                returnUrl ??= Url.Content("~/Dashboard");
                #region GET PARAM
                var _url = _config.GetSection("apiUrls")["loginUrl"];
                var data = JsonConvert.SerializeObject(userprofile);
                var loginResp = await _apiCall.PostDataAsync(data, _url);
                if (string.IsNullOrWhiteSpace(loginResp))
                {
                    errorMsg = "No Response from Login Service";
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return Page();
                }
                var _respVal = JsonConvert.DeserializeObject<Response<loginResponseData>>(loginResp);
                if (_respVal == null) { _logger.WriteErrorLog($"LOGIN RESPONSE IS NULL"); ViewData.Add("LogInErrorMsg", "LOGIN RESPONSE IS NULL"); return Page(); }
                if (_respVal.Code != (long)ApiResponseCodes.OK)
                {
                    errorMsg = _respVal.Description;
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return Page();
                }
                #endregion
                #region ASSIGN DATA TO SESSION                
                var token = _respVal.Data.Token;
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                var email = jwtSecurityToken.Claims.First(claim => claim.Type == "email").Value;
                var role = jwtSecurityToken.Claims.First(claim => claim.Type == "role").Value;
                var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "UserId").Value;

                _signInMgm.SessionSet("accessToken", token);
                _signInMgm.SessionSet("email", email);
                _signInMgm.SessionSet("userName", userprofile.userID);
                _signInMgm.SessionSet("UserId", userId);
                _signInMgm.SessionSet("role", role);
                #endregion

                _logger.WriteErrorLog($"Welcome");
                return LocalRedirect(returnUrl);
            }
            catch (Exception eex)
            {
                _logger.WriteErrorLog($"Exception:: {eex?.ToString()}");
                TempData["ErrorMessage"] = "An error has occured";
                return Page();
            }
        }

        public IActionResult OnGetLogOut()
        {
            _signInMgm.SessionSet("accessToken", "");
            _signInMgm.SessionSet("email", "");
            _signInMgm.SessionSet("userName", "");
            _signInMgm.SessionSet("UserId", "");
            HttpContext.Session.Clear();
            return Page();
        }
    }
}