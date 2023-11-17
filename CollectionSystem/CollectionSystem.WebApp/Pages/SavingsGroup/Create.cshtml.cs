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
    public class CreateModel : PageModel
    {
        private readonly ApiProcessor _apiCalls;
        private readonly IConfiguration _config;
        private readonly ManageSignIn _signInMgm;
        private readonly Logger _logger = new Logger();

        [BindProperty] public NewSavingsGroupDTO savingsGroup { get; set; }

        public CreateModel(ApiProcessor apiCalls, IConfiguration config, ManageSignIn signInMgm, Logger logger)
        {
            _apiCalls = apiCalls;
            _config = config;
            _signInMgm = signInMgm;
            _logger = logger;
        }

        public IActionResult OnGet()
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
                    var _url = _config.GetSection("apiUrls")["createSavingsGroupUrl"];
                    savingsGroup.GroupAdminUserId = _signInMgm.UserId;
                    string data = JsonConvert.SerializeObject(savingsGroup);
                    string token = _signInMgm.AccessToken;
                    var savingsGroupResp = await _apiCalls.PostDataAsync(data, _url, token);
                    if (string.IsNullOrWhiteSpace(savingsGroupResp))
                    {
                        errorMsg = "No Response from Create SavingsGroup Service";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    var _respVal = JsonConvert.DeserializeObject<Response<bool>>(savingsGroupResp);
                    if (_respVal == null) { errorMsg = $"CREATE SAVINGS GROUP RESPONSE IS NULL"; _logger.WriteErrorLog(errorMsg); TempData["ErrorMessage"] = errorMsg; return Page(); }
                    if (_respVal.Code != (long)ApiResponseCodes.OK)
                    {
                        errorMsg = $"{_respVal.Message}";
                        _logger.WriteErrorLog(errorMsg);
                        TempData["ErrorMessage"] = errorMsg;
                        return Page();
                    }
                    _logger.WriteErrorLog(_respVal.Message);
                    TempData["SuccessMessage"] = _respVal.Message;
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
