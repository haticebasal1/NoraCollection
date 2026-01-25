using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.ColorDtos;

namespace NoraCollection.API.Controllers
{
    [Route("colors")]
    [ApiController]
    public class ColorsController : CustomControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isDeleted = false, [FromQuery] bool? isActive = null)
        {
            var response = await _colorService.GetAllAsync(isDeleted, isActive);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _colorService.GetAsync(id);
            return CreateResult(response);
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count([FromQuery] bool? isDeleted = false, [FromQuery] bool? isActive = null)
        {
            var response = await _colorService.CountAsync(isActive, isDeleted);
            return CreateResult(response);
        }
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ColorCreateDto colorCreateDto)
        {
            var response = await _colorService.AddAsync(colorCreateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update(ColorUpdateDto colorUpdateDto)
        {
            var response = await _colorService.UpdateAsync(colorUpdateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _colorService.SoftDeleteAsync(id);
            return CreateResult(response);
        }
       // [Authorize(Roles = "Admin")]
        [HttpDelete("hard-delete/{id:int}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _colorService.HardDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("toogle-active/{id:int}")]
        public async Task<IActionResult> ToogleActive(int id)
        {
            var response = await _colorService.ToggleIsActiveAsync(id);
            return CreateResult(response);
        }
    }
}
