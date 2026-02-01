using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.HeroBannerDtos;

namespace NoraCollection.API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/heroBanners")]
    [ApiController]
    public class HeroBannersController : CustomControllerBase
    {
        private readonly IHeroBannerService _heroBannerManager;

        public HeroBannersController(IHeroBannerService heroBannerManager)
        {
            _heroBannerManager = heroBannerManager;
        }
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBanners()
        {
            var response = await _heroBannerManager.GetActiveBannersAsync();
            return CreateResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll ([FromQuery] bool? isdeleted = false, [FromQuery] bool? isActive = null)
        {
            var response =await _heroBannerManager.GetAllAsync(isdeleted,isActive);
            return CreateResult(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _heroBannerManager.GetByIdAsync(id);
            return CreateResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] HeroBannerCreateDto heroBannerCreateDto)
        {
            var response  = await _heroBannerManager.AddAsync(heroBannerCreateDto);
            return CreateResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] HeroBannerUpdateDto heroBannerUpdateDto)
        {
            var response = await _heroBannerManager.UpdateAsync(heroBannerUpdateDto);
            return CreateResult(response);
        }
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _heroBannerManager.HardDeleteAsync(id);
            return CreateResult(response);
        }
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _heroBannerManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        [HttpPatch("{id:int}/order")]
        public async Task<IActionResult> UpdateOrder(int id,[FromQuery] int newOrder)
        {
            var response = await _heroBannerManager.UpdateOrderAsync(id,newOrder);
            return CreateResult(response);
        }
    }
}
