using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.API.Controllers
{
    [Route("stoneTypes")]
    [ApiController]
    public class StoneTypeController : CustomControllerBase
    {
        private readonly IStoneTypeService _stoneTypeService;

        public StoneTypeController(IStoneTypeService stoneTypeService)
        {
            _stoneTypeService = stoneTypeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isDeleted = false, [FromQuery] bool? isActive = null)
        {
            var response = await _stoneTypeService.GetAllAsync(isDeleted, isActive);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _stoneTypeService.GetAsync(id);
            return CreateResult(response);
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] bool? isDeleted = false, [FromQuery] bool? isActive = null)
        {
            var response = await _stoneTypeService.CountAsync(isDeleted, isActive);
            return CreateResult(response);
        }
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] StoneTypeCreateDto stoneTypeCreateDto)
        {
            var response = await _stoneTypeService.AddAsync(stoneTypeCreateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] StoneTypeUpdateDto stoneTypeUpdateDto)
        {
            var response = await _stoneTypeService.UpdateAsync(stoneTypeUpdateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _stoneTypeService.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        // [Authorize(Roles = "Admin")]
        [HttpDelete("hard-delete/{id:int}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _stoneTypeService.HardDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("toogle-active/{id:int}")]
        public async Task<IActionResult> ToogleActive(int id)
        {
            var response = await _stoneTypeService.ToggleIsActiveAsync(id);
            return CreateResult(response);
        }
    }
}
