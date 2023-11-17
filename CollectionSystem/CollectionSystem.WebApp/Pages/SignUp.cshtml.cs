using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using CollectionSystem.WebApp;
using CollectionSystem.WebApp.Enums;
using CollectionSystem.WebApp.Helpers;
using CollectionSystem.WebApp.Models;
using CollectionSystem.WebApp.Processors;
using CollectionSystem.WebApp.Wrappers;

namespace CollectionSystem.WebApp.Pages
{
	public class SignUpModel : PageModel
    {
        private readonly ApiProcessor _apiCall;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public newUserDTO newUser { get; set; }

        public SignUpModel(ApiProcessor apiCall, IConfiguration config, ManageSignIn signInMgm, Logger logger)
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
                returnUrl ??= Url.Content("~/Index");
                #region SIGN UP
                var _url = _config.GetSection("apiUrls")["signupUrl"];
                newUser.Role = Roles.User.ToString();
                var data = JsonConvert.SerializeObject(newUser);
                var signupResp = await _apiCall.PostDataAsync(data, _url);
                if (string.IsNullOrWhiteSpace(signupResp))
                {
                    errorMsg = "No Response from Sign Up Service";
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return Page();
                }
                var _respVal = JsonConvert.DeserializeObject<Response<bool>>(signupResp);
                if (_respVal == null) { _logger.WriteErrorLog($"SIGN UP RESPONSE IS NULL"); TempData["ErrorMessage"] = "SIGN UP RESPONSE IS NULL"; return Page(); }
                if (_respVal.Code != (long)ApiResponseCodes.OK)
                {
                    errorMsg = _respVal.Description;
                    _logger.WriteErrorLog(errorMsg);
                    TempData["ErrorMessage"] = errorMsg;
                    return Page();
                }
                #endregion
                TempData["SuccessMessage"] = "SIGN UP SUCCESSFUL";
                _logger.WriteErrorLog($"SIGN UP SUCCESSFUL");
                return LocalRedirect(returnUrl);
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
