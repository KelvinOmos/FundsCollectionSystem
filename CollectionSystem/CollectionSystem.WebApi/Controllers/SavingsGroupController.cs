using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionSystem.Application.DTO.SavingsGroup;
using CollectionSystem.Application.Interfaces.SavingsGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollectionSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsGroupController : ControllerBase
    {
        private ISavingsGroupService _savingsGroupService;

        public SavingsGroupController(ISavingsGroupService savingsGroupService)
        {
            _savingsGroupService = savingsGroupService;
        }

        [HttpPost("create-savings-group")]
        public async Task<IActionResult> CreateSavingsGroup(NewSavingsGroupDTO request)
        {
            Log.Information("Create Savings Group Service");
            return Ok(await _savingsGroupService.CreateSavingsGroup(request));
        }

        [HttpPost("fetch-savings-group-by-id")]
        public async Task<IActionResult> FetchSavingsGroupById(GetSavingsGroupByIdDTO request)
        {
            Log.Information("Create Savings Group By Id Service");
            return Ok(await _savingsGroupService.GetSavingsGroupById(request));
        }

        [HttpPost("fetch-user-savings-group")]
        public async Task<IActionResult> FetchUserSavingsGroup(GetUserSavingsGroupDTO request)
        {
            Log.Information("Get User Savings Group Service");
            return Ok(await _savingsGroupService.GetUserSavingsGroup(request));
        }

        [HttpPost("fetch-savings-group-members")]
        public async Task<IActionResult> FetchSavingsGroupMembers(GetSavingsGroupMemberDTO request)
        {
            Log.Information("Get User Savings Group Member Service");
            return Ok(await _savingsGroupService.GetSavingsGroupMembers(request));
        }

        [HttpGet("fetch-savings-groups")]
        public async Task<IActionResult> FetchSavingsGroups()
        {
            Log.Information("Create Savings Groups Service");
            return Ok(await _savingsGroupService.GetSavingsGroups());
        }

        [HttpPost("join-savings-group")]
        public async Task<IActionResult> JoinSavingsGroup(JoinSavingsGroupDTO request)
        {
            Log.Information("Join Savings Group Service");
            return Ok(await _savingsGroupService.JoinSavingsGroup(request));
        }
    }
}

