using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.CampaignBarDtos;

namespace NoraCollection.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/campaignbars")]
    [ApiController]
    public class CampaignBarsController : CustomControllerBase
    {
        private readonly ICampaignBarService _campaignBarManager;

        public CampaignBarsController(ICampaignBarService campaignBarManager)
        {
            _campaignBarManager = campaignBarManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? includeDeleted = false, [FromQuery] bool? isActive = null)
        {
            var response = await _campaignBarManager.GetAllAsync(includeDeleted, isActive);
            return CreateResult(response);
        }
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBars()
        {
            var response = await _campaignBarManager.GetActiveBarsAsync();
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _campaignBarManager.GetByIdAsync(id);
            return CreateResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CampaignBarCreateDto campaignBarCreateDto)
        {
            var response = await _campaignBarManager.AddAsync(campaignBarCreateDto);
            return CreateResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CampaignBarUpdateDto campaignBarUpdateDto)
        {
            var response = await _campaignBarManager.UpdateAsync(campaignBarUpdateDto);
            return CreateResult(response);
        }
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _campaignBarManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
    }
}
