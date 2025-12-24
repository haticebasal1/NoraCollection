using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.CategoryDtos;

namespace NoraCollection.API.Controllers
{
    [Route("categories")]
    [ApiController]
    public class CategoriesController : CustomControllerBase
    {
        private readonly ICategoryService _categoryManager;

        public CategoriesController(ICategoryService categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryManager.GetAllAsync(isDeleted: false);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _categoryManager.GetAsync(id);
            return CreateResult(response);
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var response = await _categoryManager.GetBySlugAsync(slug);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] CategoryUpdateDto categoryUpdateDto)
        {
            var response = await _categoryManager.UpdateAsync(categoryUpdateDto);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _categoryManager.SoftDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("hard-delete/{id:int}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var response = await _categoryManager.HardDeleteAsync(id);
            return CreateResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-list")]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] bool? isDeleted)
        {
            var response = await _categoryManager.GetAllAsync(isDeleted);
            return CreateResult(response);
        }
    }
}
